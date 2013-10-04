#region License
// /*
// See license included in this library folder.
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