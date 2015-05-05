using System;
using Transformalize.Configuration;
using Transformalize.Logging;

namespace Transformalize.Test {

    public class TestLogger : ILogger {

        private const string LAYOUT = "{0:hh:mm:ss} | {1} | {2} | {3} | {4}";
        private readonly Func<string, string, string, string, object[], string> _formatter = (message, level, process, entity, args) => string.Format(string.Format(LAYOUT, DateTime.Now, level, process, entity, message), args);

        public string Name { get; set; }

        public TestLogger() {
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
            Console.WriteLine(_formatter(message, "info ", Name, entity, args));
        }

        public void EntityDebug(string entity, string message, params object[] args) {
            Console.WriteLine(_formatter(message, "debug", Name, entity, args));
        }

        public void EntityWarn(string entity, string message, params object[] args) {
            Console.WriteLine(_formatter(message, "warn ", Name, entity, args));
        }

        public void EntityError(string entity, string message, params object[] args) {
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
    }
}
