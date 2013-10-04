#region License
// /*
// See license included in this library folder.
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