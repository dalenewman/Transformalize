using System;
using Transformalize.Configuration;
using Transformalize.Extensions;
using Transformalize.Logging;

namespace Transformalize.Test {

    public class TestLogger : ILogger {
        private readonly string _level;

        private const string LAYOUT = "{0:hh:mm:ss} | {1} | {2} | {3} | {4}";
        private readonly Func<string, string, string, string, object[], string> _formatter = (message, level, process, entity, args) => string.Format(string.Format(LAYOUT, DateTime.Now, level, process, entity, message), args);
        private readonly bool _infoEnabled;
        private readonly bool _debugEnabled;
        private readonly bool _warnEnabled;
        private readonly bool _errorEnabled;

        public string Name { get; set; }

        public TestLogger(string level = "info")
        {
            _level = level.ToLower().Left(4);
            var levels = GetLevels(_level);
            _debugEnabled = levels[0];
            _infoEnabled = levels[1];
            _warnEnabled = levels[2];
            _errorEnabled = levels[3];
            Name = "Test";
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
            if(_infoEnabled)
                Console.WriteLine(_formatter(message, "info ", Name, entity, args));
        }

        public void EntityDebug(string entity, string message, params object[] args) {
            if(_debugEnabled)
                Console.WriteLine(_formatter(message, "debug", Name, entity, args));
        }

        public void EntityWarn(string entity, string message, params object[] args) {
            if(_warnEnabled)
                Console.WriteLine(_formatter(message, "warn ", Name, entity, args));
        }

        public void EntityError(string entity, string message, params object[] args) {
            if(_errorEnabled)
                Console.Error.WriteLine(_formatter(message, "ERROR", Name, entity, args));
        }

        public void EntityError(string entity, Exception exception, string message, params object[] args) {
            Console.Error.WriteLine(_formatter(message, "ERROR", Name, entity, args));
            Console.Error.WriteLine(exception.StackTrace);
        }

        public void Start(TflProcess process) {
            Name = process.Name;
            Info("Start logging {0}!", process.Name);
        }

        public void Stop() {
            Info("Stop logging {0}!", Name);
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
    }
}
