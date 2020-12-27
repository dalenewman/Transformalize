using System;
using Orchard.Logging;
using Transformalize.Configuration;
using Transformalize.Contracts;
using Transformalize.Logging;
using LogLevel = Transformalize.Contracts.LogLevel;
using NullLogger = Orchard.Logging.NullLogger;

namespace Pipeline.Web.Orchard.Impl
{
    public class OrchardLogger : IPipelineLogger {
        private readonly ILogger _log;

        public Process Process { get; set; }
        public OrchardLogger() {
            _log = NullLogger.Instance;
        }
        public void Debug(IContext context, Func<string> lambda) {
            _log.Debug(lambda());
        }

        public void Info(IContext context, string message, params object[] args) {
            _log.Information(message, args);
        }

        public void Warn(IContext context, string message, params object[] args) {
            _log.Warning(message, args);
            if (Process != null) {
                Process.Log.Add(new LogEntry(LogLevel.Warn, context, message, args));
            }
        }

        public void Error(IContext context, string message, params object[] args) {
            _log.Error(message, args);
            if (Process != null) {
                Process.Log.Add(new LogEntry(LogLevel.Error, context, message, args));
            }
        }

        public void Error(IContext context, Exception exception, string message, params object[] args) {
            _log.Error(exception, message, args);
        }

        public void Clear() {}

        public void SuppressConsole() {}

        public LogLevel LogLevel { get; private set; }
    }
}