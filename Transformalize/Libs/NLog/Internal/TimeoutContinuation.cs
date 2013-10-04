#region License
// /*
// See license included in this library folder.
// */
#endregion

using System;
using System.Threading;
using Transformalize.Libs.NLog.Common;

namespace Transformalize.Libs.NLog.Internal
{
    /// <summary>
    ///     Wraps <see cref="AsyncContinuation" /> with a timeout.
    /// </summary>
    internal class TimeoutContinuation : IDisposable
    {
        private AsyncContinuation asyncContinuation;
        private Timer timeoutTimer;

        /// <summary>
        ///     Initializes a new instance of the <see cref="TimeoutContinuation" /> class.
        /// </summary>
        /// <param name="asyncContinuation">The asynchronous continuation.</param>
        /// <param name="timeout">The timeout.</param>
        public TimeoutContinuation(AsyncContinuation asyncContinuation, TimeSpan timeout)
        {
            this.asyncContinuation = asyncContinuation;
            timeoutTimer = new Timer(TimerElapsed, null, timeout, TimeSpan.FromMilliseconds(-1));
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            StopTimer();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Continuation function which implements the timeout logic.
        /// </summary>
        /// <param name="exception">The exception.</param>
        public void Function(Exception exception)
        {
            try
            {
                StopTimer();

                var cont = Interlocked.Exchange(ref asyncContinuation, null);
                if (cont != null)
                {
                    cont(exception);
                }
            }
            catch (Exception ex)
            {
                if (ex.MustBeRethrown())
                {
                    throw;
                }

                ReportExceptionInHandler(ex);
            }
        }

        private static void ReportExceptionInHandler(Exception exception)
        {
            InternalLogger.Error("Exception in asynchronous handler {0}", exception);
        }

        private void StopTimer()
        {
            lock (this)
            {
                if (timeoutTimer != null)
                {
                    timeoutTimer.Dispose();
                    timeoutTimer = null;
                }
            }
        }

        private void TimerElapsed(object state)
        {
            Function(new TimeoutException("Timeout."));
        }
    }
}