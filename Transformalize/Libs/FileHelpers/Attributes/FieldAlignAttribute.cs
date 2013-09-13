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
using Transformalize.Libs.FileHelpers.Enums;

namespace Transformalize.Libs.FileHelpers.Attributes
{
    /// <summary>
    ///     Indicates the <see cref="AlignMode" /> used for <b>write</b> operations.
    /// </summary>
    /// <remarks>
    ///     See the <a href="attributes.html">Complete Attributes List</a> for more clear info and examples of each one.
    /// </remarks>
    /// <seealso href="attributes.html">Attributes List</seealso>
    /// <seealso href="quick_start.html">Quick Start Guide</seealso>
    /// <seealso href="examples.html">Examples of Use</seealso>
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class FieldAlignAttribute : Attribute
    {
        #region "  Constructors  "

        /// <summary>Uses the ' ' char to align.</summary>
        /// <param name="align">The position of the alignment.</param>
        public FieldAlignAttribute(AlignMode align) : this(align, ' ')
        {
        }

        /// <summary>You can indicate the align char.</summary>
        /// <param name="align">The position of the alignment.</param>
        /// <param name="alignChar">The character used to align.</param>
        public FieldAlignAttribute(AlignMode align, char alignChar)
        {
            Align = align;
            AlignChar = alignChar;
        }

        #endregion

        /// <summary>The position of the alignment.</summary>
        internal AlignMode Align;

        /// <summary>The character used to align.</summary>
        internal char AlignChar;
    }
}