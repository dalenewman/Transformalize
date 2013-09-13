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
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Transformalize.Libs.NLog.Common;
using Transformalize.Libs.NLog.Config;
using Transformalize.Libs.NLog.Internal;

namespace Transformalize.Libs.NLog
{
    /// <summary>
    ///     Creates and manages instances of <see cref="T:Transformalize.Libs.NLog.Logger" /> objects.
    /// </summary>
    public sealed class LogManager
    {
        private static readonly LogFactory globalFactory = new LogFactory();

#if !NET_CF && !SILVERLIGHT && !MONO
        /// <summary>
        ///     Initializes static members of the LogManager class.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = "Significant logic in .cctor()")]
        static LogManager()
        {
            try
            {
                SetupTerminationEvents();
            }
            catch (Exception exception)
            {
                if (exception.MustBeRethrown())
                {
                    throw;
                }

                InternalLogger.Warn("Error setting up termiation events: {0}", exception);
            }
        }
#endif

        /// <summary>
        ///     Prevents a default instance of the LogManager class from being created.
        /// </summary>
        private LogManager()
        {
        }

        /// <summary>
        ///     Occurs when logging <see cref="Configuration" /> changes.
        /// </summary>
        public static event EventHandler<LoggingConfigurationChangedEventArgs> ConfigurationChanged
        {
            add { globalFactory.ConfigurationChanged += value; }
            remove { globalFactory.ConfigurationChanged -= value; }
        }

#if !NET_CF && !SILVERLIGHT
        /// <summary>
        ///     Occurs when logging <see cref="Configuration" /> gets reloaded.
        /// </summary>
        public static event EventHandler<LoggingConfigurationReloadedEventArgs> ConfigurationReloaded
        {
            add { globalFactory.ConfigurationReloaded += value; }
            remove { globalFactory.ConfigurationReloaded -= value; }
        }
#endif

        /// <summary>
        ///     Gets or sets a value indicating whether NLog should throw exceptions.
        ///     By default exceptions are not thrown under any circumstances.
        /// </summary>
        public static bool ThrowExceptions
        {
            get { return globalFactory.ThrowExceptions; }
            set { globalFactory.ThrowExceptions = value; }
        }

        /// <summary>
        ///     Gets or sets the current logging configuration.
        /// </summary>
        public static LoggingConfiguration Configuration
        {
            get { return globalFactory.Configuration; }
            set { globalFactory.Configuration = value; }
        }

        /// <summary>
        ///     Gets or sets the global log threshold. Log events below this threshold are not logged.
        /// </summary>
        public static LogLevel GlobalThreshold
        {
            get { return globalFactory.GlobalThreshold; }
            set { globalFactory.GlobalThreshold = value; }
        }

#if !NET_CF
        /// <summary>
        ///     Gets the logger named after the currently-being-initialized class.
        /// </summary>
        /// <returns>The logger.</returns>
        /// <remarks>
        ///     This is a slow-running method.
        ///     Make sure you're not doing this in a loop.
        /// </remarks>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static Logger GetCurrentClassLogger()
        {
#if SILVERLIGHT
            StackFrame frame = new StackTrace().GetFrame(1);
#else
            var frame = new StackFrame(1, false);
#endif

            return globalFactory.GetLogger(frame.GetMethod().DeclaringType.FullName);
        }

        /// <summary>
        ///     Gets the logger named after the currently-being-initialized class.
        /// </summary>
        /// <param name="loggerType">
        ///     The logger class. The class must inherit from <see cref="Logger" />.
        /// </param>
        /// <returns>The logger.</returns>
        /// <remarks>
        ///     This is a slow-running method.
        ///     Make sure you're not doing this in a loop.
        /// </remarks>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static Logger GetCurrentClassLogger(Type loggerType)
        {
#if SILVERLIGHT
            StackFrame frame = new StackTrace().GetFrame(1);
#else
            var frame = new StackFrame(1, false);
#endif
            return globalFactory.GetLogger(frame.GetMethod().DeclaringType.FullName, loggerType);
        }
#endif

        /// <summary>
        ///     Creates a logger that discards all log messages.
        /// </summary>
        /// <returns>Null logger which discards all log messages.</returns>
        public static Logger CreateNullLogger()
        {
            return globalFactory.CreateNullLogger();
        }

        /// <summary>
        ///     Gets the specified named logger.
        /// </summary>
        /// <param name="name">Name of the logger.</param>
        /// <returns>
        ///     The logger reference. Multiple calls to <c>GetLogger</c> with the same argument aren't guaranteed to return the same logger reference.
        /// </returns>
        public static Logger GetLogger(string name)
        {
            return globalFactory.GetLogger(name);
        }

