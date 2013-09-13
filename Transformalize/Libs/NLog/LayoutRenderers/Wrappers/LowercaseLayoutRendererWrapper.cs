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