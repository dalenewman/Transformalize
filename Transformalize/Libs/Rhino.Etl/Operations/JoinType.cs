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

namespace Transformalize.Libs.Rhino.Etl.Operations
{
    /// <summary>
    ///     Define the supported join types
    /// </summary>
    [Flags]
    public enum JoinType
    {
        /// <summary>
        ///     Inner join
        /// </summary>
        Inner = 0,

        /// <summary>
        ///     Left outer join
        /// </summary>
        Left = 1,

        /// <summary>
        ///     Right outer join
        /// </summary>
        Right = 2,

        /// <summary>
        ///     Full outer join
        /// </summary>
        Full = Left | Right
    }
}