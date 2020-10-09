namespace Glasswall.Administration.K8.TransactionEventApi.Business.Serialisation
{
    public interface ISerialiser
    {
        TObject Deserialise<TObject>(string jsonString);
    }
}