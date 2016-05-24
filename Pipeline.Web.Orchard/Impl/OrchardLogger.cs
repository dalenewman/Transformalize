using System;
using Orchard.Logging;
using Pipeline.Context;
using Pipeline.Contracts;
using LogLevel = Pipeline.Contracts.LogLevel;

namespace Pipeline.Web.Orchard.Impl
{
    public class OrchardLogger : IPipelineLogger {
        private readonly ILogger _log;

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
        }

        public void Error(PipelineContext context, string message, params object[] args) {
            _log.Error(message, args);
        }

        public void Error(PipelineContext context, Exception exception, string message, params object[] args) {
            _log.Error(exception, message, args);
        }

        public void Clear() {
        }

        public LogLevel LogLevel { get; private set; }
    }
}