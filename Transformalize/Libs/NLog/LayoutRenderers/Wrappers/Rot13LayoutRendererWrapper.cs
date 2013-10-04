#region License
// /*
// See license included in this library folder.
// */
#endregion

using Transformalize.Libs.NLog.Config;
using Transformalize.Libs.NLog.Layouts;

namespace Transformalize.Libs.NLog.LayoutRenderers.Wrappers
{
    /// <summary>
    ///     Decodes text "encrypted" with ROT-13.
    /// </summary>
    /// <remarks>
    ///     See <a href="http://en.wikipedia.org/wiki/ROT13">http://en.wikipedia.org/wiki/ROT13</a>.
    /// </remarks>
    [LayoutRenderer("rot13")]
    [AppDomainFixedOutput]
    [ThreadAgnostic]
    public sealed class Rot13LayoutRendererWrapper : WrapperLayoutRendererBase
    {
        /// <summary>
        ///     Gets or sets the layout to be wrapped.
        /// </summary>
        /// <value>The layout to be wrapped.</value>
        /// <remarks>This variable is for backwards compatibility</remarks>
        /// <docgen category='Transformation Options' order='10' />
        public Layout Text
        {
            get { return Inner; }
            set { Inner = value; }
        }

        /// <summary>
        ///     Encodes/Decodes ROT-13-encoded string.
        /// </summary>
        /// <param name="encodedValue">The string to be encoded/decoded.</param>
        /// <returns>Encoded/Decoded text.</returns>
        public static string DecodeRot13(string encodedValue)
        {
            if (encodedValue == null)
            {
                return null;
            }

            var chars = encodedValue.ToCharArray();
            for (var i = 0; i < chars.Length; ++i)
            {
                chars[i] = DecodeRot13Char(chars[i]);
            }

            return new string(chars);
        }

        /// <summary>
        ///     Transforms the output of another layout.
        /// </summary>
        /// <param name="text">Output to be transform.</param>
        /// <returns>Transformed text.</returns>
        protected override string Transform(string text)
        {
            return DecodeRot13(text);
        }

        private static char DecodeRot13Char(char c)
        {
            if (c >= 'A' && c <= 'M')
            {
                return (char) ('N' + (c - 'A'));
            }

            if (c >= 'a' && c <= 'm')
            {
                return (char) ('n' + (c - 'a'));
            }

            if (c >= 'N' && c <= 'Z')
            {
                return (char) ('A' + (c - 'N'));
            }

            if (c >= 'n' && c <= 'z')
            {
                return (char) ('a' + (c - 'n'));
            }

            return c;
        }
    }
}