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

namespace Transformalize.Libs.NLog.Filters
{
    /// <summary>
    ///     Filter result.
    /// </summary>
    public enum FilterResult
    {
        /// <summary>
        ///     The filter doesn't want to decide whether to log or discard the message.
        /// </summary>
        Neutral,

        /// <summary>
        ///     The message should be logged.
        /// </summary>
        Log,

        /// <summary>
        ///     The message should not be logged.
        /// </summary>
        Ignore,

        /// <summary>
        ///     The message should be logged and processing should be finished.
        /// </summary>
        LogFinal,

        /// <summary>
        ///     The message should not be logged and processing should be finished.
        /// </summary>
        IgnoreFinal,
    }
}