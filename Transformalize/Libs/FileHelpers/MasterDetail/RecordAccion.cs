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

namespace Transformalize.Libs.FileHelpers.MasterDetail
{
    /// <summary>
    ///     Used with the <see cref="MasterDetailSelector" /> Delegate to determines the action used by the
    ///     <see
    ///         cref="MasterDetailEngine" />
    /// </summary>
    public enum RecordAction
    {
        /// <summary>
        ///     <b>Ignore</b> the current record.
        /// </summary>
        Skip = 0,

        /// <summary>
        ///     Use the current record as <b>Master</b>.
        /// </summary>
        Master,

        /// <summary>
        ///     Use the current record as <b>Detail</b>.
        /// </summary>
        Detail
    }
}