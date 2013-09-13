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
using System.Text;

namespace Transformalize.Libs.NLog.LayoutRenderers
{
    /// <summary>
    ///     Globally-unique identifier (GUID).
    /// </summary>
    [LayoutRenderer("guid")]
    public class GuidLayoutRenderer : LayoutRenderer
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="GuidLayoutRenderer" /> class.
        /// </summary>
        public GuidLayoutRenderer()
        {
            Format = "N";
        }

        /// <summary>
        ///     Gets or sets the GUID format as accepted by Guid.ToString() method.
        /// </summary>
        /// <docgen category='Rendering Options' order='10' />
        [DefaultValue("N")]
        public string Format { get; set; }

        /// <summary>
        ///     Renders a newly generated GUID string and appends it to the specified <see cref="StringBuilder" />.
        /// </summary>
        /// <param name="builder">
        ///     The <see cref="StringBuilder" /> to append the rendered data to.
        /// </param>
        /// <param name="logEvent">Logging event.</param>
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            builder.Append(Guid.NewGuid().ToString(Format));
        }
    }
}