using Transformalize.Libs.NVelocity.Runtime;
using Transformalize.Libs.NVelocity.Runtime.Log;

namespace Transformalize.Logging {

    public class VelocityLogSystem : ILogSystem {
        private readonly ILogger _logger;

        public VelocityLogSystem(ILogger logger) {
            _logger = logger;
        }

        public void Init(IRuntimeServices rs) {
        }

        public void LogVelocityMessage(LogLevel level, string message) {
            switch (level) {
                case LogLevel.Debug:
                    _logger.Debug(message);
                    break;
                case LogLevel.Error:
                    _logger.Error(message);
                    break;
                case LogLevel.Info:
                    _logger.Info(message);
                    break;
                case LogLevel.Warn:
                    _logger.Warn(message);
                    break;
            }
        }
    }
}