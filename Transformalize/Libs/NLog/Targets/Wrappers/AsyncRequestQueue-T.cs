#region License
// /*
// See license included in this library folder.
// */
#endregion

using System.Collections.Generic;
using System.Threading;
using Transformalize.Libs.NLog.Common;

namespace Transformalize.Libs.NLog.Targets.Wrappers
{
    /// <summary>
    ///     Asynchronous request queue.
    /// </summary>
    internal class AsyncRequestQueue
    {
        private readonly Queue<AsyncLogEventInfo> logEventInfoQueue = new Queue<AsyncLogEventInfo>();

        /// <summary>
        ///     Initializes a new instance of the AsyncRequestQueue class.
        /// </summary>
        /// <param name="requestLimit">Request limit.</param>
        /// <param name="overflowAction">The overflow action.</param>
        public AsyncRequestQueue(int requestLimit, AsyncTargetWrapperOverflowAction overflowAction)
        {
            RequestLimit = requestLimit;
            OnOverflow = overflowAction;
        }

        /// <summary>
        ///     Gets or sets the request limit.
        /// </summary>
        public int RequestLimit { get; set; }

        /// <summary>
        ///     Gets or sets the action to be taken when there's no more room in
        ///     the queue and another request is enqueued.
        /// </summary>
        public AsyncTargetWrapperOverflowAction OnOverflow { get; set; }

        /// <summary>
        ///     Gets the number of requests currently in the queue.
        /// </summary>
        public int RequestCount
        {
            get { return logEventInfoQueue.Count; }
        }

        /// <summary>
        ///     Enqueues another item. If the queue is overflown the appropriate
        ///     action is taken as specified by <see cref="OnOverflow" />.
        /// </summary>
        /// <param name="logEventInfo">The log event info.</param>
        public void Enqueue(AsyncLogEventInfo logEventInfo)
        {
            lock (this)
            {
                if (logEventInfoQueue.Count >= RequestLimit)
                {
                    switch (OnOverflow)
                    {
                        case AsyncTargetWrapperOverflowAction.Discard:
                            // dequeue and discard one element
                            logEventInfoQueue.Dequeue();
                            break;

                        case AsyncTargetWrapperOverflowAction.Grow:
                            break;

#if !NET_CF
                        case AsyncTargetWrapperOverflowAction.Block:
                            while (logEventInfoQueue.Count >= RequestLimit)
                            {
                                InternalLogger.Trace("Blocking...");
                                Monitor.Wait(this);
                                InternalLogger.Trace("Entered critical section.");
                            }

                            InternalLogger.Trace("Limit ok.");
                            break;
#endif
                    }
                }

                logEventInfoQueue.Enqueue(logEventInfo);
            }
        }

        /// <summary>
        ///     Dequeues a maximum of <c>count</c> items from the queue
        ///     and adds returns the list containing them.
        /// </summary>
        /// <param name="count">Maximum number of items to be dequeued.</param>
        /// <returns>The array of log events.</returns>
        public AsyncLogEventInfo[] DequeueBatch(int count)
        {
            var resultEvents = new List<AsyncLogEventInfo>();

            lock (this)
            {
                for (var i = 0; i < count; ++i)
                {
                    if (logEventInfoQueue.Count <= 0)
                    {
                        break;
                    }

                    resultEvents.Add(logEventInfoQueue.Dequeue());
                }
#if !NET_CF
                if (OnOverflow == AsyncTargetWrapperOverflowAction.Block)
                {
                    Monitor.PulseAll(this);
                }
#endif
            }

            return resultEvents.ToArray();
        }

        /// <summary>
        ///     Clears the queue.
        /// </summary>
        public void Clear()
        {
            lock (this)
            {
                logEventInfoQueue.Clear();
            }
        }
    }
}