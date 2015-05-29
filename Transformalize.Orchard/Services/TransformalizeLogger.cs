using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Orchard.Logging;
using Transformalize.Configuration;
using Transformalize.Extensions;
using ILogger = Transformalize.Logging.ILogger;

namespace Transformalize.Orchard.Services {

    /// <summary>
    /// This logger implementation wraps whatever Orchard CMS is using 
    /// and also records an in-memory log for reporting back to the TFL execute view.
    /// </summary>
    public class TransformalizeLogger : ILogger {

        private readonly string _level;
        private readonly global::Orchard.Logging.ILogger _orchardLogger;
        private readonly string _orchardVersion;
        private readonly string _moduleVersion;
        private readonly ConcurrentQueue<string[]> _log = new ConcurrentQueue<string[]>();
        private readonly bool _infoEnabled;
        private readonly bool _debugEnabled;
        private readonly bool _warnEnabled;
        private readonly bool _errorEnabled;

        private bool _reportedHost = false;

        public string DatetimeFormat { get; set; }
        public string Name { get; set; }

        public TransformalizeLogger(string name, string level, global::Orchard.Logging.ILogger orchardLogger, string orchardVersion, string moduleVersion) {
            _level = level.ToLower().Left(4);
            Name = name;
            _orchardLogger = orchardLogger;
            _orchardVersion = orchardVersion;
            _moduleVersion = moduleVersion;
            DatetimeFormat = "yyyy-MM-dd HH:mm:ss";

            var levels = GetLevels(_level);
            _debugEnabled = levels[0];
            _infoEnabled = levels[1];
            _warnEnabled = levels[2];
            _errorEnabled = levels[3];
        }

        private static bool[] GetLevels(string level) {
            switch (level) {
                case "debu":
                    return new[] { true, true, true, true };
                case "info":
                    return new[] { false, true, true, true };
                case "warn":
                    return new[] { false, false, true, true };
                case "erro":
                    return new[] { false, false, false, true };
                case "none":
                    return new[] { false, false, false, false };
                default:
                    goto case "info";
            }
        }

        public IEnumerable<string[]> Dump() {
            string[] item;
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
            if (_infoEnabled) {
                _log.Enqueue(GetLine("info ", entity, string.Format(message, args)));
            }
            if (!_orchardLogger.IsEnabled(LogLevel.Information))
                return;

            _orchardLogger.Information(message, args);

        }

        public void EntityDebug(string entity, string message, params object[] args) {
            if (_debugEnabled) {
                _log.Enqueue(GetLine("debug", entity, string.Format(message, args)));
            }
            if (!_orchardLogger.IsEnabled(LogLevel.Debug))
                return;
            _orchardLogger.Debug(message, args);
        }

        public void EntityWarn(string entity, string message, params object[] args) {
            if (_warnEnabled) {
                _log.Enqueue(GetLine("warn", entity, string.Format(message, args)));
            }

            if (!_orchardLogger.IsEnabled(LogLevel.Warning))
                return;
            _orchardLogger.Warning(message, args);
        }

        public void EntityError(string entity, string message, params object[] args) {
            if (_errorEnabled) {
                _log.Enqueue(GetLine("error", entity, string.Format(message, args)));
            }
            if (!_orchardLogger.IsEnabled(LogLevel.Error))
                return;
            _orchardLogger.Error(message, args);
        }

        public void EntityError(string entity, Exception exception, string message, params object[] args) {
            if (_errorEnabled) {
                _log.Enqueue(GetLine("error", entity, string.Format(message, args)));
                _log.Enqueue(GetLine("error", entity, exception.Message));
                _log.Enqueue(GetLine("error", entity, exception.StackTrace));
            }

            if (!_orchardLogger.IsEnabled(LogLevel.Error))
                return;
            _orchardLogger.Error(exception, message, args);
        }

        public void Start(TflProcess process) {
            if (!_reportedHost) {
                EntityInfo(".", "Orchard version: {0}", _orchardVersion);
                EntityInfo(".", "Transformalize.Orchard version: {0}", _moduleVersion);
                EntityInfo(".", GetHost());
                _reportedHost = true;
            }
            if (_level != "none") {
                Info("{0} entit{1} in {2} mode.", process.Entities.Count, process.Entities.Count.Pluralize(), process.Mode == string.Empty ? "Default" : process.Mode);
                Info("Running {0} with a {1} pipeline.", process.Parallel ? "Parallel" : "Serial", GetPipelineDescription(process));
            }

            if (!_orchardLogger.IsEnabled(LogLevel.Information))
                return;
            _orchardLogger.Information("TFL started logging process {0}.", Name);
        }

        public void Stop() {
            if (_orchardLogger.IsEnabled(LogLevel.Information)) {
                _orchardLogger.Information("TFL stopped logging process {0}.", Name);
            }
        }

        private string[] GetLine(string level, string entity, string message) {
            return new[] { DateTime.Now.ToString(DatetimeFormat), level, Name, entity, message };
        }

        private static string GetHost() {
            var host = System.Net.Dns.GetHostName();
            var ip4 = System.Net.Dns.GetHostEntry(host).AddressList.Where(a => a.ToString().Length > 4 && a.ToString()[4] != ':').Select(a => a.ToString()).ToArray();
            return string.Format("Host is {0} {1}", host, string.Join(", ", ip4.Any() ? ip4 : new[] { string.Empty }));
        }

        private static string GetPipelineDescription(TflProcess process) {
            var pipeline = process.PipelineThreading;
            if (pipeline != "Default")
                return pipeline;

            if (process.Entities.All(e => e.PipelineThreading == "SingleThreaded")) {
                pipeline = "SingleThreaded";
            } else if (process.Entities.All(e => e.PipelineThreading == "MultiThreaded")) {
                pipeline = "MultiThreaded";
            } else {
                pipeline = "Mixed";
            }
            return pipeline;
        }



    }
}