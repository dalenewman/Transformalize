#region License
// /*
// See license included in this library folder.
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