#region License
// /*
// See license included in this library folder.
// */
#endregion

using System;

namespace Transformalize.Libs.NLog.Internal
{
    /// <summary>
    ///     Optimized methods to get current time.
    /// </summary>
    internal class CurrentTimeGetter
    {
        private static int lastTicks = -1;
        private static DateTime lastDateTime = DateTime.MinValue;

        /// <summary>
        ///     Gets the current time in an optimized fashion.
        /// </summary>
        /// <value>Current time.</value>
        public static DateTime Now
        {
            get
            {
                var tickCount = Environment.TickCount;
                if (tickCount == lastTicks)
                {
                    return lastDateTime;
                }

                var dt = DateTime.Now;

                lastTicks = tickCount;
                lastDateTime = dt;
                return dt;
            }
        }
    }
}