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
using System.Runtime.CompilerServices;

namespace Transformalize.Libs.Ninject.Infrastructure
{
#if SILVERLIGHT
    using WeakReference = BaseWeakReference;
#endif

    /// <summary>
    ///     Weak reference that can be used in collections. It is equal to the
    ///     object it references and has the same hash code.
    /// </summary>
    public class ReferenceEqualWeakReference : WeakReference
    {
        private readonly int cashedHashCode;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ReferenceEqualWeakReference" /> class.
        /// </summary>
        /// <param name="target">The target.</param>
        public ReferenceEqualWeakReference(object target) : base(target)
        {
#if !NETCF
            cashedHashCode = RuntimeHelpers.GetHashCode(target);
#else
            this.cashedHashCode = target.GetHashCode();
#endif
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ReferenceEqualWeakReference" /> class.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="trackResurrection">
        ///     if set to <c>true</c> [track resurrection].
        /// </param>
        public ReferenceEqualWeakReference(object target, bool trackResurrection) : base(target, trackResurrection)
        {
#if !NETCF
            cashedHashCode = RuntimeHelpers.GetHashCode(target);
#else
            this.cashedHashCode = target.GetHashCode();
#endif
        }

        /// <summary>
        ///     Determines whether the specified <see cref="object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">
        ///     The <see cref="object" /> to compare with this instance.
        /// </param>
        /// <returns>
        ///     <c>true</c> if the specified <see cref="object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="T:System.NullReferenceException">
        ///     The <paramref name="obj" /> parameter is null.
        /// </exception>
        public override bool Equals(object obj)
        {
            var thisInstance = IsAlive ? Target : this;

            var referenceEqualWeakReference = obj as WeakReference;
            if (referenceEqualWeakReference != null && referenceEqualWeakReference.IsAlive)
            {
                obj = referenceEqualWeakReference.Target;
            }

            return ReferenceEquals(thisInstance, obj);
        }

        /// <summary>
        ///     Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        ///     A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            return cashedHashCode;
        }
    }
}