#region License
// /*
// See license included in this library folder.
// */
#endregion

using System.ComponentModel;
using Transformalize.Libs.NLog.Config;

namespace Transformalize.Libs.NLog.LayoutRenderers.Wrappers
{
    /// <summary>
    ///     Trims the whitespace from the result of another layout renderer.
    /// </summary>
    [LayoutRenderer("trim-whitespace")]
    [AmbientProperty("TrimWhiteSpace")]
    [ThreadAgnostic]
    public sealed class TrimWhiteSpaceLayoutRendererWrapper : WrapperLayoutRendererBase
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="TrimWhiteSpaceLayoutRendererWrapper" /> class.
        /// </summary>
        public TrimWhiteSpaceLayoutRendererWrapper()
        {
            TrimWhiteSpace = true;
        }

        /// <summary>
        ///     Gets or sets a value indicating whether lower case conversion should be applied.
        /// </summary>
        /// <value>
        ///     A value of <c>true</c> if lower case conversion should be applied; otherwise, <c>false</c>.
        /// </value>
        /// <docgen category='Transformation Options' order='10' />
        [DefaultValue(true)]
        public bool TrimWhiteSpace { get; set; }

        /// <summary>
        ///     Post-processes the rendered message.
        /// </summary>
        /// <param name="text">The text to be post-processed.</param>
        /// <returns>Trimmed string.</returns>
        protected override string Transform(string text)
        {
            return TrimWhiteSpace ? text.Trim() : text;
        }
    }
}