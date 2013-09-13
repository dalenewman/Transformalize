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
    /// <summary>Indicates that this class represents a fixed length record.</summary>
    /// <remarks>
    ///     See the <a href="attributes.html">Complete Attributes List</a> for more clear info and examples of each one.
    /// </remarks>
    /// <seealso href="attributes.html">Attributes List</seealso>
    /// <seealso href="quick_start.html">Quick Start Guide</seealso>
    /// <seealso href="examples.html">Examples of Use</seealso>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class FixedLengthRecordAttribute : TypedRecordAttribute
    {
        internal FixedMode mFixedMode = FixedMode.ExactLength;

        /// <summary>Indicates that this class represents a fixed length record. By default requieres that the records has the length equals to the sum of each field length.</summary>
        public FixedLengthRecordAttribute()
        {
        }

        /// <summary>Indicates that this class represents a fixed length record with the specified variable record behavior.</summary>
        /// <param name="mode">
        ///     The <see cref="FixedMode" /> used for variable length records.
        /// </param>
        public FixedLengthRecordAttribute(FixedMode mode)
        {
            mFixedMode = mode;
        }
    }
}