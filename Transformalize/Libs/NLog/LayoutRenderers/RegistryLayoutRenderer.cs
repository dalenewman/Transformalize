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
using System.Globalization;
using System.Text;
using Microsoft.Win32;
using Transformalize.Libs.NLog.Config;
using Transformalize.Libs.NLog.Internal;

#if !SILVERLIGHT

namespace Transformalize.Libs.NLog.LayoutRenderers
{
    /// <summary>
    ///     A value from the Registry.
    /// </summary>
    [LayoutRenderer("registry")]
    public class RegistryLayoutRenderer : LayoutRenderer
    {
        private string key;
        private RegistryKey rootKey = Registry.LocalMachine;
        private string subKey;

        /// <summary>
        ///     Gets or sets the registry value name.
        /// </summary>
        /// <docgen category='Registry Options' order='10' />
        public string Value { get; set; }

        /// <summary>
        ///     Gets or sets the value to be output when the specified registry key or value is not found.
        /// </summary>
        /// <docgen category='Registry Options' order='10' />
        public string DefaultValue { get; set; }

        /// <summary>
        ///     Gets or sets the registry key.
        /// </summary>
        /// <remarks>
        ///     Must have one of the forms:
        ///     <ul>
        ///         <li>HKLM\Key\Full\Name</li>
        ///         <li>HKEY_LOCAL_MACHINE\Key\Full\Name</li>
        ///         <li>HKCU\Key\Full\Name</li>
        ///         <li>HKEY_CURRENT_USER\Key\Full\Name</li>
        ///     </ul>
        /// </remarks>
        /// <docgen category='Registry Options' order='10' />
        [RequiredParameter]
        public string Key
        {
            get { return key; }

            set
            {
                key = value;
                var pos = key.IndexOfAny(new[] {'\\', '/'});

                if (pos >= 0)
                {
                    var root = key.Substring(0, pos);
                    switch (root.ToUpper(CultureInfo.InvariantCulture))
                    {
                        case "HKEY_LOCAL_MACHINE":
                        case "HKLM":
                            rootKey = Registry.LocalMachine;
                            break;

                        case "HKEY_CURRENT_USER":
                        case "HKCU":
                            rootKey = Registry.CurrentUser;
                            break;

                        default:
                            throw new ArgumentException("Key name is invalid. Root hive not recognized.");
                    }

                    subKey = key.Substring(pos + 1).Replace('/', '\\');
                }
                else
                {
                    throw new ArgumentException("Key name is invalid");
                }
            }
        }

        /// <summary>
        ///     Reads the specified registry key and value and appends it to
        ///     the passed <see cref="StringBuilder" />.
        /// </summary>
        /// <param name="builder">
        ///     The <see cref="StringBuilder" /> to append the rendered data to.
        /// </param>
        /// <param name="logEvent">Logging event. Ignored.</param>
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            string value;

            try
            {
                using (var registryKey = rootKey.OpenSubKey(subKey))
                {
                    value = Convert.ToString(registryKey.GetValue(Value, DefaultValue), CultureInfo.InvariantCulture);
                }
            }
            catch (Exception ex)
            {
                if (ex.MustBeRethrown())
                {
                    throw;
                }

                value = DefaultValue;
            }

            builder.Append(value);
        }
    }
}

#endif