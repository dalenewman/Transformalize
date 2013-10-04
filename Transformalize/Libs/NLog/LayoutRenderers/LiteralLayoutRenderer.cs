#region License
// /*
// See license included in this library folder.
// */
#endregion

using System.Text;
using Transformalize.Libs.NLog.Config;

namespace Transformalize.Libs.NLog.LayoutRenderers
{
    /// <summary>
    ///     A string literal.
    /// </summary>
    /// <remarks>
    ///     This is used to escape '${' sequence
    ///     as ;${literal:text=${}'
    /// </remarks>
    [LayoutRenderer("literal")]
    [ThreadAgnostic]
    [AppDomainFixedOutput]
    public class LiteralLayoutRenderer : LayoutRenderer
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="LiteralLayoutRenderer" /> class.
        /// </summary>
        public LiteralLayoutRenderer()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="LiteralLayoutRenderer" /> class.
        /// </summary>
        /// <param name="text">The literal text value.</param>
        /// <remarks>This is used by the layout compiler.</remarks>
        public LiteralLayoutRenderer(string text)
        {
            Text = text;
        }

        /// <summary>
        ///     Gets or sets the literal text.
        /// </summary>
        /// <docgen category='Rendering Options' order='10' />
        public string Text { get; set; }

        /// <summary>
        ///     Renders the specified string literal and appends it to the specified <see cref="StringBuilder" />.
        /// </summary>
        /// <param name="builder">
        ///     The <see cref="StringBuilder" /> to append the rendered data to.
        /// </param>
        /// <param name="logEvent">Logging event.</param>
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            builder.Append(Text);
        }
    }
}