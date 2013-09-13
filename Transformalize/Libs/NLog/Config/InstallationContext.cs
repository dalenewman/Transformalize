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
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;

namespace Transformalize.Libs.NLog.Config
{
    /// <summary>
    ///     Provides context for install/uninstall operations.
    /// </summary>
    public sealed class InstallationContext : IDisposable
    {
#if !SILVERLIGHT && !NET_CF
        /// <summary>
        ///     Mapping between log levels and console output colors.
        /// </summary>
        private static readonly Dictionary<LogLevel, ConsoleColor> logLevel2ConsoleColor = new Dictionary<LogLevel, ConsoleColor>
                                                                                               {
                                                                                                   {LogLevel.Trace, ConsoleColor.DarkGray},
                                                                                                   {LogLevel.Debug, ConsoleColor.Gray},
                                                                                                   {LogLevel.Info, ConsoleColor.White},
                                                                                                   {LogLevel.Warn, ConsoleColor.Yellow},
                                                                                                   {LogLevel.Error, ConsoleColor.Red},
                                                                                                   {LogLevel.Fatal, ConsoleColor.DarkRed},
                                                                                               };
#endif

        /// <summary>
        ///     Initializes a new instance of the <see cref="InstallationContext" /> class.
        /// </summary>
        public InstallationContext()
            : this(TextWriter.Null)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="InstallationContext" /> class.
        /// </summary>
        /// <param name="logOutput">The log output.</param>
        public InstallationContext(TextWriter logOutput)
        {
            LogOutput = logOutput;
            Parameters = new Dictionary<string, string>();
            LogLevel = LogLevel.Info;
        }

        /// <summary>
        ///     Gets or sets the installation log level.
        /// </summary>
        public LogLevel LogLevel { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether to ignore failures during installation.
        /// </summary>
        public bool IgnoreFailures { get; set; }

        /// <summary>
        ///     Gets the installation parameters.
        /// </summary>
        public IDictionary<string, string> Parameters { get; private set; }

        /// <summary>
        ///     Gets or sets the log output.
        /// </summary>
        public TextWriter LogOutput { get; set; }

        /// <summary>
        ///     Logs the specified trace message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="arguments">The arguments.</param>
        public void Trace([Localizable(false)] string message, params object[] arguments)
        {
            Log(LogLevel.Trace, message, arguments);
        }

        /// <summary>
        ///     Logs the specified debug message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="arguments">The arguments.</param>
        public void Debug([Localizable(false)] string message, params object[] arguments)
        {
            Log(LogLevel.Debug, message, arguments);
        }

        /// <summary>
        ///     Logs the specified informational message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="arguments">The arguments.</param>
        public void Info([Localizable(false)] string message, params object[] arguments)
        {
            Log(LogLevel.Info, message, arguments);
        }

        /// <summary>
        ///     Logs the specified warning message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="arguments">The arguments.</param>
        public void Warning([Localizable(false)] string message, params object[] arguments)
        {
            Log(LogLevel.Warn, message, arguments);
        }

        /// <summary>
        ///     Logs the specified error message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="arguments">The arguments.</param>
        public void Error([Localizable(false)] string message, params object[] arguments)
        {
            Log(LogLevel.Error, message, arguments);
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (LogOutput != null)
            {
                LogOutput.Close();
                LogOutput = null;
            }
        }

        /// <summary>
        ///     Creates the log event which can be used to render layouts during installation/uninstallations.
        /// </summary>
        /// <returns>Log event info object.</returns>
        public LogEventInfo CreateLogEvent()
        {
            var eventInfo = LogEventInfo.CreateNullEvent();

            // set properties on the event
            foreach (var kvp in Parameters)
            {
                eventInfo.Properties.Add(kvp.Key, kvp.Value);
            }

            return eventInfo;
        }

        private void Log(LogLevel logLevel, [Localizable(false)] string message, object[] arguments)
        {
            if (logLevel >= LogLevel)
            {
                if (arguments != null && arguments.Length > 0)
                {
                    message = string.Format(CultureInfo.InvariantCulture, message, arguments);
                }

#if !SILVERLIGHT && !NET_CF
                var oldColor = Console.ForegroundColor;
                Console.ForegroundColor = logLevel2ConsoleColor[logLevel];

                try
                {
                    LogOutput.WriteLine(message);
                }
                finally
                {
                    Console.ForegroundColor = oldColor;
                }
#else
                this.LogOutput.WriteLine(message);
#endif
            }
        }
    }
}