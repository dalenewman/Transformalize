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
using System.Reflection;
using Transformalize.Libs.NLog.Common;

namespace Transformalize.Libs.NLog.Internal
{
    /// <summary>
    ///     Reflection helpers.
    /// </summary>
    internal static class ReflectionHelpers
    {
        /// <summary>
        ///     Gets all usable exported types from the given assembly.
        /// </summary>
        /// <param name="assembly">Assembly to scan.</param>
        /// <returns>Usable types from the given assembly.</returns>
        /// <remarks>Types which cannot be loaded are skipped.</remarks>
        public static Type[] SafeGetTypes(this Assembly assembly)
        {
#if NET_CF || SILVERLIGHT
            return assembly.GetTypes();
#else
            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException typeLoadException)
            {
                foreach (var ex in typeLoadException.LoaderExceptions)
                {
                    InternalLogger.Warn("Type load exception: {0}", ex);
                }

                var loadedTypes = new List<Type>();
                foreach (var t in typeLoadException.Types)
                {
                    if (t != null)
                    {
                        loadedTypes.Add(t);
                    }
                }

                return loadedTypes.ToArray();
            }
#endif
        }
    }
}