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
using System.Text;
using Transformalize.Libs.NLog.Config;

namespace Transformalize.Libs.NLog.LayoutRenderers
{
    /// <summary>
    ///     The date and time in a long, sortable format yyyy-MM-dd HH:mm:ss.mmm.
    /// </summary>
    [LayoutRenderer("longdate")]
    [ThreadAgnostic]
    public class LongDateLayoutRenderer : LayoutRenderer
    {
        /// <summary>
        ///     Gets or sets a value indicating whether to output UTC time instead of local time.
        /// </summary>
        /// <docgen category='Rendering Options' order='10' />
        [DefaultValue(false)]
        public bool UniversalTime { get; set; }

        /// <summary>
        ///     Renders the date in the long format (yyyy-MM-dd HH:mm:ss.mmm) and appends it to the specified
        ///     <see
        ///         cref="StringBuilder" />
        ///     .
        /// </summary>
        /// <param name="builder">
        ///     The <see cref="StringBuilder" /> to append the rendered data to.
        /// </param>
        /// <param name="logEvent">Logging event.</param>
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            var dt = logEvent.TimeStamp;
            if (UniversalTime)
            {
                dt = dt.ToUniversalTime();
            }

            builder.Append(dt.Year);
            builder.Append('-');
            Append2DigitsZeroPadded(builder, dt.Month);
            builder.Append('-');
            Append2DigitsZeroPadded(builder, dt.Day);
            builder.Append(' ');
            Append2DigitsZeroPadded(builder, dt.Hour);
            builder.Append(':');
            Append2DigitsZeroPadded(builder, dt.Minute);
            builder.Append(':');
            Append2DigitsZeroPadded(builder, dt.Second);
            builder.Append('.');
            Append4DigitsZeroPadded(builder, (int) (dt.Ticks%10000000)/1000);
        }

        private static void Append2DigitsZeroPadded(StringBuilder builder, int number)
        {
            builder.Append((char) ((number/10) + '0'));
            builder.Append((char) ((number%10) + '0'));
        }

        private static void Append4DigitsZeroPadded(StringBuilder builder, int number)
        {
            builder.Append((char) (((number/1000)%10) + '0'));
            builder.Append((char) (((number/100)%10) + '0'));
            builder.Append((char) (((number/10)%10) + '0'));
            builder.Append((char) (((number/1)%10) + '0'));
        }
    }
}