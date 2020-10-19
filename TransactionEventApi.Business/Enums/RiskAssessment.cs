using Glasswall.Administration.K8.TransactionEventApi.Common.Enums;

namespace Glasswall.Administration.K8.TransactionEventApi.Business.Enums
{
    public static class RiskAssessment
    {
        public static Risk DetermineRisk(NCFSOutcome ncfsOutcome, GwOutcome gwOutcome)
        {
            var risk = ncfsOutcome switch
            {
                NCFSOutcome.Relayed => Risk.AllowedByNCFS,
                NCFSOutcome.Replaced => Risk.Safe,
                NCFSOutcome.Blocked => Risk.AllowedByNCFS,
                _ => Risk.Unknown
            };

            if (risk == Risk.Unknown)
            {
                risk = gwOutcome switch
                {
                    GwOutcome.Replace => Risk.Safe,
                    GwOutcome.Unmodified => Risk.AllowedByPolicy,
                    GwOutcome.Failed => Risk.BlockedByPolicy,
                    _ => Risk.Unknown
                };
            }

            return risk;
        }
    }
}