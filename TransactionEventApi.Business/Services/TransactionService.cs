using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Glasswall.Administration.K8.TransactionEventApi.Common.Enums;
using Glasswall.Administration.K8.TransactionEventApi.Common.Models.V1;
using Glasswall.Administration.K8.TransactionEventApi.Common.Services;

namespace Glasswall.Administration.K8.TransactionEventApi.Business.Services
{
    public class TransactionService : ITransactionService
    {
        public Task<IEnumerable<GetTransactionsResponseV1>> GetTransactionsAsync(GetTransactionsRequestV1 requestV1)
        {
            if (requestV1 == null) throw new ArgumentNullException(nameof(requestV1));

            return InternalGetTransactionsAsync(requestV1);
        }

        private static async Task<IEnumerable<GetTransactionsResponseV1>> InternalGetTransactionsAsync(GetTransactionsRequestV1 requestV1)
        {
            return await Task.FromResult(Enumerable.Range(0, new Random().Next(0, 1000000)).Select(s =>
                new GetTransactionsResponseV1
                {
                    ActivePolicy = Guid.NewGuid(),
                    AnalysisReport = "<xml></xml>",
                    DetectionFileType = GetRandomEnum<FileType>(),
                    Risk = GetRandomEnum<Risk>(),
                    FileId = Guid.NewGuid(),
                    Timestamp = requestV1.FilterV1.TimestampRangeStart.GetValueOrDefault()
                }));
        }
        
        private static TEnum GetRandomEnum<TEnum>()
        {
            var values = Enum.GetValues(typeof(TEnum)).Cast<TEnum>().ToList();
            return values.ElementAt(new Random().Next(0, values.Count));
        }
    }
}