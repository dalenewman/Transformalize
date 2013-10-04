#region License
// /*
// See license included in this library folder.
// */
#endregion

using Transformalize.Libs.NLog.Conditions;
using Transformalize.Libs.NLog.Config;

namespace Transformalize.Libs.NLog.LayoutRenderers.Wrappers
{
    /// <summary>
    ///     Only outputs the inner layout when the specified condition has been met.
    /// </summary>
    [LayoutRenderer("when")]
    [AmbientProperty("When")]
    [ThreadAgnostic]
    public sealed class WhenLayoutRendererWrapper : WrapperLayoutRendererBase
    {
        /// <summary>
        ///     Gets or sets the condition that must be met for the inner layout to be printed.
        /// </summary>
        /// <docgen category="Transformation Options" order="10" />
        [RequiredParameter]
        public ConditionExpression When { get; set; }

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
            if (true.Equals(When.Evaluate(logEvent)))
            {
                return base.RenderInner(logEvent);
            }

            return string.Empty;
        }
    }
}