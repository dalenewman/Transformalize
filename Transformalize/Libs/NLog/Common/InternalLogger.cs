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

using System;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Text;
using Transformalize.Libs.NLog.Internal;

namespace Transformalize.Libs.NLog.Common
{
    /// <summary>
    ///     NLog internal logger.
    /// </summary>
    public static class InternalLogger
    {
        private static readonly object lockObject = new object();

        /// <summary>
        ///     Initializes static members of the InternalLogger class.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = "Significant logic in .cctor()")]
        static InternalLogger()
        {
#if !NET_CF && !SILVERLIGHT
            LogToConsole = GetSetting("nlog.internalLogToConsole", "NLOG_INTERNAL_LOG_TO_CONSOLE", false);
            LogToConsoleError = GetSetting("nlog.internalLogToConsoleError", "NLOG_INTERNAL_LOG_TO_CONSOLE_ERROR", false);
            LogLevel = GetSetting("nlog.internalLogLevel", "NLOG_INTERNAL_LOG_LEVEL", LogLevel.Info);
            LogFile = GetSetting("nlog.internalLogFile", "NLOG_INTERNAL_LOG_FILE", string.Empty);
            Info("NLog internal logger initialized.");
#else
            LogLevel = LogLevel.Info;
#endif
            IncludeTimestamp = true;
        }

