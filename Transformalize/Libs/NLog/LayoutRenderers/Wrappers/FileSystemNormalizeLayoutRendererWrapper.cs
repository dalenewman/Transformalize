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
    ///     Filters characters not allowed in the file names by replacing them with safe character.
    /// </summary>
    [LayoutRenderer("filesystem-normalize")]
    [AmbientProperty("FSNormalize")]
    [ThreadAgnostic]
    public sealed class FileSystemNormalizeLayoutRendererWrapper : WrapperLayoutRendererBase
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="FileSystemNormalizeLayoutRendererWrapper" /> class.
        /// </summary>
        public FileSystemNormalizeLayoutRendererWrapper()
        {
            FSNormalize = true;
        }

        /// <summary>
        ///     Gets or sets a value indicating whether to modify the output of this renderer so it can be used as a part of file path
        ///     (illegal characters are replaced with '_').
        /// </summary>
        /// <docgen category='Advanced Options' order='10' />
        [DefaultValue(true)]
        public bool FSNormalize { get; set; }

        /// <summary>
        ///     Post-processes the rendered message.
        /// </summary>
        /// <param name="text">The text to be post-processed.</param>
        /// <returns>Padded and trimmed string.</returns>
        protected override string Transform(string text)
        {
            if (FSNormalize)
            {
                var builder = new StringBuilder(text);
                for (var i = 0; i < builder.Length; i++)
                {
                    var c = builder[i];
                    if (!IsSafeCharacter(c))
                    {
                        builder[i] = '_';
                    }
                }

                return builder.ToString();
            }

            return text;
        }

        private static bool IsSafeCharacter(char c)
        {
            if (char.IsLetterOrDigit(c) || c == '_' || c == '-' || c == '.' || c == ' ')
            {
                return true;
            }

            return false;
        }
    }
}