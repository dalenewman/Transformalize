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

namespace Transformalize.Libs.Ninject.Infrastructure
{
    /// <summary>
    ///     Inheritable weak reference base class for Silverlight
    /// </summary>
    public abstract class BaseWeakReference
    {
        private readonly WeakReference innerWeakReference;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ReferenceEqualWeakReference" /> class.
        /// </summary>
        /// <param name="target">The target.</param>
        protected BaseWeakReference(object target)
        {
            innerWeakReference = new WeakReference(target);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ReferenceEqualWeakReference" /> class.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="trackResurrection">
        ///     if set to <c>true</c> [track resurrection].
        /// </param>
        protected BaseWeakReference(object target, bool trackResurrection)
        {
            innerWeakReference = new WeakReference(target, trackResurrection);
        }

        /// <summary>
        ///     Gets a value indicating whether this instance is alive.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is alive; otherwise, <c>false</c>.
        /// </value>
        public bool IsAlive
        {
            get { return innerWeakReference.IsAlive; }
        }

        /// <summary>
        ///     Gets or sets the target of this weak reference.
        /// </summary>
        /// <value>The target of this weak reference.</value>
        public object Target
        {
            get { return innerWeakReference.Target; }

            set { innerWeakReference.Target = value; }
        }
    }
}