#region License
// /*
// See license included in this library folder.
// */
#endregion

using System.Globalization;
using System.Text;
using System.Threading;

namespace Transformalize.Libs.NLog.LayoutRenderers
{
    /// <summary>
    ///     The identifier of the current thread.
    /// </summary>
    [LayoutRenderer("threadid")]
    public class ThreadIdLayoutRenderer : LayoutRenderer
    {
        /// <summary>
        ///     Renders the current thread identifier and appends it to the specified <see cref="StringBuilder" />.
        /// </summary>
        /// <param name="builder">
        ///     The <see cref="StringBuilder" /> to append the rendered data to.
        /// </param>
        /// <param name="logEvent">Logging event.</param>
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            builder.Append(Thread.CurrentThread.ManagedThreadId.ToString(CultureInfo.InvariantCulture));
        }
    }
}