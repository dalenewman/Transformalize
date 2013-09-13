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
using Transformalize.Libs.NLog.Config;

namespace Transformalize.Libs.NLog.LayoutRenderers.Wrappers
{
    /// <summary>
    ///     Applies padding to another layout output.
    /// </summary>
    [LayoutRenderer("pad")]
    [AmbientProperty("Padding")]
    [AmbientProperty("PadCharacter")]
    [AmbientProperty("FixedLength")]
    [ThreadAgnostic]
    public sealed class PaddingLayoutRendererWrapper : WrapperLayoutRendererBase
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="PaddingLayoutRendererWrapper" /> class.
        /// </summary>
        public PaddingLayoutRendererWrapper()
        {
            PadCharacter = ' ';
        }

        /// <summary>
        ///     Gets or sets the number of characters to pad the output to.
        /// </summary>
        /// <remarks>
        ///     Positive padding values cause left padding, negative values
        ///     cause right padding to the desired width.
        /// </remarks>
        /// <docgen category='Transformation Options' order='10' />
        public int Padding { get; set; }

        /// <summary>
        ///     Gets or sets the padding character.
        /// </summary>
        /// <docgen category='Transformation Options' order='10' />
        [DefaultValue(' ')]
        public char PadCharacter { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether to trim the
        ///     rendered text to the absolute value of the padding length.
        /// </summary>
        /// <docgen category='Transformation Options' order='10' />
        [DefaultValue(false)]
        public bool FixedLength { get; set; }

        /// <summary>
        ///     Transforms the output of another layout.
        /// </summary>
        /// <param name="text">Output to be transform.</param>
        /// <returns>Transformed text.</returns>
        protected override string Transform(string text)
        {
            var s = text ?? string.Empty;

            if (Padding != 0)
            {
                if (Padding > 0)
                {
                    s = s.PadLeft(Padding, PadCharacter);
                }
                else
                {
                    s = s.PadRight(-Padding, PadCharacter);
                }

                var absolutePadding = Padding;
                if (absolutePadding < 0)
                {
                    absolutePadding = -absolutePadding;
                }

                if (FixedLength && s.Length > absolutePadding)
                {
                    s = s.Substring(0, absolutePadding);
                }
            }

            return s;
        }
    }
}