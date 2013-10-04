#region License
// /*
// See license included in this library folder.
// */
#endregion

using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

#if !NET_CF && !SILVERLIGHT

namespace Transformalize.Libs.NLog.ComInterop
{
    /// <summary>
    ///     NLog COM Interop LogManager interface.
    /// </summary>
    [Guid("7ee3af3b-ba37-45b6-8f5d-cc23bb46c698")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    [ComVisible(true)]
    public interface IComLogManager
    {
        /// <summary>
        ///     Loads NLog configuration from the specified file.
        /// </summary>
        /// <param name="fileName">The name of the file to load NLog configuration from.</param>
        [SuppressMessage("Microsoft.StyleCop.CSharp.OrderingRules", "SA1201:ElementsMustAppearInTheCorrectOrder",
            Justification = "Cannot change this, this is for backwards compatibility.")]
        void LoadConfigFromFile(string fileName);

        /// <summary>
        ///     Gets or sets a value indicating whether internal messages should be written to the console.
        /// </summary>
        bool InternalLogToConsole { get; set; }

        /// <summary>
        ///     Gets or sets the name of the internal log file.
        /// </summary>
        string InternalLogFile { get; set; }

        /// <summary>
        ///     Gets or sets the name of the internal log level.
        /// </summary>
        string InternalLogLevel { get; set; }

        /// <summary>
        ///     Creates the specified logger object and assigns a LoggerName to it.
        /// </summary>
        /// <param name="loggerName">Logger name.</param>
        /// <returns>The new logger instance.</returns>
        IComLogger GetLogger(string loggerName);
    }
}

#endif