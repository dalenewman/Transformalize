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

using Transformalize.Libs.Ninject.Components;

#endregion

namespace Transformalize.Libs.Ninject.Activation.Caching
{
    /// <summary>
    ///     Tracks instances for re-use in certain scopes.
    /// </summary>
    public interface ICache : INinjectComponent, IPruneable
    {
        /// <summary>
        ///     Gets the number of entries currently stored in the cache.
        /// </summary>
        int Count { get; }

        /// <summary>
        ///     Stores the specified instance in the cache.
        /// </summary>
        /// <param name="context">The context to store.</param>
        /// <param name="reference">The instance reference.</param>
        void Remember(IContext context, InstanceReference reference);

        /// <summary>
        ///     Tries to retrieve an instance to re-use in the specified context.
        /// </summary>
        /// <param name="context">The context that is being activated.</param>
        /// <returns>
        ///     The instance for re-use, or <see langword="null" /> if none has been stored.
        /// </returns>
        object TryGet(IContext context);

        /// <summary>
        ///     Deactivates and releases the specified instance from the cache.
        /// </summary>
        /// <param name="instance">The instance to release.</param>
        /// <returns>
        ///     <see langword="True" /> if the instance was found and released; otherwise <see langword="false" />.
        /// </returns>
        bool Release(object instance);

        /// <summary>
        ///     Immediately deactivates and removes all instances in the cache that are owned by
        ///     the specified scope.
        /// </summary>
        /// <param name="scope">The scope whose instances should be deactivated.</param>
        void Clear(object scope);

        /// <summary>
        ///     Immediately deactivates and removes all instances in the cache, regardless of scope.
        /// </summary>
        void Clear();
    }
}