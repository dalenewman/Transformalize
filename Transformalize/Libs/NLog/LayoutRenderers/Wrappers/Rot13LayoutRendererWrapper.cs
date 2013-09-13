#region License

// /*
// Transformalize - Replicate, Transform, and Denormalize Your Data...
// Copyright (C) 2013 Dale Newman
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
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