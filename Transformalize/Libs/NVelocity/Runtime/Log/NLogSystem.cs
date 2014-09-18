using Transformalize.Libs.NLog;

namespace Transformalize.Libs.NVelocity.Runtime.Log {
    public class NLogSystem : ILogSystem {
        private Logger _log;
        public void Init(IRuntimeServices rs) {
            _log = NLog.LogManager.GetLogger("tfl");
        }

        public void LogVelocityMessage(LogLevel level, string message) {
            switch (level) {
                case LogLevel.Debug:
                    _log.Debug(message);
                    break;
                case LogLevel.Error:
                    _log.Error(message);
                    break;
                case LogLevel.Warn:
                    _log.Warn(message);
                    break;
                default:
                    _log.Info(message);
                    break;
            }
        }
    }
}