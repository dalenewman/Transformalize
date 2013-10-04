#region License
// /*
// See license included in this library folder.
// */
#endregion

using Transformalize.Libs.NLog.LayoutRenderers;

namespace Transformalize.Libs.NLog.Layouts
{
    /// <summary>
    ///     A specialized layout that renders Log4j-compatible XML events.
    /// </summary>
    /// <remarks>
    ///     This layout is not meant to be used explicitly. Instead you can use ${log4jxmlevent} layout renderer.
    /// </remarks>
    [Layout("Log4JXmlEventLayout")]
    public class Log4JXmlEventLayout : Layout
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="Log4JXmlEventLayout" /> class.
        /// </summary>
        public Log4JXmlEventLayout()
        {
            Renderer = new Log4JXmlEventLayoutRenderer();
        }

        /// <summary>
        ///     Gets the <see cref="Log4JXmlEventLayoutRenderer" /> instance that renders log events.
        /// </summary>
        public Log4JXmlEventLayoutRenderer Renderer { get; private set; }

        /// <summary>
        ///     Renders the layout for the specified logging event by invoking layout renderers.
        /// </summary>
        /// <param name="logEvent">The logging event.</param>
        /// <returns>The rendered layout.</returns>
        protected override string GetFormattedMessage(LogEventInfo logEvent)
        {
            string cachedValue;

            if (logEvent.TryGetCachedLayoutValue(this, out cachedValue))
            {
                return cachedValue;
            }

            return logEvent.AddCachedLayoutValue(this, Renderer.Render(logEvent));
        }
    }
}