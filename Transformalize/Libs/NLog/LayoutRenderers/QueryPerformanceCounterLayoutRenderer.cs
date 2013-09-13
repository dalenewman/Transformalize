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
using System.Globalization;
using System.Text;
using Transformalize.Libs.NLog.Internal;

#if !SILVERLIGHT

namespace Transformalize.Libs.NLog.LayoutRenderers
{
    /// <summary>
    ///     High precision timer, based on the value returned from QueryPerformanceCounter() optionally converted to seconds.
    /// </summary>
    [LayoutRenderer("qpc")]
    public class QueryPerformanceCounterLayoutRenderer : LayoutRenderer
    {
        private ulong firstQpcValue;
        private double frequency = 1;
        private ulong lastQpcValue;
        private bool raw;

        /// <summary>
        ///     Initializes a new instance of the <see cref="QueryPerformanceCounterLayoutRenderer" /> class.
        /// </summary>
        public QueryPerformanceCounterLayoutRenderer()
        {
            Normalize = true;
            Difference = false;
            Precision = 4;
            AlignDecimalPoint = true;
        }

        /// <summary>
        ///     Gets or sets a value indicating whether to normalize the result by subtracting
        ///     it from the result of the first call (so that it's effectively zero-based).
        /// </summary>
        /// <docgen category='Rendering Options' order='10' />
        [DefaultValue(true)]
        public bool Normalize { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether to output the difference between the result
        ///     of QueryPerformanceCounter and the previous one.
        /// </summary>
        /// <docgen category='Rendering Options' order='10' />
        [DefaultValue(false)]
        public bool Difference { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether to convert the result to seconds by dividing
        ///     by the result of QueryPerformanceFrequency().
        /// </summary>
        /// <docgen category='Rendering Options' order='10' />
        [DefaultValue(true)]
        public bool Seconds
        {
            get { return !raw; }
            set { raw = !value; }
        }

        /// <summary>
        ///     Gets or sets the number of decimal digits to be included in output.
        /// </summary>
        /// <docgen category='Rendering Options' order='10' />
        [DefaultValue(4)]
        public int Precision { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether to align decimal point (emit non-significant zeros).
        /// </summary>
        /// <docgen category='Rendering Options' order='10' />
        [DefaultValue(true)]
        public bool AlignDecimalPoint { get; set; }

        /// <summary>
        ///     Initializes the layout renderer.
        /// </summary>
        protected override void InitializeLayoutRenderer()
        {
            base.InitializeLayoutRenderer();

            ulong performanceFrequency;

            if (!NativeMethods.QueryPerformanceFrequency(out performanceFrequency))
            {
                throw new InvalidOperationException("Cannot determine high-performance counter frequency.");
            }

            ulong qpcValue;

            if (!NativeMethods.QueryPerformanceCounter(out qpcValue))
            {
                throw new InvalidOperationException("Cannot determine high-performance counter value.");
            }

            frequency = performanceFrequency;
            firstQpcValue = qpcValue;
            lastQpcValue = qpcValue;
        }

        /// <summary>
        ///     Renders the ticks value of current time and appends it to the specified <see cref="StringBuilder" />.
        /// </summary>
        /// <param name="builder">
        ///     The <see cref="StringBuilder" /> to append the rendered data to.
        /// </param>
        /// <param name="logEvent">Logging event.</param>
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            ulong qpcValue;

            if (!NativeMethods.QueryPerformanceCounter(out qpcValue))
            {
                return;
            }

            var v = qpcValue;

            if (Difference)
            {
                qpcValue -= lastQpcValue;
            }
            else if (Normalize)
            {
                qpcValue -= firstQpcValue;
            }

            lastQpcValue = v;

            string stringValue;

            if (Seconds)
            {
                var val = Math.Round(qpcValue/frequency, Precision);

                stringValue = Convert.ToString(val, CultureInfo.InvariantCulture);
                if (AlignDecimalPoint)
                {
                    var p = stringValue.IndexOf('.');
                    if (p == -1)
                    {
                        stringValue += "." + new string('0', Precision);
                    }
                    else
                    {
                        stringValue += new string('0', Precision - (stringValue.Length - 1 - p));
                    }
                }
            }
            else
            {
                stringValue = Convert.ToString(qpcValue, CultureInfo.InvariantCulture);
            }

            builder.Append(stringValue);
        }
    }
}

#endif