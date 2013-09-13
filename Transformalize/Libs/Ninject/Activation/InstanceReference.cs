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

#endregion

namespace Transformalize.Libs.Ninject.Activation
{
    /// <summary>
    ///     Holds an instance during activation or after it has been cached.
    /// </summary>
    public class InstanceReference
    {
        /// <summary>
        ///     Gets or sets the instance.
        /// </summary>
        public object Instance { get; set; }

        /// <summary>
        ///     Returns a value indicating whether the instance is of the specified type.
        /// </summary>
        /// <typeparam name="T">The type in question.</typeparam>
        /// <returns>
        ///     <see langword="True" /> if the instance is of the specified type, otherwise <see langword="false" />.
        /// </returns>
        public bool Is<T>()
        {
            return Instance is T;
        }

        /// <summary>
        ///     Returns the instance as the specified type.
        /// </summary>
        /// <typeparam name="T">The requested type.</typeparam>
        /// <returns>The instance.</returns>
        public T As<T>()
        {
            return (T) Instance;
        }

        /// <summary>
        ///     Executes the specified action if the instance if of the specified type.
        /// </summary>
        /// <typeparam name="T">The type in question.</typeparam>
        /// <param name="action">The action to execute.</param>
        public void IfInstanceIs<T>(Action<T> action)
        {
            if (Instance is T)
                action((T) Instance);
        }
    }
}