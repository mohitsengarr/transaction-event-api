using System.Collections.Generic;
using System.Threading.Tasks;
using Glasswall.Administration.K8.TransactionEventApi.Common.Models.V1;

namespace Glasswall.Administration.K8.TransactionEventApi.Common.Services
{
    public interface IFileStore
    {
        Task<IEnumerable<GetTransactionsResponseV1>> GetFilesAsync(TransactionFilterV1 filter);
    }
}