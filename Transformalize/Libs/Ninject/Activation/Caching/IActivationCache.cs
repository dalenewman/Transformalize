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

using Transformalize.Libs.Ninject.Components;

namespace Transformalize.Libs.Ninject.Activation.Caching
{
    /// <summary>
    ///     Stores the objects that were activated
    /// </summary>
    public interface IActivationCache : INinjectComponent
    {
        /// <summary>
        ///     Clears the cache.
        /// </summary>
        void Clear();

        /// <summary>
        ///     Adds an activated instance.
        /// </summary>
        /// <param name="instance">The instance to be added.</param>
        void AddActivatedInstance(object instance);

        /// <summary>
        ///     Adds an deactivated instance.
        /// </summary>
        /// <param name="instance">The instance to be added.</param>
        void AddDeactivatedInstance(object instance);

        /// <summary>
        ///     Determines whether the specified instance is activated.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <returns>
        ///     <c>true</c> if the specified instance is activated; otherwise, <c>false</c>.
        /// </returns>
        bool IsActivated(object instance);

        /// <summary>
        ///     Determines whether the specified instance is deactivated.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <returns>
        ///     <c>true</c> if the specified instance is deactivated; otherwise, <c>false</c>.
        /// </returns>
        bool IsDeactivated(object instance);
    }
}