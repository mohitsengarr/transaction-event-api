using System.ComponentModel.DataAnnotations;

namespace Glasswall.Administration.K8.TransactionEventApi.Common.Models.V1
{
    public class GetTransactionsRequestV1
    {
        [Required]
        public TransactionFilterV1 FilterV1 { get; set; }
    }
}