#region License
// /*
// See license included in this library folder.
// */
#endregion

using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;

namespace Transformalize.Libs.NLog.LayoutRenderers
{
    /// <summary>
    ///     A counter value (increases on each layout rendering).
    /// </summary>
    [LayoutRenderer("counter")]
    public class CounterLayoutRenderer : LayoutRenderer
    {
        private static readonly Dictionary<string, int> sequences = new Dictionary<string, int>();

        /// <summary>
        ///     Initializes a new instance of the <see cref="CounterLayoutRenderer" /> class.
        /// </summary>
        public CounterLayoutRenderer()
        {
            Increment = 1;
            Value = 1;
        }

        /// <summary>
        ///     Gets or sets the initial value of the counter.
        /// </summary>
        /// <docgen category='Counter Options' order='10' />
        [DefaultValue(1)]
        public int Value { get; set; }

        /// <summary>
        ///     Gets or sets the value to be added to the counter after each layout rendering.
        /// </summary>
        /// <docgen category='Counter Options' order='10' />
        [DefaultValue(1)]
        public int Increment { get; set; }

        /// <summary>
        ///     Gets or sets the name of the sequence. Different named sequences can have individual values.
        /// </summary>
        /// <docgen category='Counter Options' order='10' />
        public string Sequence { get; set; }

        /// <summary>
        ///     Renders the specified counter value and appends it to the specified <see cref="StringBuilder" />.
        /// </summary>
        /// <param name="builder">
        ///     The <see cref="StringBuilder" /> to append the rendered data to.
        /// </param>
        /// <param name="logEvent">Logging event.</param>
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            int v;

            if (Sequence != null)
            {
                v = GetNextSequenceValue(Sequence, Value, Increment);
            }
            else
            {
                v = Value;
                Value += Increment;
            }

            builder.Append(v.ToString(CultureInfo.InvariantCulture));
        }

        private static int GetNextSequenceValue(string sequenceName, int defaultValue, int increment)
        {
            lock (sequences)
            {
                int val;

                if (!sequences.TryGetValue(sequenceName, out val))
                {
                    val = defaultValue;
                }

                var retVal = val;

                val += increment;
                sequences[sequenceName] = val;
                return retVal;
            }
        }
    }
}