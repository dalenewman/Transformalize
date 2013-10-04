#region License
// /*
// See license included in this library folder.
// */
#endregion

using System.Text;
using Transformalize.Libs.NLog.Internal;

namespace Transformalize.Libs.NLog.LayoutRenderers
{
    /// <summary>
    ///     A newline literal.
    /// </summary>
    [LayoutRenderer("newline")]
    public class NewLineLayoutRenderer : LayoutRenderer
    {
        /// <summary>
        ///     Renders the specified string literal and appends it to the specified <see cref="StringBuilder" />.
        /// </summary>
        /// <param name="builder">
        ///     The <see cref="StringBuilder" /> to append the rendered data to.
        /// </param>
        /// <param name="logEvent">Logging event.</param>
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            builder.Append(EnvironmentHelper.NewLine);
        }
    }
}