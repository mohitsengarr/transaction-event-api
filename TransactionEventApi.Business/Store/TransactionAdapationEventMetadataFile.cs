using System.Collections.Generic;

namespace Glasswall.Administration.K8.TransactionEventApi.Business.Store
{
    /// <summary>
    /// Structure of the file
    /// </summary>
    public class TransactionAdapationEventMetadataFile
    {
        public IEnumerable<TransactionAdaptionEventModel> Events { get; set; }
    }
}