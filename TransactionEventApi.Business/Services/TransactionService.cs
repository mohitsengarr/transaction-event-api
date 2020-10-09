using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Glasswall.Administration.K8.TransactionEventApi.Common.Models.V1;
using Glasswall.Administration.K8.TransactionEventApi.Common.Services;
using Microsoft.Extensions.Logging;

namespace Glasswall.Administration.K8.TransactionEventApi.Business.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ILogger<ITransactionService> _logger;
        private readonly IEnumerable<IFileStore> _fileStores;

        public TransactionService(ILogger<ITransactionService> logger, IEnumerable<IFileStore> fileStores)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _fileStores = fileStores ?? throw new ArgumentNullException(nameof(fileStores));
        }

        public Task<IEnumerable<GetTransactionsResponseV1>> GetTransactionsAsync(GetTransactionsRequestV1 requestV1)
        {
            if (requestV1 == null) throw new ArgumentNullException(nameof(requestV1));

            return InternalGetTransactionsAsync(requestV1);
        }

        private async Task<IEnumerable<GetTransactionsResponseV1>> InternalGetTransactionsAsync(GetTransactionsRequestV1 request)
        {
            _logger.LogInformation("Searching file store ");

            var tasks = _fileStores.Select(fileStore => fileStore.GetFilesAsync(request.Filter));

            return (await Task.WhenAll(tasks)).SelectMany(s => s);
        }
    }
}