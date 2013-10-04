#region License
// /*
// See license included in this library folder.
// */
#endregion

using Transformalize.Libs.NLog.Config;

namespace Transformalize.Libs.NLog.Layouts
{
    /// <summary>
    ///     A specialized layout that supports header and footer.
    /// </summary>
    [Layout("LayoutWithHeaderAndFooter")]
    [ThreadAgnostic]
    public class LayoutWithHeaderAndFooter : Layout
    {
        /// <summary>
        ///     Gets or sets the body layout (can be repeated multiple times).
        /// </summary>
        /// <docgen category='Layout Options' order='10' />
        public Layout Layout { get; set; }

        /// <summary>
        ///     Gets or sets the header layout.
        /// </summary>
        /// <docgen category='Layout Options' order='10' />
        public Layout Header { get; set; }

        /// <summary>
        ///     Gets or sets the footer layout.
        /// </summary>
        /// <docgen category='Layout Options' order='10' />
        public Layout Footer { get; set; }

        /// <summary>
        ///     Renders the layout for the specified logging event by invoking layout renderers.
        /// </summary>
        /// <param name="logEvent">The logging event.</param>
        /// <returns>The rendered layout.</returns>
        protected override string GetFormattedMessage(LogEventInfo logEvent)
        {
            return Layout.Render(logEvent);
        }
    }
}