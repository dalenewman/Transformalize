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
    ///     Implements a single-call guard around given continuation function.
    /// </summary>
    internal class SingleCallContinuation
    {
        private AsyncContinuation asyncContinuation;

        /// <summary>
        ///     Initializes a new instance of the <see cref="SingleCallContinuation" /> class.
        /// </summary>
        /// <param name="asyncContinuation">The asynchronous continuation.</param>
        public SingleCallContinuation(AsyncContinuation asyncContinuation)
        {
            this.asyncContinuation = asyncContinuation;
        }

        /// <summary>
        ///     Continuation function which implements the single-call guard.
        /// </summary>
        /// <param name="exception">The exception.</param>
        public void Function(Exception exception)
        {
            try
            {
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
    }
}