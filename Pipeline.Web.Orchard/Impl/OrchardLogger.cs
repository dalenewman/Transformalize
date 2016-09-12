using System;
using Jint.Parser.Ast;
using Orchard.Logging;
using Pipeline.Configuration;
using Pipeline.Context;
using Pipeline.Contracts;
using Pipeline.Logging;
using LogLevel = Pipeline.Contracts.LogLevel;
using NullLogger = Orchard.Logging.NullLogger;

namespace Pipeline.Web.Orchard.Impl
{
    public class OrchardLogger : IPipelineLogger {
        private readonly ILogger _log;

        public Process Process { get; set; }
        public OrchardLogger() {
            _log = NullLogger.Instance;
        }
        public void Debug(PipelineContext context, Func<string> lambda) {
            _log.Debug(lambda());
        }

        public void Info(PipelineContext context, string message, params object[] args) {
            _log.Information(message, args);
        }

        public void Warn(PipelineContext context, string message, params object[] args) {
            _log.Warning(message, args);
            if (Process != null) {
                Process.Log.Add(new LogEntry(LogLevel.Warn, context, message, args));
            }
        }

        public void Error(PipelineContext context, string message, params object[] args) {
            _log.Error(message, args);
            if (Process != null) {
                Process.Log.Add(new LogEntry(LogLevel.Error, context, message, args));
            }
        }

        public void Error(PipelineContext context, Exception exception, string message, params object[] args) {
            _log.Error(exception, message, args);
        }

        public void Clear() {
        }

        public LogLevel LogLevel { get; private set; }
    }
}