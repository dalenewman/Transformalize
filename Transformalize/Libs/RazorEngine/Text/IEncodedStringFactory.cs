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

namespace Transformalize.Libs.RazorEngine.Text
{
    /// <summary>
    ///     Defines the required contract for implementing a factory for building encoded strings.
    /// </summary>
    public interface IEncodedStringFactory
    {
        #region Methods

        /// <summary>
        ///     Creates a <see cref="IEncodedString" /> instance for the specified raw string.
        /// </summary>
        /// <param name="value">The raw string.</param>
        /// <returns>
        ///     An instance of <see cref="IEncodedString" />.
        /// </returns>
        IEncodedString CreateEncodedString(string value);

        /// <summary>
        ///     Creates a <see cref="IEncodedString" /> instance for the specified object instance.
        /// </summary>
        /// <param name="value">The object instance.</param>
        /// <returns>
        ///     An instance of <see cref="IEncodedString" />.
        /// </returns>
        IEncodedString CreateEncodedString(object value);

        #endregion
    }
}