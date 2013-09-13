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

using System.Text;
using Transformalize.Libs.NLog.Config;

namespace Transformalize.Libs.NLog.LayoutRenderers
{
    /// <summary>
    ///     The process time in format HH:mm:ss.mmm.
    /// </summary>
    [LayoutRenderer("processtime")]
    [ThreadAgnostic]
    public class ProcessTimeLayoutRenderer : LayoutRenderer
    {
        /// <summary>
        ///     Renders the current process running time and appends it to the specified <see cref="StringBuilder" />.
        /// </summary>
        /// <param name="builder">
        ///     The <see cref="StringBuilder" /> to append the rendered data to.
        /// </param>
        /// <param name="logEvent">Logging event.</param>
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            var ts = logEvent.TimeStamp.ToUniversalTime() - LogEventInfo.ZeroDate;
            if (ts.Hours < 10)
            {
                builder.Append('0');
            }

            builder.Append(ts.Hours);
            builder.Append(':');
            if (ts.Minutes < 10)
            {
                builder.Append('0');
            }

            builder.Append(ts.Minutes);
            builder.Append(':');
            if (ts.Seconds < 10)
            {
                builder.Append('0');
            }

            builder.Append(ts.Seconds);
            builder.Append('.');
            if (ts.Milliseconds < 1000)
            {
                builder.Append('0');
            }

            if (ts.Milliseconds < 100)
            {
                builder.Append('0');
            }

            if (ts.Milliseconds < 10)
            {
                builder.Append('0');
            }

            builder.Append(ts.Milliseconds);
        }
    }
}