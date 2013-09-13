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

using System;
using System.ComponentModel;
using System.Text;
using Transformalize.Libs.NLog.Config;

namespace Transformalize.Libs.NLog.LayoutRenderers.Wrappers
{
    /// <summary>
    ///     Escapes output of another layout using JSON rules.
    /// </summary>
    [LayoutRenderer("json-encode")]
    [AmbientProperty("JsonEncode")]
    [ThreadAgnostic]
    public sealed class JsonEncodeLayoutRendererWrapper : WrapperLayoutRendererBase
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="JsonEncodeLayoutRendererWrapper" /> class.
        /// </summary>
        public JsonEncodeLayoutRendererWrapper()
        {
            JsonEncode = true;
        }

        /// <summary>
        ///     Gets or sets a value indicating whether to apply JSON encoding.
        /// </summary>
        /// <docgen category="Transformation Options" order="10" />
        [DefaultValue(true)]
        public bool JsonEncode { get; set; }

        /// <summary>
        ///     Post-processes the rendered message.
        /// </summary>
        /// <param name="text">The text to be post-processed.</param>
        /// <returns>JSON-encoded string.</returns>
        protected override string Transform(string text)
        {
            return JsonEncode ? DoJsonEscape(text) : text;
        }

        private static string DoJsonEscape(string text)
        {
            var sb = new StringBuilder(text.Length);

            for (var i = 0; i < text.Length; ++i)
            {
                switch (text[i])
                {
                    case '"':
                        sb.Append("\\\"");
                        break;

                    case '\\':
                        sb.Append("\\\\");
                        break;

                    case '/':
                        sb.Append("\\/");
                        break;

                    case '\b':
                        sb.Append("\\b");
                        break;

                    case '\r':
                        sb.Append("\\r");
                        break;

                    case '\n':
                        sb.Append("\\n");
                        break;

                    case '\f':
                        sb.Append("\\f");
                        break;

                    case '\t':
                        sb.Append("\\t");
                        break;

                    default:
                        if (NeedsEscaping(text[i]))
                        {
                            sb.Append("\\u");
                            sb.Append(Convert.ToString(text[i], 16).PadLeft(4, '0'));
                        }
                        else
                        {
                            sb.Append(text[i]);
                        }

                        break;
                }
            }

            return sb.ToString();
        }

        private static bool NeedsEscaping(char ch)
        {
            return ch < 32 || ch > 127;
        }
    }
}