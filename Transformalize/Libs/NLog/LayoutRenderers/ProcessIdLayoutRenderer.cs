#region License
// /*
// See license included in this library folder.
// */
#endregion

using System.Globalization;
using System.Text;
using Transformalize.Libs.NLog.Config;
using Transformalize.Libs.NLog.Internal;

#if !SILVERLIGHT

namespace Transformalize.Libs.NLog.LayoutRenderers
{
    /// <summary>
    ///     The identifier of the current process.
    /// </summary>
    [LayoutRenderer("processid")]
    [AppDomainFixedOutput]
    [ThreadAgnostic]
    public class ProcessIdLayoutRenderer : LayoutRenderer
    {
        /// <summary>
        ///     Renders the current process ID.
        /// </summary>
        /// <param name="builder">
        ///     The <see cref="StringBuilder" /> to append the rendered data to.
        /// </param>
        /// <param name="logEvent">Logging event.</param>
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            builder.Append(ThreadIDHelper.Instance.CurrentProcessID.ToString(CultureInfo.InvariantCulture));
        }
    }
}

#endif