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

namespace Transformalize.Libs.FileHelpers.Attributes
{
    /// <summary>Indicates the number of lines to be discarded at the end.</summary>
    /// <remarks>
    ///     See the <a href="attributes.html">Complete Attributes List</a> for more clear info and examples of each one.
    /// </remarks>
    /// <seealso href="attributes.html">Attributes List</seealso>
    /// <seealso href="quick_start.html">Quick Start Guide</seealso>
    /// <seealso href="examples.html">Examples of Use</seealso>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class IgnoreLastAttribute : Attribute
    {
        internal int NumberOfLines;

        /// <summary>Indicates that the last line must be discarded.</summary>
        public IgnoreLastAttribute() : this(1)
        {
        }

        /// <summary>Indicates the number of last lines to be ignored at the end.</summary>
        /// <param name="numberOfLines">The number of lines to be discarded at end.</param>
        public IgnoreLastAttribute(int numberOfLines)
        {
            NumberOfLines = numberOfLines;
        }
    }
}