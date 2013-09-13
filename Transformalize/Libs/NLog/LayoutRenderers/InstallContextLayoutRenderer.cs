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
using Transformalize.Libs.NLog.Config;

namespace Transformalize.Libs.NLog.LayoutRenderers
{
    /// <summary>
    ///     Installation parameter (passed to InstallNLogConfig).
    /// </summary>
    [LayoutRenderer("install-context")]
    public class InstallContextLayoutRenderer : LayoutRenderer
    {
        /// <summary>
        ///     Gets or sets the name of the parameter.
        /// </summary>
        /// <docgen category='Rendering Options' order='10' />
        [RequiredParameter]
        [DefaultParameter]
        public string Parameter { get; set; }

        /// <summary>
        ///     Renders the specified installation parameter and appends it to the specified <see cref="StringBuilder" />.
        /// </summary>
        /// <param name="builder">
        ///     The <see cref="StringBuilder" /> to append the rendered data to.
        /// </param>
        /// <param name="logEvent">Logging event.</param>
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            object value;

            if (logEvent.Properties.TryGetValue(Parameter, out value))
            {
                builder.Append(Convert.ToString(value, CultureInfo.InvariantCulture));
            }
        }
    }
}