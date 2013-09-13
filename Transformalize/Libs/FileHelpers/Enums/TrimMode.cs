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

namespace Transformalize.Libs.FileHelpers.Enums
{
    /// <summary>Indicates the triming behavior of the trailing characters.</summary>
    public enum TrimMode
    {
        /// <summary>No triming is performed.</summary>
        None,

        /// <summary>The field is trimed in both sides.</summary>
        Both,

        /// <summary>The field is trimed in the left.</summary>
        Left,

        /// <summary>The field is trimed in the right.</summary>
        Right
    }
}