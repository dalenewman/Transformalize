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

    #region "  Delegate  "

    /// <summary>
    ///     Delegate thats determines the Type of the current record (Master, Detail, Skip)
    /// </summary>
    /// <param name="recordString">The string of the current record.</param>
    /// <returns>the action used for the current record (Master, Detail, Skip)</returns>
    public delegate RecordAction MasterDetailSelector(string recordString);

    #endregion

    #region "  Common Actions and Selector  "

    /// <summary>The Action taken when the selector string is found.</summary>
    public enum CommonSelector
    {
        /// <summary>
        ///     Parse the current record as <b>Master</b> if the selector string is found.
        /// </summary>
        MasterIfContains,

        /// <summary>
        ///     Parse the current record as <b>Master</b> if the record starts with some string.
        /// </summary>
        MasterIfBegins,

        /// <summary>
        ///     Parse the current record as <b>Master</b> if the record ends with some string.
        /// </summary>
        MasterIfEnds,

        /// <summary>
        ///     Parse the current record as <b>Master</b> if the record begins and ends with some string.
        /// </summary>
        MasterIfEnclosed,

        /// <summary>
        ///     Parse the current record as <b>Detail</b> if the selector string is found.
        /// </summary>
        DetailIfContains,

        /// <summary>
        ///     Parse the current record as <b>Detail</b> if the record starts with some string.
        /// </summary>
        DetailIfBegins,

        /// <summary>
        ///     Parse the current record as <b>Detail</b> if the record ends with some string.
        /// </summary>
        DetailIfEnds,

        /// <summary>
        ///     Parse the current record as <b>Detail</b> if the record begins and ends with some string.
        /// </summary>
        DetailIfEnclosed
    }

    #endregion
}