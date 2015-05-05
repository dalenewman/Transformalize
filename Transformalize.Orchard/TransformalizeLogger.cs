using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Orchard.Logging;
using tflLogger = Transformalize.Logging.ILogger;
using Transformalize.Extensions;

namespace Transformalize.Orchard {

    public class TransformalizeLogger : tflLogger {

        private readonly string _level;
        private readonly ILogger _logger;
        private readonly string _orchardVersion;
        private readonly string _moduleVersion;
        private readonly ConcurrentQueue<LinkedList<string>> _log = new ConcurrentQueue<LinkedList<string>>();

        public string Name { get; set; }

        public TransformalizeLogger(string name, ILogger logger, string level, string orchardVersion, string moduleVersion) {
            _level = level.ToLower().Left(4);
            Name = name;
            _logger = logger;
            _orchardVersion = orchardVersion;
            _moduleVersion = moduleVersion;
        }

        public IEnumerable<LinkedList<string>> Dump() {
            LinkedList<string> item;
            while (_log.TryDequeue(out item)) {
                yield return item;
            }
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
                var line = AppendLevel("info ", entity);
                line.AddLast(string.Format(message, args));
                _log.Enqueue(line);
            }
            if (!_logger.IsEnabled(LogLevel.Information))
                return;

            _logger.Information(message, args);

        }

        public void EntityDebug(string entity, string message, params object[] args) {
            if (_level == "debu") {
                var line = AppendLevel("debug", entity);
                line.AddLast(string.Format(message, args));
                _log.Enqueue(line);
            }
            if (!_logger.IsEnabled(LogLevel.Debug))
                return;
            _logger.Debug(message, args);
        }

        public void EntityWarn(string entity, string message, params object[] args) {
            if (_level != "none") {
                var line = AppendLevel("warn ", entity);
                line.AddLast(string.Format(message, args));
                _log.Enqueue(line);
            }

            if (!_logger.IsEnabled(LogLevel.Warning))
                return;
            _logger.Warning(message, args);
        }

        public void EntityError(string entity, string message, params object[] args) {
            if (_level != "none") {
                var line = AppendLevel("error", entity);
                line.AddLast(string.Format(message, args));
                _log.Enqueue(line);
            }
            if (!_logger.IsEnabled(LogLevel.Error))
                return;
            _logger.Error(message, args);
        }

        public void EntityError(string entity, Exception exception, string message, params object[] args) {
            if (_level != "none") {
                var line1 = AppendLevel("error", entity);
                line1.AddLast(string.Format(message, args));
                _log.Enqueue(line1);

                var line2 = AppendLevel("error", entity);
                line2.AddLast(exception.Message);
                _log.Enqueue(line2);

                var line3 = AppendLevel("error", entity);
                line3.AddLast(exception.StackTrace);
                _log.Enqueue(line3);
            }

            if (!_logger.IsEnabled(LogLevel.Error))
                return;
            _logger.Error(exception, message, args);
        }

        public void Start() {
            if (!_logger.IsEnabled(LogLevel.Information))
                return;
            EntityInfo(".", "Orchard version: {0}", _orchardVersion);
            EntityInfo(".", "Transformalize.Orchard version: {0}", _moduleVersion);
            _logger.Information("TFL has started logging.");
        }

        public void Stop() {
            if (_logger.IsEnabled(LogLevel.Information)) {
                _logger.Information("TFL has stopped logging.");
            }
        }

        private LinkedList<string> AppendLevel(string level, string entity) {
            return new LinkedList<string>(new[] { DateTime.Now.ToString(), level, Name, entity });
        }

    }
}