#region License
// /*
// See license included in this library folder.
// */
#endregion

using System.Text;
using Transformalize.Libs.NLog.Config;

namespace Transformalize.Libs.NLog.LayoutRenderers
{
    /// <summary>
    ///     The log level.
    /// </summary>
    [LayoutRenderer("level")]
    [ThreadAgnostic]
    public class LevelLayoutRenderer : LayoutRenderer
    {
        /// <summary>
        ///     Renders the current log level and appends it to the specified <see cref="StringBuilder" />.
        /// </summary>
        /// <param name="builder">
        ///     The <see cref="StringBuilder" /> to append the rendered data to.
        /// </param>
        /// <param name="logEvent">Logging event.</param>
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            builder.Append(logEvent.Level);
        }
    }
}