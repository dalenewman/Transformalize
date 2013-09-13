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

using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Transformalize.Libs.Ninject.Infrastructure;

namespace Transformalize.Libs.Ninject.Activation.Caching
{
    /// <summary>
    ///     Compares ReferenceEqualWeakReferences to objects
    /// </summary>
    public class WeakReferenceEqualityComparer : IEqualityComparer<object>
    {
        /// <summary>
        ///     Returns if the specifed objects are equal.
        /// </summary>
        /// <param name="x">The first object.</param>
        /// <param name="y">The second object.</param>
        /// <returns>True if the objects are equal; otherwise false</returns>
        public new bool Equals(object x, object y)
        {
            return x.Equals(y);
        }

        /// <summary>
        ///     Returns the hash code of the specified object.
        /// </summary>
        /// <param name="obj">The object for which the hash code is calculated.</param>
        /// <returns>The hash code of the specified object.</returns>
        public int GetHashCode(object obj)
        {
            var weakReference = obj as ReferenceEqualWeakReference;
            return weakReference != null ? weakReference.GetHashCode() :
#if !NETCF
                       RuntimeHelpers.GetHashCode(obj);
#else
                obj.GetHashCode();
#endif
        }
    }
}