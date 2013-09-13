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