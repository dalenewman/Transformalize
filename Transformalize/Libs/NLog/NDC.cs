#region License
// /*
// See license included in this library folder.
// */
#endregion

using System;

namespace Transformalize.Libs.NLog
{
    /// <summary>
    ///     Nested Diagnostics Context - for log4net compatibility.
    /// </summary>
    [Obsolete("Use NestedDiagnosticsContext")]
    public static class NDC
    {
        /// <summary>
        ///     Gets the top NDC message but doesn't remove it.
        /// </summary>
        /// <returns>The top message. .</returns>
        public static string TopMessage
        {
            get { return NestedDiagnosticsContext.TopMessage; }
        }

        /// <summary>
        ///     Pushes the specified text on current thread NDC.
        /// </summary>
        /// <param name="text">The text to be pushed.</param>
        /// <returns>An instance of the object that implements IDisposable that returns the stack to the previous level when IDisposable.Dispose() is called. To be used with C# using() statement.</returns>
        public static IDisposable Push(string text)
        {
            return NestedDiagnosticsContext.Push(text);
        }

        /// <summary>
        ///     Pops the top message off the NDC stack.
        /// </summary>
        /// <returns>The top message which is no longer on the stack.</returns>
        public static string Pop()
        {
            return NestedDiagnosticsContext.Pop();
        }

        /// <summary>
        ///     Clears current thread NDC stack.
        /// </summary>
        public static void Clear()
        {
            NestedDiagnosticsContext.Clear();
        }

        /// <summary>
        ///     Gets all messages on the stack.
        /// </summary>
        /// <returns>Array of strings on the stack.</returns>
        public static string[] GetAllMessages()
        {
            return NestedDiagnosticsContext.GetAllMessages();
        }
    }
}