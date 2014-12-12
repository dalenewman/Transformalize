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

        public IEnumerable<TemplateAction> Read(ActionElementCollection elements) {

            var actions = new List<TemplateAction>();
            foreach (ActionConfigurationElement action in elements) {
                var modes = action.GetModes();
                if (modes.Length > 0 && !modes.Contains("*") && !modes.Any(m => m.Equals(_process.Mode, IC))) {
                    TflLogger.Debug(_process.Name, string.Empty, "Bypassing {0} action in {1} mode.", action.Action, _process.Mode);
                    continue;
                }

                var templateAction = new TemplateAction(_process, string.Empty, action, modes);

                if (!String.IsNullOrEmpty(action.Connection)) {
                    if (_process.Connections.ContainsKey(action.Connection)) {
                        templateAction.Connection = _process.Connections[action.Connection];
                    } else {
                        var message = string.Format("The template '{0}' refers to an invalid connection named '{1}'.", action.Action, action.Connection);
                        throw new TransformalizeException(_process.Name, string.Empty, message);
                    }
                }

                actions.Add(templateAction);

            }
            return actions.ToArray();
        }
    }
}