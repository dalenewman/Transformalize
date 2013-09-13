#region License

// /*
// Transformalize - Replicate, Transform, and Denormalize Your Data...
// Copyright (C) 2013 Dale Newman
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
// */

#endregion

using System.Runtime.InteropServices;

#if !NET_CF && !SILVERLIGHT

namespace Transformalize.Libs.NLog.ComInterop
{
    /// <summary>
    ///     NLog COM Interop logger implementation.
    /// </summary>
    [ComVisible(true)]
    [ProgId("NLog.Logger")]
    [Guid("181f39a8-41a8-4e35-91b6-5f8d96f5e61c")]
    [ClassInterface(ClassInterfaceType.None)]
    public class ComLogger : IComLogger
    {
        private static readonly Logger DefaultLogger = LogManager.CreateNullLogger();

        private Logger logger = DefaultLogger;
        private string loggerName = string.Empty;

        /// <summary>
        ///     Gets a value indicating whether the Trace level is enabled.
        /// </summary>
        /// <value></value>
        public bool IsTraceEnabled
        {
            get { return logger.IsTraceEnabled; }
        }

        /// <summary>
        ///     Gets a value indicating whether the Debug level is enabled.
        /// </summary>
        /// <value></value>
        public bool IsDebugEnabled
        {
            get { return logger.IsDebugEnabled; }
        }

        /// <summary>
        ///     Gets a value indicating whether the Info level is enabled.
        /// </summary>
        /// <value></value>
        public bool IsInfoEnabled
        {
            get { return logger.IsInfoEnabled; }
        }

        /// <summary>
        ///     Gets a value indicating whether the Warn level is enabled.
        /// </summary>
        /// <value></value>
        public bool IsWarnEnabled
        {
            get { return logger.IsWarnEnabled; }
        }

        /// <summary>
        ///     Gets a value indicating whether the Error level is enabled.
        /// </summary>
        /// <value></value>
        public bool IsErrorEnabled
        {
            get { return logger.IsErrorEnabled; }
        }

        /// <summary>
        ///     Gets a value indicating whether the Fatal level is enabled.
        /// </summary>
        /// <value></value>
        public bool IsFatalEnabled
        {
            get { return logger.IsFatalEnabled; }
        }

        /// <summary>
        ///     Gets or sets the logger name.
        /// </summary>
        /// <value></value>
        public string LoggerName
        {
            get { return loggerName; }

            set
            {
                loggerName = value;
                logger = LogManager.GetLogger(value);
            }
        }

        /// <summary>
        ///     Writes the diagnostic message at the specified level.
        /// </summary>
        /// <param name="level">The log level.</param>
        /// <param name="message">
        ///     A <see langword="string" /> to be written.
        /// </param>
        public void Log(string level, string message)
        {
            logger.Log(LogLevel.FromString(level), message);
        }

        /// <summary>
        ///     Writes the diagnostic message at the Trace level.
        /// </summary>
        /// <param name="message">
        ///     A <see langword="string" /> to be written.
        /// </param>
        public void Trace(string message)
        {
            logger.Trace(message);
        }

        /// <summary>
        ///     Writes the diagnostic message at the Debug level.
        /// </summary>
        /// <param name="message">
        ///     A <see langword="string" /> to be written.
        /// </param>
        public void Debug(string message)
        {
            logger.Debug(message);
        }

        /// <summary>
        ///     Writes the diagnostic message at the Info level.
        /// </summary>
        /// <param name="message">
        ///     A <see langword="string" /> to be written.
        /// </param>
        public void Info(string message)
        {
            logger.Info(message);
        }

        /// <summary>
        ///     Writes the diagnostic message at the Warn level.
        /// </summary>
        /// <param name="message">
        ///     A <see langword="string" /> to be written.
        /// </param>
        public void Warn(string message)
        {
            logger.Warn(message);
        }

        /// <summary>
        ///     Writes the diagnostic message at the Error level.
        /// </summary>
        /// <param name="message">
        ///     A <see langword="string" /> to be written.
        /// </param>
        public void Error(string message)
        {
            logger.Error(message);
        }

        /// <summary>
        ///     Writes the diagnostic message at the Fatal level.
        /// </summary>
        /// <param name="message">
        ///     A <see langword="string" /> to be written.
        /// </param>
        public void Fatal(string message)
        {
            logger.Fatal(message);
        }

        /// <summary>
        ///     Checks if the specified log level is enabled.
        /// </summary>
        /// <param name="level">The log level.</param>
        /// <returns>
        ///     A value indicating whether the specified log level is enabled.
        /// </returns>
        public bool IsEnabled(string level)
        {
            return logger.IsEnabled(LogLevel.FromString(level));
        }
    }
}

#endif