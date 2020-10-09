using System;
using System.Collections.Generic;
using System.Linq;
using Glasswall.Administration.K8.TransactionEventApi.Common.Configuration.Validation.Errors;

namespace Glasswall.Administration.K8.TransactionEventApi.Common.Configuration.Validation.Validator
{
    public class TimespanValidator
        : IConfigurationItemValidator
    {
        private readonly TimeSpan _minLengthInclusive;
        private readonly TimeSpan _maxLengthInclusive;
        private readonly bool _optional;

        public TimespanValidator(TimeSpan? minLengthInclusive = null, TimeSpan? maxLengthInclusive = null)
        {
            _minLengthInclusive = minLengthInclusive.GetValueOrDefault();
            _maxLengthInclusive = maxLengthInclusive.GetValueOrDefault();
            _optional = minLengthInclusive == null && maxLengthInclusive == null;
        }

        public bool TryParse(string key, string rawValue, List<ConfigurationParserError> validationErrors, out object parsed)
        {
            if (validationErrors == null) throw new ArgumentNullException(nameof(validationErrors));

            var canParse = TimeSpan.TryParse(rawValue, out var p);
            parsed = p;

            var thisItemsErrors = new List<ConfigurationParserError>();
            
            if (!_optional)
            {
                if (string.IsNullOrWhiteSpace(rawValue))
                    thisItemsErrors.Add(new ConfigurationParserError(key, "ColumnValue is required."));

                if (!canParse)
                {
                    thisItemsErrors.Add(new ConfigurationParserError(key,
                        $"Value must be a timespan. Got {rawValue}"));
                }
                else
                {
                    parsed = p;
                    if (p < _minLengthInclusive)
                        thisItemsErrors.Add(new ConfigurationParserError(key,
                            $"Value must be at least {_minLengthInclusive}. Got {p}"));

                    if (p > _maxLengthInclusive)
                        thisItemsErrors.Add(new ConfigurationParserError(key,
                            $"Value must not be longer than {_minLengthInclusive}. Got {p}"));
                }

                validationErrors.AddRange(thisItemsErrors);
            }

            return !thisItemsErrors.Any();
        }
    }
}