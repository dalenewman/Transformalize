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