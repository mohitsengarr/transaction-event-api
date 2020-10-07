using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Glasswall.Administration.K8.TransactionEventApi.Common.Enums;

namespace Glasswall.Administration.K8.TransactionEventApi.Common.Models.V1
{
    public class TransactionFilterV1
    {
        [Required]
        public DateTimeOffset? TimestampRangeStart { get; set; }

        [Required]
        public DateTimeOffset? TimestampRangeEnd { get; set; }
        
        public IEnumerable<Guid> FileId { get; set; }

        public IEnumerable<FileType> DetectionFileType { get; set; }

        public IEnumerable<Risk> Risk { get; set; }

        public IEnumerable<Guid> ActivePolicy { get; set; }
    }
}