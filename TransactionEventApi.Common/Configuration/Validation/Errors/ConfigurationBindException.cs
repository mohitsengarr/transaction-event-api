using System;
using System.Collections.Generic;
using System.Linq;

namespace Glasswall.Administration.K8.TransactionEventApi.Common.Configuration.Validation.Errors
{
    public class ConfigurationBindException : Exception
    {
        public ConfigurationBindException(IEnumerable<ConfigurationParserError> errors)
            : base("Error binding configuration: " + string.Join(Environment.NewLine, errors.Select(error => $"{error.ParamName} - {error.Reason}")))
        {
        }
    }
}