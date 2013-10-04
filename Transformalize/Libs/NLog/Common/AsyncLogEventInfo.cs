#region License
// /*
// See license included in this library folder.
// */
#endregion
namespace Transformalize.Libs.NLog.Common
{
    /// <summary>
    ///     Represents the logging event with asynchronous continuation.
    /// </summary>
    public struct AsyncLogEventInfo
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="AsyncLogEventInfo" /> struct.
        /// </summary>
        /// <param name="logEvent">The log event.</param>
        /// <param name="continuation">The continuation.</param>
        public AsyncLogEventInfo(LogEventInfo logEvent, AsyncContinuation continuation)
            : this()
        {
            LogEvent = logEvent;
            Continuation = continuation;
        }

        /// <summary>
        ///     Gets the log event.
        /// </summary>
        public LogEventInfo LogEvent { get; private set; }

        /// <summary>
        ///     Gets the continuation.
        /// </summary>
        public AsyncContinuation Continuation { get; internal set; }

        /// <summary>
        ///     Implements the operator ==.
        /// </summary>
        /// <param name="eventInfo1">The event info1.</param>
        /// <param name="eventInfo2">The event info2.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(AsyncLogEventInfo eventInfo1, AsyncLogEventInfo eventInfo2)
        {
            return ReferenceEquals(eventInfo1.Continuation, eventInfo2.Continuation)
                   && ReferenceEquals(eventInfo1.LogEvent, eventInfo2.LogEvent);
        }

        /// <summary>
        ///     Implements the operator ==.
        /// </summary>
        /// <param name="eventInfo1">The event info1.</param>
        /// <param name="eventInfo2">The event info2.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(AsyncLogEventInfo eventInfo1, AsyncLogEventInfo eventInfo2)
        {
            return !ReferenceEquals(eventInfo1.Continuation, eventInfo2.Continuation)
                   || !ReferenceEquals(eventInfo1.LogEvent, eventInfo2.LogEvent);
        }

        /// <summary>
        ///     Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">
        ///     The <see cref="System.Object" /> to compare with this instance.
        /// </param>
        /// <returns>
        ///     A value of <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            var other = (AsyncLogEventInfo) obj;
            return this == other;
        }

        /// <summary>
        ///     Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        ///     A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            return LogEvent.GetHashCode() ^ Continuation.GetHashCode();
        }
    }
}