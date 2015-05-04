using System;
using System.Collections.Generic;
using Transformalize.Logging;

namespace Transformalize.Libs.Rhino.Etl {

    public class WithLoggingMixin {

        public ILogger Logger { get; set; }

        public List<Exception> Errors { get; private set; }
        public WithLoggingMixin() {
            Logger = new NullLogger();
            Errors = new List<Exception>();
        }

        public void Debug(string message, params object[] args) {
            Logger.Debug(message, args);
        }

        public void Error(Exception e, string message, params object[] args) {
            Errors.Add(e);
            Logger.Error(e, message, args);
        }

        public void Warn(string message, params object[] args) {
            Logger.Warn(message, args);
        }
    }
}