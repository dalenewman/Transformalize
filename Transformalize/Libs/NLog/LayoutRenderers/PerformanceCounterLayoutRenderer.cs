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

using System.Diagnostics;
using System.Globalization;
using System.Text;
using Transformalize.Libs.NLog.Config;

#if !NET_CF && !SILVERLIGHT

namespace Transformalize.Libs.NLog.LayoutRenderers
{
    /// <summary>
    ///     The performance counter.
    /// </summary>
    [LayoutRenderer("performancecounter")]
    public class PerformanceCounterLayoutRenderer : LayoutRenderer
    {
        private PerformanceCounter perfCounter;

        /// <summary>
        ///     Gets or sets the name of the counter category.
        /// </summary>
        /// <docgen category='Performance Counter Options' order='10' />
        [RequiredParameter]
        public string Category { get; set; }

        /// <summary>
        ///     Gets or sets the name of the performance counter.
        /// </summary>
        /// <docgen category='Performance Counter Options' order='10' />
        [RequiredParameter]
        public string Counter { get; set; }

        /// <summary>
        ///     Gets or sets the name of the performance counter instance (e.g. this.Global_).
        /// </summary>
        /// <docgen category='Performance Counter Options' order='10' />
        public string Instance { get; set; }

        /// <summary>
        ///     Gets or sets the name of the machine to read the performance counter from.
        /// </summary>
        /// <docgen category='Performance Counter Options' order='10' />
        public string MachineName { get; set; }

        /// <summary>
        ///     Initializes the layout renderer.
        /// </summary>
        protected override void InitializeLayoutRenderer()
        {
            base.InitializeLayoutRenderer();

            if (MachineName != null)
            {
                perfCounter = new PerformanceCounter(Category, Counter, Instance, MachineName);
            }
            else
            {
                perfCounter = new PerformanceCounter(Category, Counter, Instance, true);
            }
        }

        /// <summary>
        ///     Closes the layout renderer.
        /// </summary>
        protected override void CloseLayoutRenderer()
        {
            base.CloseLayoutRenderer();
            if (perfCounter != null)
            {
                perfCounter.Close();
                perfCounter = null;
            }
        }

        /// <summary>
        ///     Renders the specified environment variable and appends it to the specified <see cref="StringBuilder" />.
        /// </summary>
        /// <param name="builder">
        ///     The <see cref="StringBuilder" /> to append the rendered data to.
        /// </param>
        /// <param name="logEvent">Logging event.</param>
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            builder.Append(perfCounter.NextValue().ToString(CultureInfo.InvariantCulture));
        }
    }
}

#endif