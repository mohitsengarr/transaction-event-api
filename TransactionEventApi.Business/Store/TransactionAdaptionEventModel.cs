using System;
using System.Collections.Generic;
using Glasswall.Administration.K8.TransactionEventApi.Common.Enums;

namespace Glasswall.Administration.K8.TransactionEventApi.Business.Store
{
    /// <summary>
    /// Represents the model sent by each step in the adaption process
    /// </summary>
    public class TransactionAdaptionEventModel
    {
        public Dictionary<string, string> Properties { get; set; }

        public static TransactionAdaptionEventModel NewDocumentEvent(
            Guid? policyId = null,
            RequestMode? mode = null,
            Guid? fileId = null,
            DateTimeOffset? timestamp = null)
        {
            var model = Create(EventId.NewDocument, fileId, timestamp);
            model.Properties.Add("PolicyId", policyId.GetValueOrDefault(Guid.NewGuid()).ToString());
            model.Properties.Add("RequestMode", mode.GetValueOrDefault(RequestMode.Response).ToString());
            return model;
        }

        public static TransactionAdaptionEventModel FileTypeDetectedEvent(
            FileType fileType,
            Guid? fileId = null,
            DateTimeOffset? timestamp = null)
        {
            var model = Create(EventId.FileTypeDetected, fileId, timestamp);
            model.Properties.Add("FileType", fileType.ToString());
            return model;
        }

        public static TransactionAdaptionEventModel RebuildEventStarting(
            Guid? fileId = null,
            DateTimeOffset? timestamp = null)
        {
            var model = Create(EventId.RebuildStarted, fileId, timestamp);
            return model;
        }

        public static TransactionAdaptionEventModel RebuildCompletedEvent(
            GwOutcome gwOutcome,
            Guid? fileId = null,
            DateTimeOffset? timestamp = null)
        {
            var model = Create(EventId.RebuildCompleted, fileId, timestamp);
            model.Properties.Add("GwOutcome", gwOutcome.ToString());
            return model;
        }

        public static TransactionAdaptionEventModel AnalysisCompletedEvent(
            Guid? fileId = null,
            DateTimeOffset? timestamp = null)
        {
            var model = Create(EventId.AnalysisCompleted, fileId, timestamp);
            return model;
        }

        public static TransactionAdaptionEventModel NcfsStartedEvent(
            Guid? fileId = null,
            DateTimeOffset? timestamp = null)
        {
            var model = Create(EventId.NCFSStartedEvent, fileId, timestamp);
            return model;
        }

        public static TransactionAdaptionEventModel NcfsCompletedEvent(
            NCFSOutcome ncfsOutcome,
            Guid? fileId = null,
            DateTimeOffset? timestamp = null)
        {
            var model = Create(EventId.NCFSCompletedEvent, fileId, timestamp);
            model.Properties.Add("NCFSOutcome", ncfsOutcome.ToString());
            return model;
        }

        private static TransactionAdaptionEventModel Create(EventId eventId, Guid? fileId, DateTimeOffset? timestamp)
        {
            return new TransactionAdaptionEventModel
            {
                Properties = new Dictionary<string, string>
                {
                    ["FileId"] = fileId?.ToString() ?? Guid.NewGuid().ToString(),
                    ["EventId"] = ((int)eventId).ToString(),
                    ["Timestamp"] = timestamp.GetValueOrDefault(DateTimeOffset.UtcNow).ToString("O")
                }
            };
        }
    }
}