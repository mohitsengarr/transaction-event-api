using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Glasswall.Administration.K8.TransactionEventApi.Business.Serialisation
{
    public class JsonSerialiser : ISerialiser
    {
        public TObject Deserialise<TObject>(string jsonString)
        {
            if (jsonString == null) throw new ArgumentNullException(nameof(jsonString));
             
            return JsonConvert.DeserializeObject<TObject>(jsonString);
        }
    }
}
