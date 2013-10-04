#region License
// /*
// See license included in this library folder.
// */
#endregion

using System.Text;
using System.Threading;

namespace Transformalize.Libs.NLog.LayoutRenderers
{
    /// <summary>
    ///     The name of the current thread.
    /// </summary>
    [LayoutRenderer("threadname")]
    public class ThreadNameLayoutRenderer : LayoutRenderer
    {
        /// <summary>
        ///     Renders the current thread name and appends it to the specified <see cref="StringBuilder" />.
        /// </summary>
        /// <param name="builder">
        ///     The <see cref="StringBuilder" /> to append the rendered data to.
        /// </param>
        /// <param name="logEvent">Logging event.</param>
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            builder.Append(Thread.CurrentThread.Name);
        }
    }
}