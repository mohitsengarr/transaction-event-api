using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Glasswall.Administration.K8.TransactionEventApi.Common.Configuration.Validation.Errors;

namespace Glasswall.Administration.K8.TransactionEventApi.Business.Configuration
{
    [Serializable]
    public class ConfigurationBindException : Exception
    {
        public ConfigurationBindException(IEnumerable<ConfigurationParserError> errors)
            : base("Error binding configuration: " + string.Join(Environment.NewLine, errors.Select(error => $"{error.ParamName} - {error.Reason}")))
        {
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }
    }
}