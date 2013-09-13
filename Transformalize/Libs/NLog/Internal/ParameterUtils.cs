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

namespace Transformalize.Libs.NLog.Internal
{
    /// <summary>
    ///     Parameter validation utilities.
    /// </summary>
    internal static class ParameterUtils
    {
        /// <summary>
        ///     Asserts that the value is not null and throws <see cref="ArgumentNullException" /> otherwise.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <param name="parameterName">Name of the parameter.</param>
        public static void AssertNotNull(object value, string parameterName)
        {
            if (value == null)
            {
                throw new ArgumentNullException(parameterName);
            }
        }
    }
}