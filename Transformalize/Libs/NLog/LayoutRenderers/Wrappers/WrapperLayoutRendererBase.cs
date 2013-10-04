#region License
// /*
// See license included in this library folder.
// */
#endregion

using System.Text;
using Transformalize.Libs.NLog.Config;
using Transformalize.Libs.NLog.Layouts;

namespace Transformalize.Libs.NLog.LayoutRenderers.Wrappers
{
    /// <summary>
    ///     Decodes text "encrypted" with ROT-13.
    /// </summary>
    /// <remarks>
    ///     See <a href="http://en.wikipedia.org/wiki/ROT13">http://en.wikipedia.org/wiki/ROT13</a>.
    /// </remarks>
    public abstract class WrapperLayoutRendererBase : LayoutRenderer
    {
        /// <summary>
        ///     Gets or sets the wrapped layout.
        /// </summary>
        /// <docgen category='Transformation Options' order='10' />
        [DefaultParameter]
        public Layout Inner { get; set; }

        /// <summary>
        ///     Renders the inner message, processes it and appends it to the specified <see cref="StringBuilder" />.
        /// </summary>
        /// <param name="builder">
        ///     The <see cref="StringBuilder" /> to append the rendered data to.
        /// </param>
        /// <param name="logEvent">Logging event.</param>
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            var msg = RenderInner(logEvent);
            builder.Append(Transform(msg));
        }

        /// <summary>
        ///     Transforms the output of another layout.
        /// </summary>
        /// <param name="text">Output to be transform.</param>
        /// <returns>Transformed text.</returns>
        protected abstract string Transform(string text);

        /// <summary>
        ///     Renders the inner layout contents.
        /// </summary>
        /// <param name="logEvent">The log event.</param>
        /// <returns>Contents of inner layout.</returns>
        protected virtual string RenderInner(LogEventInfo logEvent)
        {
            return Inner.Render(logEvent);
        }
    }
}