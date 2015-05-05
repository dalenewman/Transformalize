using System;
using Transformalize.Logging;

namespace Transformalize.Test {

    public class TestLogger : ILogger {

        private const string LAYOUT = "{0:hh:mm:ss} | {1} | {2} | {3}";
        private const string ENTITY_LAYOUT = "{0:hh:mm:ss} | {1} | {2} | {3} | {4}";
        private readonly Func<string, string, string, object[], string> _formatter = (message, level, process, args) => string.Format(string.Format(LAYOUT, DateTime.Now, level, process, message), args);
        private readonly Func<string, string, string, string, object[], string> _entityFormatter = (message, level, process, entity, args) => string.Format(string.Format(ENTITY_LAYOUT, DateTime.Now, level, process, entity, message), args);

        public string Name { get; set; }

        public TestLogger() {
            Name = "Test";
        }

        public void Info(string message, params object[] args) {
            Console.WriteLine(_formatter(message, "info ", Name, args));
        }

        public void Debug(string message, params object[] args) {
            Console.WriteLine(_formatter(message, "debug", Name, args));
        }

        public void Warn(string message, params object[] args) {
            Console.WriteLine(_formatter(message, "warn ", Name, args));
        }

        public void Error(string message, params object[] args) {
            Console.Error.WriteLine(_formatter(message, "ERROR", Name, args));
        }

        public void Error(Exception exception, string message, params object[] args) {
            Console.Error.WriteLine(_formatter(Name, message, "ERROR", args));
            Console.Error.WriteLine(exception.StackTrace);
        }

        public void EntityInfo(string entity, string message, params object[] args) {
            Console.WriteLine(string.IsNullOrEmpty(entity)
                ? _formatter(message, "info ", Name, args)
                : _entityFormatter(message, "info ", Name, entity, args));
        }

        public void EntityDebug(string entity, string message, params object[] args) {
            Console.WriteLine(string.IsNullOrEmpty(entity)
                ? _formatter(message, "debug", Name, args)
                : _entityFormatter(message, "debug", Name, entity, args));
        }

        public void EntityWarn(string entity, string message, params object[] args) {
            Console.WriteLine(string.IsNullOrEmpty(entity)
                ? _formatter(message, "warn ", Name, args)
                : _entityFormatter(message, "warn ", Name, entity, args));
        }

        public void EntityError(string entity, string message, params object[] args) {
            Console.Error.WriteLine(_entityFormatter(message, "ERROR", Name, entity, args));
        }

        public void EntityError(string entity, Exception exception, string message, params object[] args) {
            Console.Error.WriteLine(_entityFormatter(message, "ERROR", Name, entity, args));
            Console.Error.WriteLine(exception.StackTrace);
        }

        public void Start() {
        }

        public void Stop() {
        }
    }
}
