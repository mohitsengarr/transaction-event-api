using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.Storage.Files.Shares;
using Glasswall.Administration.K8.TransactionEventApi.Business.Serialisation;
using Glasswall.Administration.K8.TransactionEventApi.Business.Store;
using Glasswall.Administration.K8.TransactionEventApi.Common.Enums;
using Glasswall.Administration.K8.TransactionEventApi.Common.Models.V1;
using Glasswall.Administration.K8.TransactionEventApi.Common.Services;
using Microsoft.Extensions.Logging;
using EventId = Glasswall.Administration.K8.TransactionEventApi.Common.Enums.EventId;

namespace Glasswall.Administration.K8.TransactionEventApi.Business.Services
{
    public class FileStore : IFileStore
    {
        private readonly ILogger<IFileStore> _logger;
        private readonly ISerialiser _serialiser;
        private readonly string _connectionString;

        public FileStore(
            ILogger<IFileStore> logger,
            ISerialiser serialiser,
            string connectionString)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _serialiser = serialiser ?? throw new ArgumentNullException(nameof(serialiser));
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        public Task<IEnumerable<GetTransactionsResponseV1>> GetFilesAsync(TransactionFilterV1 filter)
        {
            if (filter == null) throw new ArgumentNullException(nameof(filter));

            return InternalQueryFilesAsync(filter);
        }

        private async Task<IEnumerable<GetTransactionsResponseV1>> InternalQueryFilesAsync(TransactionFilterV1 filter)
        {
            var shareService = new ShareServiceClient(_connectionString);
            var share = shareService.GetShareClient("transactions");
            var rootDirectory = share.GetRootDirectoryClient();

            var filePaths = await GetFilePathsThatMatchDateRange(rootDirectory, filter);
            return await ProcessFiles(rootDirectory, filter, filePaths.ToArray());
        }

        private async Task<IEnumerable<GetTransactionsResponseV1>> ProcessFiles(
            ShareDirectoryClient directory,
            TransactionFilterV1 filter,
            IReadOnlyCollection<string> filePaths)
        {
            var files = new List<GetTransactionsResponseV1>(filePaths.Count);
            
            foreach (var filePath in filePaths)
            {
                var fileClient = directory.GetFileClient(filePath);
                var fileContents = await fileClient.DownloadAsync();

                TransactionAdapationEventMetadataFile metadataFile = null;

                using (var ms = new MemoryStream())
                {
                    await fileContents.Value.Content.CopyToAsync(ms);
                    var fileBytes = ms.ToArray();
                    var fileString = Encoding.UTF8.GetString(fileBytes);
                    metadataFile = _serialiser.Deserialise<TransactionAdapationEventMetadataFile>(fileString);
                }

                var newDocumentEvent = metadataFile.Events.EventOrDefault(EventId.NewDocument);
                var fileTypeDetectedEvent = metadataFile.Events.EventOrDefault(EventId.FileTypeDetected);

                Guid.TryParse(newDocumentEvent.PropertyOrDefault("PolicyId"), out var policyId);
                Guid.TryParse(newDocumentEvent.PropertyOrDefault("FileId"), out var fileId);
                DateTimeOffset.TryParse(newDocumentEvent.PropertyOrDefault("Timestamp"), out var timestamp);

                var response = new GetTransactionsResponseV1
                {
                    ActivePolicyId = policyId,
                    DetectionFileType = fileTypeDetectedEvent.PropertyOrDefault<FileType>("FileType").GetValueOrDefault(FileType.Unknown),
                    FileId = fileId,
                    Risk = Risk.Safe, // TBD - need to work out logic
                    Timestamp = timestamp
                };

                files.Add(response);
            }

            return files;
        }

        private async Task<IEnumerable<string>> GetFilePathsThatMatchDateRange(ShareDirectoryClient directory, TransactionFilterV1 filter)
        {
            var paths = new List<string>();
            _logger.LogInformation("Listing metadata to download");
            var start = filter.TimestampRangeStart.GetValueOrDefault();
            var end = filter.TimestampRangeEnd.GetValueOrDefault();
            await RecurseAndCollectMetdataPaths(directory, start, end, paths);
            _logger.LogInformation($"Listed {paths.Count} to download");
            return paths;
        }

        private async Task RecurseAndCollectMetdataPaths(
            ShareDirectoryClient currentDirectoryClient, 
            DateTimeOffset rangeStart,
            DateTimeOffset rangeEnd,
            ICollection<string> collection)
        {
            if (collection.Count % 100 == 0)
                _logger.LogInformation($"Listing metadata to download - {collection.Count} so far");

            await foreach (var item in currentDirectoryClient.GetFilesAndDirectoriesAsync())
            {
                var fullPath = $"{currentDirectoryClient.Path}{(currentDirectoryClient.Path == "" ? "": "/")}{item.Name}";

                if (!item.IsDirectory) continue;

                var parts = fullPath.Split('/');

                // reached file id
                if (parts.Length == 5) collection.Add($"{fullPath}/metadata.json");

                var recurse = parts.Length switch
                {
                    1 => YearInRange(rangeStart, rangeEnd, parts),
                    2 => MonthFolderInRange(rangeStart, rangeEnd, parts),
                    3 => DayOfMonthInRange(rangeStart, rangeEnd, parts),
                    4 => HourOfDayInRange(rangeStart, rangeEnd, parts),
                    _ => false 
                };

                if (!recurse) continue;

                await RecurseAndCollectMetdataPaths(currentDirectoryClient.GetSubdirectoryClient(item.Name), rangeStart, rangeEnd, collection);
            }
        }

        private static bool YearInRange(DateTimeOffset start, DateTimeOffset end, IReadOnlyList<string> folderParts)
        {
            if (!int.TryParse(folderParts[0], out var val)) return false;
            return val >= start.Year && val <= end.Year;
        }

        private static bool MonthFolderInRange(DateTimeOffset start, DateTimeOffset end, IReadOnlyList<string> folderParts)
        {
            if (!int.TryParse(folderParts[0], out var parsedYear)) return false;
            if (!int.TryParse(folderParts[1], out var parsedMonth)) return false;

            if (start.Year == end.Year) return parsedMonth >= start.Month && parsedMonth <= start.Month;
            if (parsedYear == start.Year) return parsedMonth >= start.Month;
            if (parsedYear == end.Year) return parsedMonth <= end.Month;
            return true;
        }

        private static bool DayOfMonthInRange(DateTimeOffset start, DateTimeOffset end, IReadOnlyList<string> folderParts)
        {
            if (!int.TryParse(folderParts[0], out var parsedYear)) return false;
            if (!int.TryParse(folderParts[1], out var parsedMonth)) return false;
            if (!int.TryParse(folderParts[2], out var parsedDay)) return false;

            var date = new DateTimeOffset(new DateTime(parsedYear, parsedMonth, parsedDay));
            return date >= start && date <= end;
        }

        private static bool HourOfDayInRange(DateTimeOffset start, DateTimeOffset end, IReadOnlyList<string> folderParts)
        {
            if (!int.TryParse(folderParts[0], out var parsedYear)) return false;
            if (!int.TryParse(folderParts[1], out var parsedMonth)) return false;
            if (!int.TryParse(folderParts[2], out var parsedDay)) return false;
            if (!int.TryParse(folderParts[3], out var parsedHour)) return false;

            var date = new DateTimeOffset(new DateTime(parsedYear, parsedMonth, parsedDay, parsedHour, 0, 0));
            return date >= start && date <= end;
        }
    }
}