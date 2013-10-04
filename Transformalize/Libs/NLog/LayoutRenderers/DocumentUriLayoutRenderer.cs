#region License
// /*
// See license included in this library folder.
// */
#endregion
namespace Transformalize.Libs.NLog.LayoutRenderers
{
#if (SILVERLIGHT || DOCUMENTATION) && !WINDOWS_PHONE

namespace NLog.LayoutRenderers
{
    using System.Text;
#if !DOCUMENTATION
    using System.Windows.Browser;
#endif

    /// <summary>
    /// URI of the HTML page which hosts the current Silverlight application.
    /// </summary>
    [LayoutRenderer("document-uri")]
    public class DocumentUriLayoutRenderer : LayoutRenderer
    {
        /// <summary>
        /// Renders the specified environmental information and appends it to the specified <see cref="StringBuilder"/>.
        /// </summary>
        /// <param name="builder">The <see cref="StringBuilder"/> to append the rendered data to.</param>
        /// <param name="logEvent">Logging event.</param>
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
#if !DOCUMENTATION
            builder.Append(HtmlPage.Document.DocumentUri.ToString());
#endif
        }
    }
}

#endif
}