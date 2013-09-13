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

using System.Net;

namespace Transformalize.Libs.RazorEngine.Text
{
    /// <summary>
    ///     Represents a Html-encoded string.
    /// </summary>
    public class HtmlEncodedString : IEncodedString
    {
        #region Fields

        private readonly string _encodedString;

        #endregion

        #region Constructor

        /// <summary>
        ///     Initialises a new instance of <see cref="HtmlEncodedString" />
        /// </summary>
        /// <param name="value">The raw string to be encoded.</param>
        public HtmlEncodedString(string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
                _encodedString = WebUtility.HtmlEncode(value);
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Gets the encoded string.
        /// </summary>
        /// <returns>The encoded string.</returns>
        public string ToEncodedString()
        {
            return _encodedString ?? string.Empty;
        }

        /// <summary>
        ///     Gets the string representation of this instance.
        /// </summary>
        /// <returns>The string representation of this instance.</returns>
        public override string ToString()
        {
            return ToEncodedString();
        }

        #endregion
    }
}