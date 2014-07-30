using System;
using System.Collections.Generic;
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Libs.NLog;

namespace Transformalize.Main {

    public class ActionReader {
        private readonly Logger _log = LogManager.GetLogger("tfl");
        private const StringComparison IC = StringComparison.OrdinalIgnoreCase;
        private readonly Process _process;

        public ActionReader(Process process) {
            _process = process;
        }

        public IEnumerable<TemplateAction> Read(ActionElementCollection actions) {
            foreach (ActionConfigurationElement action in actions) {
                var modes = action.GetModes();
                if (modes.Length > 0 && !modes.Contains("*") && !modes.Any(m => m.Equals(_process.Mode, IC)))
                    continue;

                var templateAction = new TemplateAction(_process, string.Empty, action, modes);

                if (!String.IsNullOrEmpty(action.Connection)) {
                    if (_process.Connections.ContainsKey(action.Connection)) {
                        templateAction.Connection = _process.Connections[action.Connection];
                    } else {
                        var message = string.Format("The template '{0}' refers to an invalid connection named '{1}'.", action.Action, action.Connection);
                        _log.Error(message);
                        throw new TransformalizeException(message);
                    }
                }

                yield return templateAction;

            }

        }
    }
}