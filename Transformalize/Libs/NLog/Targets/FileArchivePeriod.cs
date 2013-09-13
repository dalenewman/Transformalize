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


#if !SILVERLIGHT2 && !SILVERLIGHT3 && !WINDOWS_PHONE

namespace Transformalize.Libs.NLog.Targets
{
    /// <summary>
    ///     Modes of archiving files based on time.
    /// </summary>
    public enum FileArchivePeriod
    {
        /// <summary>
        ///     Don't archive based on time.
        /// </summary>
        None,

        /// <summary>
        ///     Archive every year.
        /// </summary>
        Year,

        /// <summary>
        ///     Archive every month.
        /// </summary>
        Month,

        /// <summary>
        ///     Archive daily.
        /// </summary>
        Day,

        /// <summary>
        ///     Archive every hour.
        /// </summary>
        Hour,

        /// <summary>
        ///     Archive every minute.
        /// </summary>
        Minute
    }
}

#endif