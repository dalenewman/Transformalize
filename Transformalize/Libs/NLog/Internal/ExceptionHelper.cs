#region License
// /*
// See license included in this library folder.
// */
#endregion

using System;
using System.Threading;

namespace Transformalize.Libs.NLog.Internal
{
    /// <summary>
    ///     Helper class for dealing with exceptions.
    /// </summary>
    internal static class ExceptionHelper
    {
        /// <summary>
        ///     Determines whether the exception must be rethrown.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <returns>True if the exception must be rethrown, false otherwise.</returns>
        public static bool MustBeRethrown(this Exception exception)
        {
            if (exception is StackOverflowException)
            {
                return true;
            }

            if (exception is ThreadAbortException)
            {
                return true;
            }

            if (exception is OutOfMemoryException)
            {
                return true;
            }

            return false;
        }
    }
}