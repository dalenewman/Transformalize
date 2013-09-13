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
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Text;
using Transformalize.Libs.NLog.Config;

#if !NET_CF && !MONO && !SILVERLIGHT

namespace Transformalize.Libs.NLog.LayoutRenderers
{
    /// <summary>
    ///     The information about the running process.
    /// </summary>
    [LayoutRenderer("processinfo")]
    public class ProcessInfoLayoutRenderer : LayoutRenderer
    {
        private Process process;

        private PropertyInfo propertyInfo;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ProcessInfoLayoutRenderer" /> class.
        /// </summary>
        public ProcessInfoLayoutRenderer()
        {
            Property = ProcessInfoProperty.Id;
        }

        /// <summary>
        ///     Gets or sets the property to retrieve.
        /// </summary>
        /// <docgen category='Rendering Options' order='10' />
        [DefaultValue("Id"), DefaultParameter]
        public ProcessInfoProperty Property { get; set; }

        /// <summary>
        ///     Initializes the layout renderer.
        /// </summary>
        protected override void InitializeLayoutRenderer()
        {
            base.InitializeLayoutRenderer();
            propertyInfo = typeof (Process).GetProperty(Property.ToString());
            if (propertyInfo == null)
            {
                throw new ArgumentException("Property '" + propertyInfo + "' not found in System.Diagnostics.Process");
            }

            process = Process.GetCurrentProcess();
        }

        /// <summary>
        ///     Closes the layout renderer.
        /// </summary>
        protected override void CloseLayoutRenderer()
        {
            if (process != null)
            {
                process.Close();
                process = null;
            }

            base.CloseLayoutRenderer();
        }

        /// <summary>
        ///     Renders the selected process information.
        /// </summary>
        /// <param name="builder">
        ///     The <see cref="StringBuilder" /> to append the rendered data to.
        /// </param>
        /// <param name="logEvent">Logging event.</param>
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            if (propertyInfo != null)
            {
                builder.Append(Convert.ToString(propertyInfo.GetValue(process, null), CultureInfo.InvariantCulture));
            }
        }
    }
}

#endif