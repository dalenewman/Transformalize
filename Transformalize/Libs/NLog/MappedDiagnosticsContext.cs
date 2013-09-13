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

using System.Collections.Generic;
using Transformalize.Libs.NLog.Internal;

namespace Transformalize.Libs.NLog
{
    /// <summary>
    ///     Mapped Diagnostics Context - a thread-local structure that keeps a dictionary
    ///     of strings and provides methods to output them in layouts.
    ///     Mostly for compatibility with log4net.
    /// </summary>
    public static class MappedDiagnosticsContext
    {
        private static readonly object dataSlot = ThreadLocalStorageHelper.AllocateDataSlot();

        internal static IDictionary<string, string> ThreadDictionary
        {
            get { return ThreadLocalStorageHelper.GetDataForSlot<Dictionary<string, string>>(dataSlot); }
        }

        /// <summary>
        ///     Sets the current thread MDC item to the specified value.
        /// </summary>
        /// <param name="item">Item name.</param>
        /// <param name="value">Item value.</param>
        public static void Set(string item, string value)
        {
            ThreadDictionary[item] = value;
        }

        /// <summary>
        ///     Gets the current thread MDC named item.
        /// </summary>
        /// <param name="item">Item name.</param>
        /// <returns>The item value of string.Empty if the value is not present.</returns>
        public static string Get(string item)
        {
            string s;

            if (!ThreadDictionary.TryGetValue(item, out s))
            {
                s = string.Empty;
            }

            return s;
        }

        /// <summary>
        ///     Checks whether the specified item exists in current thread MDC.
        /// </summary>
        /// <param name="item">Item name.</param>
        /// <returns>A boolean indicating whether the specified item exists in current thread MDC.</returns>
        public static bool Contains(string item)
        {
            return ThreadDictionary.ContainsKey(item);
        }

        /// <summary>
        ///     Removes the specified item from current thread MDC.
        /// </summary>
        /// <param name="item">Item name.</param>
        public static void Remove(string item)
        {
            ThreadDictionary.Remove(item);
        }

        /// <summary>
        ///     Clears the content of current thread MDC.
        /// </summary>
        public static void Clear()
        {
            ThreadDictionary.Clear();
        }
    }
}