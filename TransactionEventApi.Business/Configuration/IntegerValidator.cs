using System;
using System.Collections.Generic;
using System.Linq;
using Glasswall.Administration.K8.TransactionEventApi.Common.Configuration;
using Glasswall.Administration.K8.TransactionEventApi.Common.Configuration.Validation;
using Glasswall.Administration.K8.TransactionEventApi.Common.Configuration.Validation.Errors;

namespace Glasswall.Administration.K8.TransactionEventApi.Business.Configuration
{
    public class IntegerValidator
        : IConfigurationItemValidator
    {
        private readonly int _minValInclusive;
        private readonly int _maxValInclusive;
        private readonly bool _optional;

        public IntegerValidator(int? minVal, int? maxVal)
        {
            _minValInclusive = minVal.GetValueOrDefault();
            _maxValInclusive = maxVal.GetValueOrDefault();
            _optional = minVal == null && maxVal == null;
        }

        public bool TryParse(string key, string rawValue, List<ConfigurationParserError> validationErrors, out object parsed)
        {
            if (validationErrors == null) throw new ArgumentNullException(nameof(validationErrors));

            var thisItemsErrors = new List<ConfigurationParserError>();

            if (!_optional)
            {
                if (string.IsNullOrWhiteSpace(rawValue))
                    thisItemsErrors.Add(new ConfigurationParserError(key, "ColumnValue is required."));

                if (rawValue?.Length < _minValInclusive)
                    thisItemsErrors.Add(new ConfigurationParserError(key, $"ColumnValue must be at least {_minValInclusive}. Got {rawValue.Length}"));

                if (rawValue?.Length > _maxValInclusive)
                    thisItemsErrors.Add(new ConfigurationParserError(key, $"ColumnValue must not be more than {_maxValInclusive}. Got {rawValue.Length}"));

                validationErrors.AddRange(thisItemsErrors);
            }

            parsed = int.TryParse(rawValue, out var result) ? result : 0;
            return !thisItemsErrors.Any();
        }
    }
}