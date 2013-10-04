#region License
// /*
// See license included in this library folder.
// */
#endregion

using System;

namespace Transformalize.Libs.NLog
{
    /// <summary>
    ///     Mapped Diagnostics Context - used for log4net compatibility.
    /// </summary>
    [Obsolete("Use MappedDiagnosticsContext instead")]
    public static class MDC
    {
        /// <summary>
        ///     Sets the current thread MDC item to the specified value.
        /// </summary>
        /// <param name="item">Item name.</param>
        /// <param name="value">Item value.</param>
        public static void Set(string item, string value)
        {
            MappedDiagnosticsContext.Set(item, value);
        }

        /// <summary>
        ///     Gets the current thread MDC named item.
        /// </summary>
        /// <param name="item">Item name.</param>
        /// <returns>The item value of string.Empty if the value is not present.</returns>
        public static string Get(string item)
        {
            return MappedDiagnosticsContext.Get(item);
        }

        /// <summary>
        ///     Checks whether the specified item exists in current thread MDC.
        /// </summary>
        /// <param name="item">Item name.</param>
        /// <returns>A boolean indicating whether the specified item exists in current thread MDC.</returns>
        public static bool Contains(string item)
        {
            return MappedDiagnosticsContext.Contains(item);
        }

        /// <summary>
        ///     Removes the specified item from current thread MDC.
        /// </summary>
        /// <param name="item">Item name.</param>
        public static void Remove(string item)
        {
            MappedDiagnosticsContext.Remove(item);
        }

        /// <summary>
        ///     Clears the content of current thread MDC.
        /// </summary>
        public static void Clear()
        {
            MappedDiagnosticsContext.Clear();
        }
    }
}