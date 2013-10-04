#region License
// /*
// See license included in this library folder.
// */
#endregion

using System;
using System.Globalization;
using System.Text;
using Transformalize.Libs.NLog.Config;

namespace Transformalize.Libs.NLog.LayoutRenderers
{
    /// <summary>
    ///     Log event context data.
    /// </summary>
    [LayoutRenderer("event-context")]
    public class EventContextLayoutRenderer : LayoutRenderer
    {
        /// <summary>
        ///     Gets or sets the name of the item.
        /// </summary>
        /// <docgen category='Rendering Options' order='10' />
        [RequiredParameter]
        [DefaultParameter]
        public string Item { get; set; }

        /// <summary>
        ///     Renders the specified log event context item and appends it to the specified <see cref="StringBuilder" />.
        /// </summary>
        /// <param name="builder">
        ///     The <see cref="StringBuilder" /> to append the rendered data to.
        /// </param>
        /// <param name="logEvent">Logging event.</param>
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            object value;

            if (logEvent.Properties.TryGetValue(Item, out value))
            {
                builder.Append(Convert.ToString(value, CultureInfo.InvariantCulture));
            }
        }
    }
}