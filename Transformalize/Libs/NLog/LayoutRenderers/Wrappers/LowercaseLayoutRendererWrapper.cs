#region License
// /*
// See license included in this library folder.
// */
#endregion

using System.ComponentModel;
using System.Globalization;
using Transformalize.Libs.NLog.Config;

namespace Transformalize.Libs.NLog.LayoutRenderers.Wrappers
{
    /// <summary>
    ///     Converts the result of another layout output to lower case.
    /// </summary>
    [LayoutRenderer("lowercase")]
    [AmbientProperty("Lowercase")]
    [ThreadAgnostic]
    public sealed class LowercaseLayoutRendererWrapper : WrapperLayoutRendererBase
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="LowercaseLayoutRendererWrapper" /> class.
        /// </summary>
        public LowercaseLayoutRendererWrapper()
        {
            Culture = CultureInfo.InvariantCulture;
            Lowercase = true;
        }

        /// <summary>
        ///     Gets or sets a value indicating whether lower case conversion should be applied.
        /// </summary>
        /// <value>
        ///     A value of <c>true</c> if lower case conversion should be applied; otherwise, <c>false</c>.
        /// </value>
        /// <docgen category='Transformation Options' order='10' />
        [DefaultValue(true)]
        public bool Lowercase { get; set; }

        /// <summary>
        ///     Gets or sets the culture used for rendering.
        /// </summary>
        /// <docgen category='Transformation Options' order='10' />
        public CultureInfo Culture { get; set; }

        /// <summary>
        ///     Post-processes the rendered message.
        /// </summary>
        /// <param name="text">The text to be post-processed.</param>
        /// <returns>Padded and trimmed string.</returns>
        protected override string Transform(string text)
        {
            return Lowercase ? text.ToLower(Culture) : text;
        }
    }
}