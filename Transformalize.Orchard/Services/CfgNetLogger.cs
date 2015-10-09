using Orchard.Logging;

namespace Transformalize.Orchard.Services {

    public class CfgNetLogger : Cfg.Net.Contracts.ILogger {

        private readonly ILogger _logger;

        public CfgNetLogger(ILogger logger) {
            _logger = logger;
        }

        public void Warn(string message, params object[] args) {
            _logger.Warning(message, args);
        }

        public void Error(string message, params object[] args) {
            _logger.Error(message, args);
        }
    }
}