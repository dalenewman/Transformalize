#region License
// /*
// See license included in this library folder.
// */
#endregion

using System.Collections.Generic;

namespace Transformalize.Libs.NLog
{
    /// <summary>
    ///     Global Diagnostics Context - a dictionary structure to hold per-application-instance values.
    /// </summary>
    public static class GlobalDiagnosticsContext
    {
        private static readonly Dictionary<string, string> dict = new Dictionary<string, string>();

        /// <summary>
        ///     Sets the Global Diagnostics Context item to the specified value.
        /// </summary>
        /// <param name="item">Item name.</param>
        /// <param name="value">Item value.</param>
        public static void Set(string item, string value)
        {
            lock (dict)
            {
                dict[item] = value;
            }
        }

        /// <summary>
        ///     Gets the Global Diagnostics Context named item.
        /// </summary>
        /// <param name="item">Item name.</param>
        /// <returns>The item value of string.Empty if the value is not present.</returns>
        public static string Get(string item)
        {
            lock (dict)
            {
                string s;

                if (!dict.TryGetValue(item, out s))
                {
                    s = string.Empty;
                }

                return s;
            }
        }

        /// <summary>
        ///     Checks whether the specified item exists in the Global Diagnostics Context.
        /// </summary>
        /// <param name="item">Item name.</param>
        /// <returns>A boolean indicating whether the specified item exists in current thread GDC.</returns>
        public static bool Contains(string item)
        {
            lock (dict)
            {
                return dict.ContainsKey(item);
            }
        }

        /// <summary>
        ///     Removes the specified item from the Global Diagnostics Context.
        /// </summary>
        /// <param name="item">Item name.</param>
        public static void Remove(string item)
        {
            lock (dict)
            {
                dict.Remove(item);
            }
        }

        /// <summary>
        ///     Clears the content of the GDC.
        /// </summary>
        public static void Clear()
        {
            lock (dict)
            {
                dict.Clear();
            }
        }
    }
}