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
using System.Configuration;
using System.Xml;
using Transformalize.Libs.NLog.Common;
using Transformalize.Libs.NLog.Internal;

#if !NET_CF && !SILVERLIGHT

namespace Transformalize.Libs.NLog.Config
{
    /// <summary>
    ///     NLog configuration section handler class for configuring NLog from App.config.
    /// </summary>
    public sealed class ConfigSectionHandler : IConfigurationSectionHandler
    {
        /// <summary>
        ///     Creates a configuration section handler.
        /// </summary>
        /// <param name="parent">Parent object.</param>
        /// <param name="configContext">Configuration context object.</param>
        /// <param name="section">Section XML node.</param>
        /// <returns>The created section handler object.</returns>
        object IConfigurationSectionHandler.Create(object parent, object configContext, XmlNode section)
        {
            try
            {
                var configFileName = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;

                return new XmlLoggingConfiguration((XmlElement) section, configFileName);
            }
            catch (Exception exception)
            {
                if (exception.MustBeRethrown())
                {
                    throw;
                }

                InternalLogger.Error("ConfigSectionHandler error: {0}", exception);
                throw;
            }
        }
    }
}

#endif