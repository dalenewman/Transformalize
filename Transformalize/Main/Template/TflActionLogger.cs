using Transformalize.Logging;

namespace Transformalize.Main {
    public class TflActionLogger : Cfg.Net.Contracts.ILogger {
        private readonly ILogger _tflLogger;

        public TflActionLogger(ILogger tflLogger) {
            _tflLogger = tflLogger;
        }

        public void Warn(string message, params object[] args) {
            _tflLogger.Warn(message, args);
        }

        public void Error(string message, params object[] args) {
            _tflLogger.Error(message, args);
        }
    }
}