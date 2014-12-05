#region License
// /*
// See license included in this library folder.
// */
#endregion

using System;
using System.Collections.Generic;
using System.Globalization;
using Transformalize.Libs.Rhino.Etl.Exceptions;
using Transformalize.Logging;
using Transformalize.Main;

namespace Transformalize.Libs.Rhino.Etl {
    /// <summary>
    ///     A base class that expose easily logging events
    /// </summary>
    public class WithLoggingMixin {
        private readonly List<Exception> _errors = new List<Exception>();

        /// <summary>
        ///     Initializes a new instance of the <see cref="WithLoggingMixin" /> class.
        /// </summary>
        protected WithLoggingMixin() {
        }

        /// <summary>
        ///     Gets all the errors
        /// </summary>
        /// <value>The errors.</value>
        public Exception[] Errors {
            get { return _errors.ToArray(); }
        }

        protected void Error(Exception exception, string format, params object[] args) {
            var message = string.Format(CultureInfo.InvariantCulture, format, args);
            var errorMessage = exception != null ? string.Format("{0}: {1}", message, exception.Message) : message;
            _errors.Add(new TransformalizeException(string.Empty, string.Empty, exception, errorMessage));
        }

        protected void Error(string format, params object[] args) {
            _errors.Add(new TransformalizeException(string.Empty, string.Empty, format, args));
        }

        protected void Error(string message) {
            _errors.Add(new TransformalizeException(string.Empty, string.Empty, message));
        }

        protected void Warn(string format, params object[] args) {
            if (TflLogger.IsWarnEnabled) {
                TflLogger.Warn(string.Empty, string.Empty, format, args);
            }
        }

        /// <summary>
        ///     Logs a debug message
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The args.</param>
        protected void Debug(string format, params object[] args) {
            if (TflLogger.IsDebugEnabled) {
                TflLogger.Debug(string.Empty, string.Empty, format, args);
            }
        }

        /// <summary>
        ///     Logs an information message
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The args.</param>
        protected void Info(string format, params object[] args) {
            if (TflLogger.IsInfoEnabled) {
                TflLogger.Info(string.Empty, string.Empty, format, args);
            }
        }

    }
}