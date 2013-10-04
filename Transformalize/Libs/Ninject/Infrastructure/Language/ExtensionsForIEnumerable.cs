#region License
// /*
// See license included in this library folder.
// */
#endregion
#region Using Directives

using System;
using System.Collections;
using System.Linq;

#endregion

namespace Transformalize.Libs.Ninject.Infrastructure.Language
{
    internal static class ExtensionsForIEnumerable
    {
        public static IEnumerable CastSlow(this IEnumerable series, Type elementType)
        {
            var method = typeof (Enumerable).GetMethod("Cast").MakeGenericMethod(elementType);
            return method.Invoke(null, new[] {series}) as IEnumerable;
        }

        public static Array ToArraySlow(this IEnumerable series, Type elementType)
        {
            var method = typeof (Enumerable).GetMethod("ToArray").MakeGenericMethod(elementType);
            return method.Invoke(null, new[] {series}) as Array;
        }

        public static IList ToListSlow(this IEnumerable series, Type elementType)
        {
            var method = typeof (Enumerable).GetMethod("ToList").MakeGenericMethod(elementType);
            return method.Invoke(null, new[] {series}) as IList;
        }
    }
}