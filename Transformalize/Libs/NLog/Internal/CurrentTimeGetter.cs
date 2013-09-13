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