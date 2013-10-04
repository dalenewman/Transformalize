#region License
// /*
// See license included in this library folder.
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