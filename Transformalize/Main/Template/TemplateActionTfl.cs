using System;
using System.Collections.Generic;
using Transformalize.Logging;

namespace Transformalize.Main {

    public class TemplateActionTfl : TemplateActionHandler {

        private readonly ILogger _logger;

        public TemplateActionTfl(ILogger logger) {
            _logger = logger;
        }

        public override void Handle(TemplateAction action) {
            Dictionary<string, string> parameters = null;

            if (action.Url.IndexOf('?') > 0) {
                parameters = Common.ParseQueryString(new Uri(action.Url).Query);
            }

            var name = _logger.Name; 
            var processes = ProcessFactory.Create(action.Url, _logger, new Options(), parameters);
            foreach (var process in processes) {
                _logger.Info("Executing {0}", process.Name);
                _logger.Name = process.Name;
                process.ExecuteScaler();
            }
            _logger.Name = name;
        }
    }
}