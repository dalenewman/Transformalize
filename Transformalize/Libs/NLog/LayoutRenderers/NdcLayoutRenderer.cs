#region License
// /*
// See license included in this library folder.
// */
#endregion

using System;
using System.Text;

namespace Transformalize.Libs.NLog.LayoutRenderers
{
    /// <summary>
    ///     Nested Diagnostic Context item. Provided for compatibility with log4net.
    /// </summary>
    [LayoutRenderer("ndc")]
    public class NdcLayoutRenderer : LayoutRenderer
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="NdcLayoutRenderer" /> class.
        /// </summary>
        public NdcLayoutRenderer()
        {
            Separator = " ";
            BottomFrames = -1;
            TopFrames = -1;
        }

        /// <summary>
        ///     Gets or sets the number of top stack frames to be rendered.
        /// </summary>
        /// <docgen category='Rendering Options' order='10' />
        public int TopFrames { get; set; }

        /// <summary>
        ///     Gets or sets the number of bottom stack frames to be rendered.
        /// </summary>
        /// <docgen category='Rendering Options' order='10' />
        public int BottomFrames { get; set; }

        /// <summary>
        ///     Gets or sets the separator to be used for concatenating nested diagnostics context output.
        /// </summary>
        /// <docgen category='Rendering Options' order='10' />
        public string Separator { get; set; }

        /// <summary>
        ///     Renders the specified Nested Diagnostics Context item and appends it to the specified <see cref="StringBuilder" />.
        /// </summary>
        /// <param name="builder">
        ///     The <see cref="StringBuilder" /> to append the rendered data to.
        /// </param>
        /// <param name="logEvent">Logging event.</param>
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            var messages = NestedDiagnosticsContext.GetAllMessages();
            var startPos = 0;
            var endPos = messages.Length;

            if (TopFrames != -1)
            {
                endPos = Math.Min(TopFrames, messages.Length);
            }
            else if (BottomFrames != -1)
            {
                startPos = messages.Length - Math.Min(BottomFrames, messages.Length);
            }

            var totalLength = 0;
            var separatorLength = 0;

            for (var i = endPos - 1; i >= startPos; --i)
            {
                totalLength += separatorLength + messages[i].Length;
                separatorLength = Separator.Length;
            }

            var separator = string.Empty;

            var sb = new StringBuilder();
            for (var i = endPos - 1; i >= startPos; --i)
            {
                sb.Append(separator);
                sb.Append(messages[i]);
                separator = Separator;
            }

            builder.Append(sb);
        }
    }
}