        /// <summary>
        ///     Gets the specified named logger.
        /// </summary>
        /// <param name="name">Name of the logger.</param>
        /// <param name="loggerType">
        ///     The logger class. The class must inherit from <see cref="Logger" />.
        /// </param>
        /// <returns>
        ///     The logger reference. Multiple calls to <c>GetLogger</c> with the same argument aren't guaranteed to return the same logger reference.
        /// </returns>
        public static Logger GetLogger(string name, Type loggerType)
        {
            return globalFactory.GetLogger(name, loggerType);
        }

        /// <summary>
        ///     Loops through all loggers previously returned by GetLogger.
        ///     and recalculates their target and filter list. Useful after modifying the configuration programmatically
        ///     to ensure that all loggers have been properly configured.
        /// </summary>
        public static void ReconfigExistingLoggers()
        {
            globalFactory.ReconfigExistingLoggers();
        }

#if !SILVERLIGHT
        /// <summary>
        ///     Flush any pending log messages (in case of asynchronous targets).
        /// </summary>
        public static void Flush()
        {
            globalFactory.Flush();
        }

        /// <summary>
        ///     Flush any pending log messages (in case of asynchronous targets).
        /// </summary>
        /// <param name="timeout">Maximum time to allow for the flush. Any messages after that time will be discarded.</param>
        public static void Flush(TimeSpan timeout)
        {
            globalFactory.Flush(timeout);
        }

        /// <summary>
        ///     Flush any pending log messages (in case of asynchronous targets).
        /// </summary>
        /// <param name="timeoutMilliseconds">Maximum time to allow for the flush. Any messages after that time will be discarded.</param>
        public static void Flush(int timeoutMilliseconds)
        {
            globalFactory.Flush(timeoutMilliseconds);
        }
#endif

        /// <summary>
        ///     Flush any pending log messages (in case of asynchronous targets).
        /// </summary>
        /// <param name="asyncContinuation">The asynchronous continuation.</param>
        public static void Flush(AsyncContinuation asyncContinuation)
        {
            globalFactory.Flush(asyncContinuation);
        }

        /// <summary>
        ///     Flush any pending log messages (in case of asynchronous targets).
        /// </summary>
        /// <param name="asyncContinuation">The asynchronous continuation.</param>
        /// <param name="timeout">Maximum time to allow for the flush. Any messages after that time will be discarded.</param>
        public static void Flush(AsyncContinuation asyncContinuation, TimeSpan timeout)
        {
            globalFactory.Flush(asyncContinuation, timeout);
        }

        /// <summary>
        ///     Flush any pending log messages (in case of asynchronous targets).
        /// </summary>
        /// <param name="asyncContinuation">The asynchronous continuation.</param>
        /// <param name="timeoutMilliseconds">Maximum time to allow for the flush. Any messages after that time will be discarded.</param>
        public static void Flush(AsyncContinuation asyncContinuation, int timeoutMilliseconds)
        {
            globalFactory.Flush(asyncContinuation, timeoutMilliseconds);
        }

        /// <summary>
        ///     Decreases the log enable counter and if it reaches -1
        ///     the logs are disabled.
        /// </summary>
        /// <remarks>
        ///     Logging is enabled if the number of <see cref="EnableLogging" /> calls is greater
        ///     than or equal to <see cref="DisableLogging" /> calls.
        /// </remarks>
        /// <returns>
        ///     An object that iplements IDisposable whose Dispose() method
        ///     reenables logging. To be used with C# <c>using ()</c> statement.
        /// </returns>
        public static IDisposable DisableLogging()
        {
            return globalFactory.DisableLogging();
        }

        /// <summary>Increases the log enable counter and if it reaches 0 the logs are disabled.</summary>
        /// <remarks>
        ///     Logging is enabled if the number of <see cref="EnableLogging" /> calls is greater
        ///     than or equal to <see cref="DisableLogging" /> calls.
        /// </remarks>
        public static void EnableLogging()
        {
            globalFactory.EnableLogging();
        }

        /// <summary>
        ///     Returns <see langword="true" /> if logging is currently enabled.
        /// </summary>
        /// <returns>
        ///     A value of <see langword="true" /> if logging is currently enabled,
        ///     <see langword="false" /> otherwise.
        /// </returns>
        /// <remarks>
        ///     Logging is enabled if the number of <see cref="EnableLogging" /> calls is greater
        ///     than or equal to <see cref="DisableLogging" /> calls.
        /// </remarks>
        public static bool IsLoggingEnabled()
        {
            return globalFactory.IsLoggingEnabled();
        }

#if !NET_CF && !SILVERLIGHT && !MONO
        private static void SetupTerminationEvents()
        {
            AppDomain.CurrentDomain.ProcessExit += TurnOffLogging;
            AppDomain.CurrentDomain.DomainUnload += TurnOffLogging;
        }

        private static void TurnOffLogging(object sender, EventArgs args)
        {
            // reset logging configuration to null
            // this causes old configuration (if any) to be closed.
            InternalLogger.Info("Shutting down logging...");
            Configuration = null;
            InternalLogger.Info("Logger has been shut down.");
        }
#endif
    }
}