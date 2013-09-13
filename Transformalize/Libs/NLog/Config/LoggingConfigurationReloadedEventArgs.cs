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