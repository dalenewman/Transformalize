#region License
// /*
// See license included in this library folder.
// */
#endregion

using Transformalize.Libs.NLog.Config;
using Transformalize.Libs.NLog.Internal;

namespace Transformalize.Libs.NLog.LayoutRenderers.Wrappers
{
    /// <summary>
    ///     Encodes the result of another layout output for use with URLs.
    /// </summary>
    [LayoutRenderer("url-encode")]
    [ThreadAgnostic]
    public sealed class UrlEncodeLayoutRendererWrapper : WrapperLayoutRendererBase
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="UrlEncodeLayoutRendererWrapper" /> class.
        /// </summary>
        public UrlEncodeLayoutRendererWrapper()
        {
            SpaceAsPlus = true;
        }

        /// <summary>
        ///     Gets or sets a value indicating whether spaces should be translated to '+' or '%20'.
        /// </summary>
        /// <value>
        ///     A value of <c>true</c> if space should be translated to '+'; otherwise, <c>false</c>.
        /// </value>
        /// <docgen category='Transformation Options' order='10' />
        public bool SpaceAsPlus { get; set; }

        /// <summary>
        ///     Transforms the output of another layout.
        /// </summary>
        /// <param name="text">Output to be transform.</param>
        /// <returns>Transformed text.</returns>
        protected override string Transform(string text)
        {
            return UrlHelper.UrlEncode(text, SpaceAsPlus);
        }
    }
}