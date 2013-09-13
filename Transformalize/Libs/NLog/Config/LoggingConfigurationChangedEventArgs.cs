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

namespace Transformalize.Libs.NLog.Config
{
    /// <summary>
    ///     Arguments for <see cref="LogFactory.ConfigurationChanged" /> events.
    /// </summary>
    public class LoggingConfigurationChangedEventArgs : EventArgs
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="LoggingConfigurationChangedEventArgs" /> class.
        /// </summary>
        /// <param name="oldConfiguration">The old configuration.</param>
        /// <param name="newConfiguration">The new configuration.</param>
        internal LoggingConfigurationChangedEventArgs(LoggingConfiguration oldConfiguration, LoggingConfiguration newConfiguration)
        {
            OldConfiguration = oldConfiguration;
            NewConfiguration = newConfiguration;
        }

        /// <summary>
        ///     Gets the old configuration.
        /// </summary>
        /// <value>The old configuration.</value>
        public LoggingConfiguration OldConfiguration { get; private set; }

        /// <summary>
        ///     Gets the new configuration.
        /// </summary>
        /// <value>The new configuration.</value>
        public LoggingConfiguration NewConfiguration { get; private set; }
    }
}