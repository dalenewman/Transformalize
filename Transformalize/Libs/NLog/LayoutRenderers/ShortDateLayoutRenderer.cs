#region License
// /*
// See license included in this library folder.
// */
#endregion

using System.ComponentModel;
using System.Globalization;
using System.Text;
using Transformalize.Libs.NLog.Config;

namespace Transformalize.Libs.NLog.LayoutRenderers
{
    /// <summary>
    ///     The short date in a sortable format yyyy-MM-dd.
    /// </summary>
    [LayoutRenderer("shortdate")]
    [ThreadAgnostic]
    public class ShortDateLayoutRenderer : LayoutRenderer
    {
        /// <summary>
        ///     Gets or sets a value indicating whether to output UTC time instead of local time.
        /// </summary>
        /// <docgen category='Rendering Options' order='10' />
        [DefaultValue(false)]
        public bool UniversalTime { get; set; }

        /// <summary>
        ///     Renders the current short date string (yyyy-MM-dd) and appends it to the specified <see cref="StringBuilder" />.
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

            builder.Append(ts.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
        }
    }
}