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

namespace Transformalize.Libs.NLog.Common
{
    /// <summary>
    ///     A cyclic buffer of <see cref="LogEventInfo" /> object.
    /// </summary>
    public class LogEventInfoBuffer
    {
        private readonly bool growAsNeeded;
        private readonly int growLimit;

        private AsyncLogEventInfo[] buffer;
        private int count;
        private int getPointer;
        private int putPointer;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogEventInfoBuffer" /> class.
        /// </summary>
        /// <param name="size">Buffer size.</param>
        /// <param name="growAsNeeded">Whether buffer should grow as it becomes full.</param>
        /// <param name="growLimit">The maximum number of items that the buffer can grow to.</param>
        public LogEventInfoBuffer(int size, bool growAsNeeded, int growLimit)
        {
            this.growAsNeeded = growAsNeeded;
            buffer = new AsyncLogEventInfo[size];
            this.growLimit = growLimit;
            getPointer = 0;
            putPointer = 0;
        }

        /// <summary>
        ///     Gets the number of items in the array.
        /// </summary>
        public int Size
        {
            get { return buffer.Length; }
        }

        /// <summary>
        ///     Adds the specified log event to the buffer.
        /// </summary>
        /// <param name="eventInfo">Log event.</param>
        /// <returns>The number of items in the buffer.</returns>
        public int Append(AsyncLogEventInfo eventInfo)
        {
            lock (this)
            {
                // make room for additional item
                if (count >= buffer.Length)
                {
                    if (growAsNeeded && buffer.Length < growLimit)
                    {
                        // create a new buffer, copy data from current
                        var newLength = buffer.Length*2;
                        if (newLength >= growLimit)
                        {
                            newLength = growLimit;
                        }

                        // InternalLogger.Trace("Enlarging LogEventInfoBuffer from {0} to {1}", this.buffer.Length, this.buffer.Length * 2);
                        var newBuffer = new AsyncLogEventInfo[newLength];
                        Array.Copy(buffer, 0, newBuffer, 0, buffer.Length);
                        buffer = newBuffer;
                    }
                    else
                    {
                        // lose the oldest item
                        getPointer = getPointer + 1;
                    }
                }

                // put the item
                putPointer = putPointer%buffer.Length;
                buffer[putPointer] = eventInfo;
                putPointer = putPointer + 1;
                count++;
                if (count >= buffer.Length)
                {
                    count = buffer.Length;
                }

                return count;
            }
        }

        /// <summary>
        ///     Gets the array of events accumulated in the buffer and clears the buffer as one atomic operation.
        /// </summary>
        /// <returns>Events in the buffer.</returns>
        public AsyncLogEventInfo[] GetEventsAndClear()
        {
            lock (this)
            {
                var cnt = count;
                var returnValue = new AsyncLogEventInfo[cnt];

                // InternalLogger.Trace("GetEventsAndClear({0},{1},{2})", this.getPointer, this.putPointer, this.count);
                for (var i = 0; i < cnt; ++i)
                {
                    var p = (getPointer + i)%buffer.Length;
                    var e = buffer[p];
                    buffer[p] = default(AsyncLogEventInfo); // we don't want memory leaks
                    returnValue[i] = e;
                }

                count = 0;
                getPointer = 0;
                putPointer = 0;

                return returnValue;
            }
        }
    }
}