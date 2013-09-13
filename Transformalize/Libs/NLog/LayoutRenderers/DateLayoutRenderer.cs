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

using System.ComponentModel;
using System.Globalization;
using System.Text;
using Transformalize.Libs.NLog.Config;

namespace Transformalize.Libs.NLog.LayoutRenderers
{
    /// <summary>
    ///     Current date and time.
    /// </summary>
    [LayoutRenderer("date")]
    [ThreadAgnostic]
    public class DateLayoutRenderer : LayoutRenderer
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="DateLayoutRenderer" /> class.
        /// </summary>
        public DateLayoutRenderer()
        {
            Format = "G";
            Culture = CultureInfo.InvariantCulture;
        }

        /// <summary>
        ///     Gets or sets the culture used for rendering.
        /// </summary>
        /// <docgen category='Rendering Options' order='10' />
        public CultureInfo Culture { get; set; }

        /// <summary>
        ///     Gets or sets the date format. Can be any argument accepted by DateTime.ToString(format).
        /// </summary>
        /// <docgen category='Rendering Options' order='10' />
        [DefaultParameter]
        public string Format { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether to output UTC time instead of local time.
        /// </summary>
        /// <docgen category='Rendering Options' order='10' />
        [DefaultValue(false)]
        public bool UniversalTime { get; set; }

        /// <summary>
        ///     Renders the current date and appends it to the specified <see cref="StringBuilder" />.
        /// </summary>
        /// <param name="builder">
        ///     The <see cref="StringBuilder" /> to append the rendered data to.
        /// </param>
        /// <param name="logEvent">Logging event.</param>
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            var ts = logEvent.TimeStamp;
            if (UniversalTime)
            {
                ts = ts.ToUniversalTime();
            }

            builder.Append(ts.ToString(Format, Culture));
        }
    }
}