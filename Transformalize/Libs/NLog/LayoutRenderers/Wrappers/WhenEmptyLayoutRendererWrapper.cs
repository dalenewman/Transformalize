#region License
// /*
// See license included in this library folder.
// */
#endregion

using Transformalize.Libs.NLog.Config;
using Transformalize.Libs.NLog.Layouts;

namespace Transformalize.Libs.NLog.LayoutRenderers.Wrappers
{
    /// <summary>
    ///     Outputs alternative layout when the inner layout produces empty result.
    /// </summary>
    [LayoutRenderer("whenEmpty")]
    [AmbientProperty("WhenEmpty")]
    [ThreadAgnostic]
    public sealed class WhenEmptyLayoutRendererWrapper : WrapperLayoutRendererBase
    {
        /// <summary>
        ///     Gets or sets the layout to be rendered when original layout produced empty result.
        /// </summary>
        /// <docgen category="Transformation Options" order="10" />
        [RequiredParameter]
        public Layout WhenEmpty { get; set; }

        /// <summary>
        ///     Transforms the output of another layout.
        /// </summary>
        /// <param name="text">Output to be transform.</param>
        /// <returns>Transformed text.</returns>
        protected override string Transform(string text)
        {
            return text;
        }

        /// <summary>
        ///     Renders the inner layout contents.
        /// </summary>
        /// <param name="logEvent">The log event.</param>
        /// <returns>
        ///     Contents of inner layout.
        /// </returns>
        protected override string RenderInner(LogEventInfo logEvent)
        {
            var inner = base.RenderInner(logEvent);
            if (!string.IsNullOrEmpty(inner))
            {
                return inner;
            }

            // render WhenEmpty when the inner layout was empty
            return WhenEmpty.Render(logEvent);
        }
    }
}