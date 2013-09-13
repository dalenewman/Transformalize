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

namespace Transformalize.Libs.NLog
{
    /// <summary>
    ///     Global Diagnostics Context - used for log4net compatibility.
    /// </summary>
    [Obsolete("Use GlobalDiagnosticsContext instead")]
    public static class GDC
    {
        /// <summary>
        ///     Sets the Global Diagnostics Context item to the specified value.
        /// </summary>
        /// <param name="item">Item name.</param>
        /// <param name="value">Item value.</param>
        public static void Set(string item, string value)
        {
            GlobalDiagnosticsContext.Set(item, value);
        }

        /// <summary>
        ///     Gets the Global Diagnostics Context named item.
        /// </summary>
        /// <param name="item">Item name.</param>
        /// <returns>The item value of string.Empty if the value is not present.</returns>
        public static string Get(string item)
        {
            return GlobalDiagnosticsContext.Get(item);
        }

        /// <summary>
        ///     Checks whether the specified item exists in the Global Diagnostics Context.
        /// </summary>
        /// <param name="item">Item name.</param>
        /// <returns>A boolean indicating whether the specified item exists in current thread GDC.</returns>
        public static bool Contains(string item)
        {
            return GlobalDiagnosticsContext.Contains(item);
        }

        /// <summary>
        ///     Removes the specified item from the Global Diagnostics Context.
        /// </summary>
        /// <param name="item">Item name.</param>
        public static void Remove(string item)
        {
            GlobalDiagnosticsContext.Remove(item);
        }

        /// <summary>
        ///     Clears the content of the GDC.
        /// </summary>
        public static void Clear()
        {
            GlobalDiagnosticsContext.Clear();
        }
    }
}