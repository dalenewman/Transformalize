using NVelocity.Runtime;
using NVelocity.Runtime.Log;
using Pipeline.Contracts;
using LogLevel = NVelocity.Runtime.Log.LogLevel;

namespace Pipeline.Template.Velocity {
    public class VelocityLogSystem : ILogSystem {
        private readonly IContext _context;

        public VelocityLogSystem(IContext context) {
            _context = context;
        }

        public void Init(IRuntimeServices rs) {
        }

        public void LogVelocityMessage(LogLevel level, string message) {
            switch (level) {
                case LogLevel.Debug:
                    _context.Debug(() => message);
                    break;
                case LogLevel.Error:
                    _context.Error(message);
                    break;
                case LogLevel.Info:
                    _context.Info(message);
                    break;
                case LogLevel.Warn:
                    _context.Warn(message);
                    break;
            }
        }
    }
}
