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

namespace Transformalize.Libs.NLog.Config
{
    /// <summary>
    ///     Used to mark configurable parameters which are arrays.
    ///     Specifies the mapping between XML elements and .NET types.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class ArrayParameterAttribute : Attribute
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ArrayParameterAttribute" /> class.
        /// </summary>
        /// <param name="itemType">The type of the array item.</param>
        /// <param name="elementName">The XML element name that represents the item.</param>
        public ArrayParameterAttribute(Type itemType, string elementName)
        {
            ItemType = itemType;
            ElementName = elementName;
        }

        /// <summary>
        ///     Gets the .NET type of the array item.
        /// </summary>
        public Type ItemType { get; private set; }

        /// <summary>
        ///     Gets the XML element name.
        /// </summary>
        public string ElementName { get; private set; }
    }
}