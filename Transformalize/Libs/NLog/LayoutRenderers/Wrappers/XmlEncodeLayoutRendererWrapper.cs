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

using System.ComponentModel;
using System.Text;
using Transformalize.Libs.NLog.Config;

namespace Transformalize.Libs.NLog.LayoutRenderers.Wrappers
{
    /// <summary>
    ///     Converts the result of another layout output to be XML-compliant.
    /// </summary>
    [LayoutRenderer("xml-encode")]
    [AmbientProperty("XmlEncode")]
    [ThreadAgnostic]
    public sealed class XmlEncodeLayoutRendererWrapper : WrapperLayoutRendererBase
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="XmlEncodeLayoutRendererWrapper" /> class.
        /// </summary>
        public XmlEncodeLayoutRendererWrapper()
        {
            XmlEncode = true;
        }

        /// <summary>
        ///     Gets or sets a value indicating whether to apply XML encoding.
        /// </summary>
        /// <docgen category="Transformation Options" order="10" />
        [DefaultValue(true)]
        public bool XmlEncode { get; set; }

        /// <summary>
        ///     Post-processes the rendered message.
        /// </summary>
        /// <param name="text">The text to be post-processed.</param>
        /// <returns>Padded and trimmed string.</returns>
        protected override string Transform(string text)
        {
            return XmlEncode ? DoXmlEscape(text) : text;
        }

        private static string DoXmlEscape(string text)
        {
            var sb = new StringBuilder(text.Length);

            for (var i = 0; i < text.Length; ++i)
            {
                switch (text[i])
                {
                    case '<':
                        sb.Append("&lt;");
                        break;

                    case '>':
                        sb.Append("&gt;");
                        break;

                    case '&':
                        sb.Append("&amp;");
                        break;

                    case '\'':
                        sb.Append("&apos;");
                        break;

                    case '"':
                        sb.Append("&quot;");
                        break;

                    default:
                        sb.Append(text[i]);
                        break;
                }
            }

            return sb.ToString();
        }
    }
}