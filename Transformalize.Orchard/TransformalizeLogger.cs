using System;
using System.Text;
using Orchard.Logging;
using tflLogger = Transformalize.Logging.ILogger;
using Transformalize.Extensions;

namespace Transformalize.Orchard {

    public class TransformalizeLogger : tflLogger {
        private readonly string _level;
        private readonly string _process;
        private readonly ILogger _logger;
        private readonly string _orchardVersion;
        private readonly string _moduleVersion;

        private const string DELIMITER = " | ";

        private readonly StringBuilder _builder = new StringBuilder();

        public TransformalizeLogger(string process, ILogger logger, string level, string orchardVersion, string moduleVersion) {
            _level = level.ToLower().Left(4);
            _process = process;
            _logger = logger;
            _orchardVersion = orchardVersion;
            _moduleVersion = moduleVersion;
        }

        public string[] Dump() {
            return _builder.ToString().Split(new []{Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries);
        }

        public void Info(string message, params object[] args) {
            EntityInfo(".", message, args);
        }

        public void Debug(string message, params object[] args) {
            EntityDebug(".", message, args);
        }

        public void Warn(string message, params object[] args) {
            EntityWarn(".", message, args);
        }

        public void Error(string message, params object[] args) {
            EntityError(".", message, args);
        }

        public void Error(Exception exception, string message, params object[] args) {
            EntityError(".", exception, message, args);
        }

        public void EntityInfo(string entity, string message, params object[] args) {
            if (_level != "none") {
                AppendLevel("info ", entity);
                _builder.AppendFormat(message, args);
                _builder.AppendLine();
            }
            if (!_logger.IsEnabled(LogLevel.Information))
                return;

            _logger.Information(message, args);

        }

        public void EntityDebug(string entity, string message, params object[] args) {
            if (_level == "debu") {
                AppendLevel("debug", entity);
                _builder.AppendFormat(message, args);
                _builder.AppendLine();
            }
            if (!_logger.IsEnabled(LogLevel.Debug))
                return;
            _logger.Debug(message, args);
        }

        public void EntityWarn(string entity, string message, params object[] args) {
            if (_level != "none") {
                AppendLevel("warn ", entity);
                _builder.AppendFormat(message, args);
                _builder.AppendLine();
            }

            if (!_logger.IsEnabled(LogLevel.Warning))
                return;
            _logger.Warning(message, args);
        }

        public void EntityError(string entity, string message, params object[] args) {
            if (_level != "none") {
                AppendLevel("error", entity);
                _builder.AppendFormat(message, args);
                _builder.AppendLine();
            }
            if (!_logger.IsEnabled(LogLevel.Error))
                return;
            _logger.Error(message, args);
        }

        public void EntityError(string entity, Exception exception, string message, params object[] args) {
            if (_level != "none") {
                AppendLevel("error", entity);
                _builder.AppendFormat(message, args);
                _builder.Append("<br/>");
                _builder.Append(exception.Message);
                _builder.Append(exception.StackTrace.Replace(Environment.NewLine, "<br/>"));
                _builder.AppendLine();
            }

            if (!_logger.IsEnabled(LogLevel.Error))
                return;
            _logger.Error(exception, message, args);
        }

        public void Start() {
            if (!_logger.IsEnabled(LogLevel.Information))
                return;
            Info("Injecting memory logger");
            Info("Orchard version: {0}", _orchardVersion);
            Info("Transformalize.Orchard version: {0}", _moduleVersion);
            _logger.Information("TFL has started logging.");
        }

        public void Stop() {
            if (_logger.IsEnabled(LogLevel.Information)) {
                _logger.Information("TFL has stopped logging.");
            }
        }

        private void AppendLevel(string level, string entity) {
            _builder.Append(DateTime.Now);
            _builder.Append(DELIMITER);
            _builder.Append(level);
            _builder.Append(DELIMITER);
            _builder.Append(_process);
            _builder.Append(DELIMITER);
            _builder.Append(entity);
            _builder.Append(DELIMITER);
        }

    }
}