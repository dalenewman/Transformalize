#region License
// /*
// See license included in this library folder.
// */
#endregion

using Transformalize.Libs.NLog.Config;

namespace Transformalize.Libs.NLog.LayoutRenderers.Wrappers
{
    /// <summary>
    ///     Only outputs the inner layout when exception has been defined for log message.
    /// </summary>
    [LayoutRenderer("onexception")]
    [ThreadAgnostic]
    public sealed class OnExceptionLayoutRendererWrapper : WrapperLayoutRendererBase
    {
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
            if (logEvent.Exception != null)
            {
                return base.RenderInner(logEvent);
            }

            return string.Empty;
        }
    }
}