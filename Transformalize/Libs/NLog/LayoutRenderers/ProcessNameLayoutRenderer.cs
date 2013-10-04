#region License
// /*
// See license included in this library folder.
// */
#endregion

using System.ComponentModel;
using System.Text;
using Transformalize.Libs.NLog.Config;
using Transformalize.Libs.NLog.Internal;

#if !SILVERLIGHT

namespace Transformalize.Libs.NLog.LayoutRenderers
{
    /// <summary>
    ///     The name of the current process.
    /// </summary>
    [LayoutRenderer("processname")]
    [AppDomainFixedOutput]
    [ThreadAgnostic]
    public class ProcessNameLayoutRenderer : LayoutRenderer
    {
        /// <summary>
        ///     Gets or sets a value indicating whether to write the full path to the process executable.
        /// </summary>
        /// <docgen category='Rendering Options' order='10' />
        [DefaultValue(false)]
        public bool FullName { get; set; }

        /// <summary>
        ///     Renders the current process name (optionally with a full path).
        /// </summary>
        /// <param name="builder">
        ///     The <see cref="StringBuilder" /> to append the rendered data to.
        /// </param>
        /// <param name="logEvent">Logging event.</param>
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            if (FullName)
            {
                builder.Append(ThreadIDHelper.Instance.CurrentProcessName);
            }
            else
            {
                builder.Append(ThreadIDHelper.Instance.CurrentProcessBaseName);
            }
        }
    }
}

#endif