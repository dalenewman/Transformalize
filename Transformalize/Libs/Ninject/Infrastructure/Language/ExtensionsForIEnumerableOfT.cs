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

#region Using Directives

using System;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace Transformalize.Libs.Ninject.Infrastructure.Language
{
    /// <summary>
    ///     Provides extension methods for see cref="IEnumerable{T}"/>
    /// </summary>
    public static class ExtensionsForIEnumerableOfT
    {
        /// <summary>
        ///     Executes the given action for each of the elements in the enumerable.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="series">The series.</param>
        /// <param name="action">The action.</param>
        public static void Map<T>(this IEnumerable<T> series, Action<T> action)
        {
            foreach (var item in series)
                action(item);
        }

        /// <summary>
        ///     Converts the given enumerable type to prevent changed on the type behind.
        /// </summary>
        /// <typeparam name="T">The type of the enumerable.</typeparam>
        /// <param name="series">The series.</param>
        /// <returns>The input type as real enumerable not castable to the original type.</returns>
        public static IEnumerable<T> ToEnumerable<T>(this IEnumerable<T> series)
        {
            return series.Select(x => x);
        }
    }
}