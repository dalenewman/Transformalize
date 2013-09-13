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

namespace Transformalize.Libs.Rhino.Etl.DataReaders
{
    /// <summary>
    ///     Represent a virtual property, with a type and name.
    ///     It also exposes the ability to get the "property" from a container.
    /// </summary>
    /// <remarks>
    ///     This is needed because we want to use both types and untyped containers.
    ///     Those can be entities, hashtables, etc.
    /// </remarks>
    public abstract class Descriptor
    {
        /// <summary>
        ///     The name of this descriptor
        /// </summary>
        public string Name;

        /// <summary>
        ///     The type fo this descriptor
        /// </summary>
        public Type Type;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Descriptor" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="type">The type.</param>
        public Descriptor(string name, Type type)
        {
            Name = name;
            Type = type;
        }

        /// <summary>
        ///     Gets the value from the container
        /// </summary>
        /// <param name="container">The container.</param>
        public abstract object GetValue(object container);
    }
}