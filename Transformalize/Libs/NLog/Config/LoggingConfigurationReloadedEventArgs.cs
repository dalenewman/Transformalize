#region License
// /*
// See license included in this library folder.
// */
#endregion

using System;

#if !SILVERLIGHT && !NET_CF

namespace Transformalize.Libs.NLog.Config
{
    /// <summary>
    ///     Arguments for <see cref="LogFactory.ConfigurationReloaded" />.
    /// </summary>
    public class LoggingConfigurationReloadedEventArgs : EventArgs
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="LoggingConfigurationReloadedEventArgs" /> class.
        /// </summary>
        /// <param name="succeeded">Whether configuration reload has succeeded.</param>
        /// <param name="exception">The exception during configuration reload.</param>
        internal LoggingConfigurationReloadedEventArgs(bool succeeded, Exception exception)
        {
            Succeeded = succeeded;
            Exception = exception;
        }

        /// <summary>
        ///     Gets a value indicating whether configuration reload has succeeded.
        /// </summary>
        /// <value>
        ///     A value of <c>true</c> if succeeded; otherwise, <c>false</c>.
        /// </value>
        public bool Succeeded { get; private set; }

        /// <summary>
        ///     Gets the exception which occurred during configuration reload.
        /// </summary>
        /// <value>The exception.</value>
        public Exception Exception { get; private set; }
    }
}

#endif