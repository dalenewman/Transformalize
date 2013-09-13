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
using Transformalize.Libs.NLog.Config;

namespace Transformalize.Libs.NLog.LayoutRenderers
{
    /// <summary>
    ///     Marks class as a layout renderer and assigns a format string to it.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class LayoutRendererAttribute : NameBaseAttribute
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="LayoutRendererAttribute" /> class.
        /// </summary>
        /// <param name="name">Name of the layout renderer.</param>
        public LayoutRendererAttribute(string name)
            : base(name)
        {
        }
    }
}