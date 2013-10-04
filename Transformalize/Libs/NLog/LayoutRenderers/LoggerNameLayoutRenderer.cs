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
    ///     The logger name.
    /// </summary>
    [LayoutRenderer("logger")]
    [ThreadAgnostic]
    public class LoggerNameLayoutRenderer : LayoutRenderer
    {
        /// <summary>
        ///     Gets or sets a value indicating whether to render short logger name (the part after the trailing dot character).
        /// </summary>
        /// <docgen category='Rendering Options' order='10' />
        [DefaultValue(false)]
        public bool ShortName { get; set; }

        /// <summary>
        ///     Renders the logger name and appends it to the specified <see cref="StringBuilder" />.
        /// </summary>
        /// <param name="builder">
        ///     The <see cref="StringBuilder" /> to append the rendered data to.
        /// </param>
        /// <param name="logEvent">Logging event.</param>
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            if (ShortName)
            {
                var lastDot = logEvent.LoggerName.LastIndexOf('.');
                if (lastDot < 0)
                {
                    builder.Append(logEvent.LoggerName);
                }
                else
                {
                    builder.Append(logEvent.LoggerName.Substring(lastDot + 1));
                }
            }
            else
            {
                builder.Append(logEvent.LoggerName);
            }
        }
    }
}