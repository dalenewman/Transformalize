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