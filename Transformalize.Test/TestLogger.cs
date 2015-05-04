using System;
using Transformalize.Logging;

namespace Transformalize.Test {
    public class TestLogger : ILogger {
        private const string LAYOUT = "{0:hh:mm:ss} | {1} | {2}";
        private const string ENTITY_LAYOUT = "{0:hh:mm:ss} | {1} | {2} | {3}";
        private readonly Func<string, string, object[], string> _formatter = (message, level, args) => string.Format(string.Format(LAYOUT, DateTime.Now, level, message), args);
        private readonly Func<string, string, string, object[], string> _entityFormatter = (message, level, entity, args) => string.Format(string.Format(ENTITY_LAYOUT, DateTime.Now, level, entity, message), args);

        public void Info(string message, params object[] args) {
            Console.WriteLine(_formatter(message, "info ", args));
        }

        public void Debug(string message, params object[] args) {
            Console.WriteLine(_formatter(message, "debug", args));
        }

        public void Warn(string message, params object[] args) {
            Console.WriteLine(_formatter(message, "warn ", args));
        }

        public void Error(string message, params object[] args) {
            Console.Error.WriteLine(_formatter(message, "ERROR", args));
        }

        public void Error(Exception exception, string message, params object[] args) {
            Console.Error.WriteLine(_formatter(message, "ERROR", args));
            Console.Error.WriteLine(exception.StackTrace);
        }

        public void EntityInfo(string entity, string message, params object[] args) {
            Console.WriteLine(string.IsNullOrEmpty(entity)
                ? _formatter(message, "info ", args)
                : _entityFormatter(message, "info ", entity, args));
        }

        public void EntityDebug(string entity, string message, params object[] args) {
            Console.WriteLine(string.IsNullOrEmpty(entity)
                ? _formatter(message, "debug", args)
                : _entityFormatter(message, "debug", entity, args));
        }

        public void EntityWarn(string entity, string message, params object[] args) {
            Console.WriteLine(string.IsNullOrEmpty(entity)
                ? _formatter(message, "warn ", args)
                : _entityFormatter(message, "warn ", entity, args));
        }

        public void EntityError(string entity, string message, params object[] args) {
            Console.Error.WriteLine(_entityFormatter(message, "ERROR", entity, args));
        }

        public void EntityError(string entity, Exception exception, string message, params object[] args) {
            Console.Error.WriteLine(_entityFormatter(message, "ERROR", entity, args));
            Console.Error.WriteLine(exception.StackTrace);
        }

        public void Start() {
        }

        public void Stop() {
        }
    }
}
