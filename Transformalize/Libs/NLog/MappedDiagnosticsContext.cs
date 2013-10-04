#region License
// /*
// See license included in this library folder.
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