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
    /// <summary>The condition used to include or exclude each record.</summary>
    public enum RecordCondition
    {
        /// <summary>No Condition, Include it always.</summary>
        None = 0,

        /// <summary>Include the record if it contains the selector string.</summary>
        IncludeIfContains,

        /// <summary>Include the record if it begins with selector string.</summary>
        IncludeIfBegins,

        /// <summary>Include the record if it ends with selector string.</summary>
        IncludeIfEnds,

        /// <summary>Include the record if it begins and ends with selector string.</summary>
        IncludeIfEnclosed,
#if ! MINI
        /// <summary>Include the record if it matchs the regular expression passed as selector.</summary>
        IncludeIfMatchRegex,
#endif

        /// <summary>Exclude the record if it contains the selector string.</summary>
        ExcludeIfContains,

        /// <summary>Exclude the record if it begins with selector string.</summary>
        ExcludeIfBegins,

        /// <summary>Exclude the record if it ends with selector string.</summary>
        ExcludeIfEnds,

        /// <summary>Exclude the record if it begins and ends with selector string.</summary>
        ExcludeIfEnclosed
#if ! MINI
        ,

        /// <summary>Exclude the record if it matchs the regular expression passed as selector.</summary>
        ExcludeIfMatchRegex
#endif
    }
}