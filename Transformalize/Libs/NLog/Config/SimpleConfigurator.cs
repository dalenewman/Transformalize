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

using System.Diagnostics.CodeAnalysis;
using Transformalize.Libs.NLog.Targets;

namespace Transformalize.Libs.NLog.Config
{
    /// <summary>
    ///     Provides simple programmatic configuration API used for trivial logging cases.
    /// </summary>
    public static class SimpleConfigurator
    {
#if !NET_CF
        /// <summary>
        ///     Configures NLog for console logging so that all messages above and including
        ///     the <see cref="LogLevel.Info" /> level are output to the console.
        /// </summary>
        public static void ConfigureForConsoleLogging()
        {
            ConfigureForConsoleLogging(LogLevel.Info);
        }

        /// <summary>
        ///     Configures NLog for console logging so that all messages above and including
        ///     the specified level are output to the console.
        /// </summary>
        /// <param name="minLevel">The minimal logging level.</param>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Target is disposed elsewhere.")]
        public static void ConfigureForConsoleLogging(LogLevel minLevel)
        {
            var consoleTarget = new ConsoleTarget();

            var config = new LoggingConfiguration();
            var rule = new LoggingRule("*", minLevel, consoleTarget);
            config.LoggingRules.Add(rule);
            LogManager.Configuration = config;
        }
#endif

        /// <summary>
        ///     Configures NLog for to log to the specified target so that all messages
        ///     above and including the <see cref="LogLevel.Info" /> level are output.
        /// </summary>
        /// <param name="target">The target to log all messages to.</param>
        public static void ConfigureForTargetLogging(Target target)
        {
            ConfigureForTargetLogging(target, LogLevel.Info);
        }

        /// <summary>
        ///     Configures NLog for to log to the specified target so that all messages
        ///     above and including the specified level are output.
        /// </summary>
        /// <param name="target">The target to log all messages to.</param>
        /// <param name="minLevel">The minimal logging level.</param>
        public static void ConfigureForTargetLogging(Target target, LogLevel minLevel)
        {
            var config = new LoggingConfiguration();
            var rule = new LoggingRule("*", minLevel, target);
            config.LoggingRules.Add(rule);
            LogManager.Configuration = config;
        }

#if !SILVERLIGHT2 && !SILVERLIGHT3 && !WINDOWS_PHONE
        /// <summary>
        ///     Configures NLog for file logging so that all messages above and including
        ///     the <see cref="LogLevel.Info" /> level are written to the specified file.
        /// </summary>
        /// <param name="fileName">Log file name.</param>
        public static void ConfigureForFileLogging(string fileName)
        {
            ConfigureForFileLogging(fileName, LogLevel.Info);
        }

        /// <summary>
        ///     Configures NLog for file logging so that all messages above and including
        ///     the specified level are written to the specified file.
        /// </summary>
        /// <param name="fileName">Log file name.</param>
        /// <param name="minLevel">The minimal logging level.</param>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Target is disposed elsewhere.")]
        public static void ConfigureForFileLogging(string fileName, LogLevel minLevel)
        {
            var target = new FileTarget();
            target.FileName = fileName;
            ConfigureForTargetLogging(target, minLevel);
        }
#endif
    }
}