        /// <summary>
        ///     Gets or sets the internal log level.
        /// </summary>
        public static LogLevel LogLevel { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether internal messages should be written to the console output stream.
        /// </summary>
        public static bool LogToConsole { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether internal messages should be written to the console error stream.
        /// </summary>
        public static bool LogToConsoleError { get; set; }

        /// <summary>
        ///     Gets or sets the name of the internal log file.
        /// </summary>
        /// <remarks>
        ///     A value of <see langword="null" /> value disables internal logging to a file.
        /// </remarks>
        public static string LogFile { get; set; }

        /// <summary>
        ///     Gets or sets the text writer that will receive internal logs.
        /// </summary>
        public static TextWriter LogWriter { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether timestamp should be included in internal log output.
        /// </summary>
        public static bool IncludeTimestamp { get; set; }

        /// <summary>
        ///     Gets a value indicating whether internal log includes Trace messages.
        /// </summary>
        public static bool IsTraceEnabled
        {
            get { return LogLevel.Trace >= LogLevel; }
        }

        /// <summary>
        ///     Gets a value indicating whether internal log includes Debug messages.
        /// </summary>
        public static bool IsDebugEnabled
        {
            get { return LogLevel.Debug >= LogLevel; }
        }

        /// <summary>
        ///     Gets a value indicating whether internal log includes Info messages.
        /// </summary>
        public static bool IsInfoEnabled
        {
            get { return LogLevel.Info >= LogLevel; }
        }

        /// <summary>
        ///     Gets a value indicating whether internal log includes Warn messages.
        /// </summary>
        public static bool IsWarnEnabled
        {
            get { return LogLevel.Warn >= LogLevel; }
        }

        /// <summary>
        ///     Gets a value indicating whether internal log includes Error messages.
        /// </summary>
        public static bool IsErrorEnabled
        {
            get { return LogLevel.Error >= LogLevel; }
        }

        /// <summary>
        ///     Gets a value indicating whether internal log includes Fatal messages.
        /// </summary>
        public static bool IsFatalEnabled
        {
            get { return LogLevel.Fatal >= LogLevel; }
        }

        /// <summary>
        ///     Logs the specified message at the specified level.
        /// </summary>
        /// <param name="level">Log level.</param>
        /// <param name="message">Message which may include positional parameters.</param>
        /// <param name="args">Arguments to the message.</param>
        public static void Log(LogLevel level, string message, params object[] args)
        {
            Write(level, message, args);
        }

        /// <summary>
        ///     Logs the specified message at the specified level.
        /// </summary>
        /// <param name="level">Log level.</param>
        /// <param name="message">Log message.</param>
        public static void Log(LogLevel level, [Localizable(false)] string message)
        {
            Write(level, message, null);
        }

        /// <summary>
        ///     Logs the specified message at the Trace level.
        /// </summary>
        /// <param name="message">Message which may include positional parameters.</param>
        /// <param name="args">Arguments to the message.</param>
        public static void Trace([Localizable(false)] string message, params object[] args)
        {
            Write(LogLevel.Trace, message, args);
        }

        /// <summary>
        ///     Logs the specified message at the Trace level.
        /// </summary>
        /// <param name="message">Log message.</param>
        public static void Trace([Localizable(false)] string message)
        {
            Write(LogLevel.Trace, message, null);
        }

        /// <summary>
        ///     Logs the specified message at the Debug level.
        /// </summary>
        /// <param name="message">Message which may include positional parameters.</param>
        /// <param name="args">Arguments to the message.</param>
        public static void Debug([Localizable(false)] string message, params object[] args)
        {
            Write(LogLevel.Debug, message, args);
        }

        /// <summary>
        ///     Logs the specified message at the Debug level.
        /// </summary>
        /// <param name="message">Log message.</param>
        public static void Debug([Localizable(false)] string message)
        {
            Write(LogLevel.Debug, message, null);
        }

        /// <summary>
        ///     Logs the specified message at the Info level.
        /// </summary>
        /// <param name="message">Message which may include positional parameters.</param>
        /// <param name="args">Arguments to the message.</param>
        public static void Info([Localizable(false)] string message, params object[] args)
        {
            Write(LogLevel.Info, message, args);
        }

        /// <summary>
        ///     Logs the specified message at the Info level.
        /// </summary>
        /// <param name="message">Log message.</param>
        public static void Info([Localizable(false)] string message)
        {
            Write(LogLevel.Info, message, null);
        }

        /// <summary>
        ///     Logs the specified message at the Warn level.
        /// </summary>
        /// <param name="message">Message which may include positional parameters.</param>
        /// <param name="args">Arguments to the message.</param>
        public static void Warn([Localizable(false)] string message, params object[] args)
        {
            Write(LogLevel.Warn, message, args);
        }

        /// <summary>
        ///     Logs the specified message at the Warn level.
        /// </summary>
        /// <param name="message">Log message.</param>
        public static void Warn([Localizable(false)] string message)
        {
            Write(LogLevel.Warn, message, null);
        }

        /// <summary>
        ///     Logs the specified message at the Error level.
        /// </summary>
        /// <param name="message">Message which may include positional parameters.</param>
        /// <param name="args">Arguments to the message.</param>
        public static void Error([Localizable(false)] string message, params object[] args)
        {
            Write(LogLevel.Error, message, args);
        }

        /// <summary>
        ///     Logs the specified message at the Error level.
        /// </summary>
        /// <param name="message">Log message.</param>
        public static void Error([Localizable(false)] string message)
        {
            Write(LogLevel.Error, message, null);
        }

        /// <summary>
        ///     Logs the specified message at the Fatal level.
        /// </summary>
        /// <param name="message">Message which may include positional parameters.</param>
        /// <param name="args">Arguments to the message.</param>
        public static void Fatal([Localizable(false)] string message, params object[] args)
        {
            Write(LogLevel.Fatal, message, args);
        }

        /// <summary>
        ///     Logs the specified message at the Fatal level.
        /// </summary>
        /// <param name="message">Log message.</param>
        public static void Fatal([Localizable(false)] string message)
        {
            Write(LogLevel.Fatal, message, null);
        }

        private static void Write(LogLevel level, string message, object[] args)
        {
            if (level < LogLevel)
            {
                return;
            }

            if (string.IsNullOrEmpty(LogFile) && !LogToConsole && !LogToConsoleError && LogWriter == null)
            {
                return;
            }

            try
            {
                var formattedMessage = message;
                if (args != null)
                {
                    formattedMessage = string.Format(CultureInfo.InvariantCulture, message, args);
                }

                var builder = new StringBuilder(message.Length + 32);
                if (IncludeTimestamp)
                {
                    builder.Append(CurrentTimeGetter.Now.ToString("yyyy-MM-dd HH:mm:ss.ffff", CultureInfo.InvariantCulture));
                    builder.Append(" ");
                }

                builder.Append(level);
                builder.Append(" ");
                builder.Append(formattedMessage);
                var msg = builder.ToString();

                // log to file
                var logFile = LogFile;
                if (!string.IsNullOrEmpty(logFile))
                {
                    using (var textWriter = File.AppendText(logFile))
                    {
                        textWriter.WriteLine(msg);
                    }
                }

                // log to LogWriter
                var writer = LogWriter;
                if (writer != null)
                {
                    lock (lockObject)
                    {
                        writer.WriteLine(msg);
                    }
                }

                // log to console
                if (LogToConsole)
                {
                    Console.WriteLine(msg);
                }

                // log to console error
                if (LogToConsoleError)
                {
                    Console.Error.WriteLine(msg);
                }
            }
            catch (Exception exception)
            {
                if (exception.MustBeRethrown())
                {
                    throw;
                }

                // we have no place to log the message to so we ignore it
            }
        }

#if !NET_CF && !SILVERLIGHT
        private static string GetSettingString(string configName, string envName)
        {
            var settingValue = ConfigurationManager.AppSettings[configName];
            if (settingValue == null)
            {
                try
                {
                    settingValue = Environment.GetEnvironmentVariable(envName);
                }
                catch (Exception exception)
                {
                    if (exception.MustBeRethrown())
                    {
                        throw;
                    }
                }
            }

            return settingValue;
        }

        private static LogLevel GetSetting(string configName, string envName, LogLevel defaultValue)
        {
            var value = GetSettingString(configName, envName);
            if (value == null)
            {
                return defaultValue;
            }

            try
            {
                return LogLevel.FromString(value);
            }
            catch (Exception exception)
            {
                if (exception.MustBeRethrown())
                {
                    throw;
                }

                return defaultValue;
            }
        }

        private static T GetSetting<T>(string configName, string envName, T defaultValue)
        {
            var value = GetSettingString(configName, envName);
            if (value == null)
            {
                return defaultValue;
            }

            try
            {
                return (T) Convert.ChangeType(value, typeof (T), CultureInfo.InvariantCulture);
            }
            catch (Exception exception)
            {
                if (exception.MustBeRethrown())
                {
                    throw;
                }

                return defaultValue;
            }
        }
#endif
    }
}