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
    /// <summary>Indicates that the engine must ignore the empty lines while reading.</summary>
    /// <remarks>
    ///     See the <a href="attributes.html">Complete Attributes List</a> for more clear info and examples of each one.
    /// </remarks>
    /// <seealso href="attributes.html">Attributes List</seealso>
    /// <seealso href="quick_start.html">Quick Start Guide</seealso>
    /// <seealso href="examples.html">Examples of Use</seealso>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class IgnoreEmptyLinesAttribute : Attribute
    {
        internal bool mIgnoreSpaces = false;

        /// <summary>Indicates that the engine must ignore the empty lines while reading.</summary>
        public IgnoreEmptyLinesAttribute()
        {
        }

        /// <summary>Indicates that the engine must ignore the empty lines while reading.</summary>
        /// <param name="ignoreSpaces">Indicates if also must ignore lines with spaces.</param>
        public IgnoreEmptyLinesAttribute(bool ignoreSpaces)
        {
            mIgnoreSpaces = ignoreSpaces;
        }
    }
}