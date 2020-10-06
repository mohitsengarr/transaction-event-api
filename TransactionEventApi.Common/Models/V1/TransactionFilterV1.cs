using System;
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
        
        public Guid? FileId { get; set; }

        public FileType? DetectionFileType { get; set; }

        public Risk? Risk { get; set; }

        public Guid? ActivePolicy { get; set; }

        public string AnalysisReport { get; set; }
    }
}