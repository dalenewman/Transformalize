using System;
using System.Collections.Generic;
using Orchard.ContentManagement;
using Orchard.Logging;
using Orchard.Tasks.Scheduling;
using Transformalize.Orchard.Models;

namespace Transformalize.Orchard.Services {
    public class TransformalizeTaskHandler : IScheduledTaskHandler {

        private readonly ITransformalizeService _tfl;
        private const string TRANSFORMALIZE_TYPE = "Transformalize";

        public ILogger Logger { get; set; }

        public TransformalizeTaskHandler(ITransformalizeService tfl) {
            _tfl = tfl;
            Logger = NullLogger.Instance;
        }

        public void Process(ScheduledTaskContext context) {
            var part = context.Task.ContentItem.As<ConfigurationPart>();
            if (part == null)
                return;

            var name = GetName(part.Title());
            if (context.Task.TaskType != name)
                return;

            Logger.Information("Running {0}", name);

            if (part.TryCatch) {
                try {
                    _tfl.Run(new TransformalizeRequest(part, new Dictionary<string, string>(), null, Logger));
                } catch (Exception ex) {
                    Logger.Error(ex, ex.Message);
                }
            } else {
                _tfl.Run(new TransformalizeRequest(part, new Dictionary<string, string>(), null, Logger));
            }
        }

        public static string GetName(string title) {
            return TRANSFORMALIZE_TYPE + " " + title;
        }
    }
}