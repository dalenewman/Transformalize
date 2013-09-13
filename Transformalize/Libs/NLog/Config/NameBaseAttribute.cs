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
using Transformalize.Libs.NLog.LayoutRenderers;
using Transformalize.Libs.NLog.Layouts;
using Transformalize.Libs.NLog.Targets;

namespace Transformalize.Libs.NLog.Config
{
    /// <summary>
    ///     Attaches a simple name to an item (such as <see cref="Target" />,
    ///     <see cref="LayoutRenderer" />, <see cref="Layout" />, etc.).
    /// </summary>
    public abstract class NameBaseAttribute : Attribute
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="NameBaseAttribute" /> class.
        /// </summary>
        /// <param name="name">The name of the item.</param>
        protected NameBaseAttribute(string name)
        {
            Name = name;
        }

        /// <summary>
        ///     Gets the name of the item.
        /// </summary>
        /// <value>The name of the item.</value>
        public string Name { get; private set; }
    }
}