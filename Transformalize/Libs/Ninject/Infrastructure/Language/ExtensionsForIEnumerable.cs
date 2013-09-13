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