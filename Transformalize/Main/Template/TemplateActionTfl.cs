using System;
using System.Collections.Generic;
using Transformalize.Logging;

namespace Transformalize.Main {
    public class TemplateActionTfl : TemplateActionHandler {

        public override void Handle(TemplateAction action) {
            Dictionary<string, string> parameters = null;

            if (action.Url.IndexOf('?') > 0) {
                parameters = Common.ParseQueryString(new Uri(action.Url).Query);
            }

            var processes = ProcessFactory.Create(action.Url, new Options(), parameters);
            foreach (var process in processes) {
                process.ShouldLog = false;
                TflLogger.Info(action.ProcessName, string.Empty, "Executing {0}", process.Name);
                process.ExecuteScaler();
            }
        }
    }
}