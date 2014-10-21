using Transformalize.Logging;
using Transformalize.Main;

namespace Transformalize.Libs.NVelocity.Runtime.Log {
    public class NLogSystem : ILogSystem {
        private const string NONE = "None";

        public void Init(IRuntimeServices rs) {
        }

        public void LogVelocityMessage(LogLevel level, string message) {
            switch (level) {
                case LogLevel.Debug:
                    TflLogger.Debug(NONE, NONE, message);
                    break;
                case LogLevel.Error:
                    TflLogger.Error(NONE, NONE, message);
                    break;
                case LogLevel.Warn:
                    TflLogger.Warn(NONE, NONE, message);
                    break;
                default:
                    TflLogger.Info(NONE, NONE, message);
                    break;
            }
        }
    }
}