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

using System.ComponentModel;
using System.Threading;
using Transformalize.Libs.NLog.Common;

namespace Transformalize.Libs.NLog.Targets.Wrappers
{
    /// <summary>
    ///     A target that buffers log events and sends them in batches to the wrapped target.
    /// </summary>
    /// <seealso href="http://nlog-project.org/wiki/BufferingWrapper_target">Documentation on NLog Wiki</seealso>
    [Target("BufferingWrapper", IsWrapper = true)]
    public class BufferingTargetWrapper : WrapperTargetBase
    {
        private LogEventInfoBuffer buffer;
        private Timer flushTimer;

        /// <summary>
        ///     Initializes a new instance of the <see cref="BufferingTargetWrapper" /> class.
        /// </summary>
        public BufferingTargetWrapper()
            : this(null)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="BufferingTargetWrapper" /> class.
        /// </summary>
        /// <param name="wrappedTarget">The wrapped target.</param>
        public BufferingTargetWrapper(Target wrappedTarget)
            : this(wrappedTarget, 100)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="BufferingTargetWrapper" /> class.
        /// </summary>
        /// <param name="wrappedTarget">The wrapped target.</param>
        /// <param name="bufferSize">Size of the buffer.</param>
        public BufferingTargetWrapper(Target wrappedTarget, int bufferSize)
            : this(wrappedTarget, bufferSize, -1)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="BufferingTargetWrapper" /> class.
        /// </summary>
        /// <param name="wrappedTarget">The wrapped target.</param>
        /// <param name="bufferSize">Size of the buffer.</param>
        /// <param name="flushTimeout">The flush timeout.</param>
        public BufferingTargetWrapper(Target wrappedTarget, int bufferSize, int flushTimeout)
        {
            WrappedTarget = wrappedTarget;
            BufferSize = bufferSize;
            FlushTimeout = flushTimeout;
            SlidingTimeout = true;
        }

        /// <summary>
        ///     Gets or sets the number of log events to be buffered.
        /// </summary>
        /// <docgen category='Buffering Options' order='100' />
        [DefaultValue(100)]
        public int BufferSize { get; set; }

        /// <summary>
        ///     Gets or sets the timeout (in milliseconds) after which the contents of buffer will be flushed
        ///     if there's no write in the specified period of time. Use -1 to disable timed flushes.
        /// </summary>
        /// <docgen category='Buffering Options' order='100' />
        [DefaultValue(-1)]
        public int FlushTimeout { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether to use sliding timeout.
        /// </summary>
        /// <remarks>
        ///     This value determines how the inactivity period is determined. If sliding timeout is enabled,
        ///     the inactivity timer is reset after each write, if it is disabled - inactivity timer will
        ///     count from the first event written to the buffer.
        /// </remarks>
        /// <docgen category='Buffering Options' order='100' />
        [DefaultValue(true)]
        public bool SlidingTimeout { get; set; }

        /// <summary>
        ///     Flushes pending events in the buffer (if any).
        /// </summary>
        /// <param name="asyncContinuation">The asynchronous continuation.</param>
        protected override void FlushAsync(AsyncContinuation asyncContinuation)
        {
            var events = buffer.GetEventsAndClear();

            if (events.Length == 0)
            {
                WrappedTarget.Flush(asyncContinuation);
            }
            else
            {
                WrappedTarget.WriteAsyncLogEvents(events, ex => WrappedTarget.Flush(asyncContinuation));
            }
        }

        /// <summary>
        ///     Initializes the target.
        /// </summary>
        protected override void InitializeTarget()
        {
            base.InitializeTarget();
            buffer = new LogEventInfoBuffer(BufferSize, false, 0);
            flushTimer = new Timer(FlushCallback, null, -1, -1);
        }

        /// <summary>
        ///     Closes the target by flushing pending events in the buffer (if any).
        /// </summary>
        protected override void CloseTarget()
        {
            base.CloseTarget();
            if (flushTimer != null)
            {
                flushTimer.Dispose();
                flushTimer = null;
            }
        }

        /// <summary>
        ///     Adds the specified log event to the buffer and flushes
        ///     the buffer in case the buffer gets full.
        /// </summary>
        /// <param name="logEvent">The log event.</param>
        protected override void Write(AsyncLogEventInfo logEvent)
        {
            WrappedTarget.PrecalculateVolatileLayouts(logEvent.LogEvent);

            var count = buffer.Append(logEvent);
            if (count >= BufferSize)
            {
                var events = buffer.GetEventsAndClear();
                WrappedTarget.WriteAsyncLogEvents(events);
            }
            else
            {
                if (FlushTimeout > 0)
                {
                    // reset the timer on first item added to the buffer or whenever SlidingTimeout is set to true
                    if (SlidingTimeout || count == 1)
                    {
                        flushTimer.Change(FlushTimeout, -1);
                    }
                }
            }
        }

        private void FlushCallback(object state)
        {
            lock (SyncRoot)
            {
                if (IsInitialized)
                {
                    var events = buffer.GetEventsAndClear();
                    if (events.Length > 0)
                    {
                        WrappedTarget.WriteAsyncLogEvents(events);
                    }
                }
            }
        }
    }
}