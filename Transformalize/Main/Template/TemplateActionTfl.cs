using Transformalize.Logging;

namespace Transformalize.Main {
    public class TemplateActionTfl : TemplateActionHandler {

        private readonly ILogger _logger;

        public TemplateActionTfl(ILogger logger) {
            _logger = logger;
        }

        public override void Handle(TemplateAction action) {
            var name = _logger.Name;
            var resource = string.IsNullOrEmpty(action.Url) ? action.File : action.Url;
            var processes = ProcessFactory.Create(resource, _logger, new Options());
            foreach (var process in processes) {
                _logger.Warn("Executing {0}", process.Name);
                _logger.Name = process.Name;
                process.ExecuteScaler();
            }
            _logger.Name = name;
        }
    }
}