using System;
using System.Collections.Generic;
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Logging;

namespace Transformalize.Main {

    public class ActionReader {

        private const StringComparison IC = StringComparison.OrdinalIgnoreCase;
        private readonly Process _process;

        public ActionReader(Process process) {
            _process = process;
        }

        public IEnumerable<TemplateAction> Read(List<TflAction> elements) {

            var actions = new List<TemplateAction>();
            foreach (var action in elements) {
                var modes = action.GetModes();
                if (modes.Length > 0 && !modes.Contains("*") && !modes.Any(m => m.Equals(_process.Mode, IC))) {
                    TflLogger.Debug(_process.Name, string.Empty, "Bypassing {0} action in {1} mode.", action.Action, _process.Mode);
                    continue;
                }

                var templateAction = new TemplateAction(_process, string.Empty, action);
                if (!String.IsNullOrEmpty(action.Connection)) {
                    templateAction.Connection = _process.Connections.GetConnectionByName(action.Connection).Connection;
                }

                actions.Add(templateAction);

            }
            return actions.ToArray();
        }
    }
}