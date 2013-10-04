#region License
// /*
// See license included in this library folder.
// */
#endregion

using System.Globalization;
using System.Text;
using Transformalize.Libs.NLog.Config;

namespace Transformalize.Libs.NLog.LayoutRenderers
{
    /// <summary>
    ///     The Ticks value of current date and time.
    /// </summary>
    [LayoutRenderer("ticks")]
    [ThreadAgnostic]
    public class TicksLayoutRenderer : LayoutRenderer
    {
        /// <summary>
        ///     Renders the ticks value of current time and appends it to the specified <see cref="StringBuilder" />.
        /// </summary>
        /// <param name="builder">
        ///     The <see cref="StringBuilder" /> to append the rendered data to.
        /// </param>
        /// <param name="logEvent">Logging event.</param>
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            builder.Append(logEvent.TimeStamp.Ticks.ToString(CultureInfo.InvariantCulture));
        }
    }
}