using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Glasswall.Administration.K8.TransactionEventApi.Common.Models.V1;
using Glasswall.Administration.K8.TransactionEventApi.Common.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Glasswall.Administration.K8.TransactionEventApi.Controllers
{
    [ApiController]
    [Route("api/v1/transactions")]
    public class TransactionController : ControllerBase
    {
        private readonly ILogger<TransactionController> _logger;
        private readonly ITransactionService _transactionService;

        public TransactionController(ILogger<TransactionController> logger, ITransactionService transactionService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _transactionService = transactionService ?? throw new ArgumentNullException(nameof(transactionService));
        }

        [HttpPost]
        [ValidateModel]
        public async Task<IEnumerable<GetTransactionsResponseV1>> Get([Required][FromBody]GetTransactionsRequestV1 requestV1)
        {
            if (requestV1 == null) throw new ArgumentNullException(nameof(requestV1));

            _logger.LogInformation("Beginning get transactions requestV1");

            var transactions = await _transactionService.GetTransactionsAsync(requestV1);

            _logger.LogInformation("Finished get transactions requestV1");

            return transactions;
        }
    }
}
