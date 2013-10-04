#region License
// /*
// See license included in this library folder.
// */
#endregion

using System.Runtime.InteropServices;
using Transformalize.Libs.NLog.Common;
using Transformalize.Libs.NLog.Config;

#if !NET_CF && !SILVERLIGHT

namespace Transformalize.Libs.NLog.ComInterop
{
    /// <summary>
    ///     NLog COM Interop LogManager implementation.
    /// </summary>
    [ComVisible(true)]
    [ProgId("NLog.LogManager")]
    [Guid("9a7e8d84-72e4-478a-9a05-23c7ef0cfca8")]
    [ClassInterface(ClassInterfaceType.None)]
    public class ComLogManager : IComLogManager
    {
        /// <summary>
        ///     Gets or sets a value indicating whether to log internal messages to the console.
        /// </summary>
        /// <value>
        ///     A value of <c>true</c> if internal messages should be logged to the console; otherwise, <c>false</c>.
        /// </value>
        public bool InternalLogToConsole
        {
            get { return InternalLogger.LogToConsole; }
            set { InternalLogger.LogToConsole = value; }
        }

        /// <summary>
        ///     Gets or sets the name of the internal log level.
        /// </summary>
        /// <value></value>
        public string InternalLogLevel
        {
            get { return InternalLogger.LogLevel.ToString(); }
            set { InternalLogger.LogLevel = LogLevel.FromString(value); }
        }

        /// <summary>
        ///     Gets or sets the name of the internal log file.
        /// </summary>
        /// <value></value>
        public string InternalLogFile
        {
            get { return InternalLogger.LogFile; }
            set { InternalLogger.LogFile = value; }
        }

        /// <summary>
        ///     Creates the specified logger object and assigns a LoggerName to it.
        /// </summary>
        /// <param name="loggerName">The name of the logger.</param>
        /// <returns>The new logger instance.</returns>
        public IComLogger GetLogger(string loggerName)
        {
            IComLogger logger = new ComLogger
                                    {
                                        LoggerName = loggerName
                                    };

            return logger;
        }

        /// <summary>
        ///     Loads NLog configuration from the specified file.
        /// </summary>
        /// <param name="fileName">The name of the file to load NLog configuration from.</param>
        public void LoadConfigFromFile(string fileName)
        {
            LogManager.Configuration = new XmlLoggingConfiguration(fileName);
        }
    }
}

#endif