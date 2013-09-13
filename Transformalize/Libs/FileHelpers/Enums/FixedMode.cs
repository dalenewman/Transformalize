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

using Transformalize.Libs.FileHelpers.Attributes;

namespace Transformalize.Libs.FileHelpers.Enums
{
    /// <summary>
    ///     Indicates the behavior when variable length records are found in a [<see cref="FixedLengthRecordAttribute" />]. (Note: nothing in common with [FieldOptional])
    /// </summary>
    public enum FixedMode
    {
        /// <summary>The records must have the length equals to the sum of each field length.</summary>
        ExactLength = 0,

        /// <summary>The records can contain less chars in the last field.</summary>
        AllowMoreChars,

        /// <summary>The records can contain more chars in the last field.</summary>
        AllowLessChars,

        /// <summary>The records can contain more or less chars in the last field.</summary>
        AllowVariableLength
    }
}