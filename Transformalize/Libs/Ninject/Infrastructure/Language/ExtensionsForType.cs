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
using System.Collections.Generic;

namespace Transformalize.Libs.Ninject.Infrastructure.Language
{
    /// <summary>
    ///     Extension methods for type
    /// </summary>
    /// <remarks></remarks>
    public static class ExtensionsForType
    {
        /// <summary>
        ///     Gets an enumerable containing the given type and all its base types
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>An enumerable containing the given type and all its base types</returns>
        public static IEnumerable<Type> GetAllBaseTypes(this Type type)
        {
            while (type != null)
            {
                yield return type;
                type = type.BaseType;
            }
        }
    }
}