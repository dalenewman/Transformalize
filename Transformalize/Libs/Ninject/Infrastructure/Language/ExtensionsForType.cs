#region License
// /*
// See license included in this library folder.
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