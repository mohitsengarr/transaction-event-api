namespace Glasswall.Administration.K8.TransactionEventApi.Common.Configuration
{
    public interface IConfigurationParser
    {
        TConfiguration Parse<TConfiguration>() where TConfiguration : new();
    }
}