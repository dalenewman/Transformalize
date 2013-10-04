#region License
// /*
// See license included in this library folder.
// */
#endregion

using System;
using System.Collections.Generic;
using Transformalize.Libs.NLog.Internal;

namespace Transformalize.Libs.NLog
{
    /// <summary>
    ///     Nested Diagnostics Context - a thread-local structure that keeps a stack
    ///     of strings and provides methods to output them in layouts
    ///     Mostly for compatibility with log4net.
    /// </summary>
    public static class NestedDiagnosticsContext
    {
        private static readonly object dataSlot = ThreadLocalStorageHelper.AllocateDataSlot();

        /// <summary>
        ///     Gets the top NDC message but doesn't remove it.
        /// </summary>
        /// <returns>The top message. .</returns>
        public static string TopMessage
        {
            get
            {
                var stack = ThreadStack;
                if (stack.Count > 0)
                {
                    return stack.Peek();
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        private static Stack<string> ThreadStack
        {
            get { return ThreadLocalStorageHelper.GetDataForSlot<Stack<string>>(dataSlot); }
        }

        /// <summary>
        ///     Pushes the specified text on current thread NDC.
        /// </summary>
        /// <param name="text">The text to be pushed.</param>
        /// <returns>An instance of the object that implements IDisposable that returns the stack to the previous level when IDisposable.Dispose() is called. To be used with C# using() statement.</returns>
        public static IDisposable Push(string text)
        {
            var stack = ThreadStack;
            var previousCount = stack.Count;
            stack.Push(text);
            return new StackPopper(stack, previousCount);
        }

        /// <summary>
        ///     Pops the top message off the NDC stack.
        /// </summary>
        /// <returns>The top message which is no longer on the stack.</returns>
        public static string Pop()
        {
            var stack = ThreadStack;
            if (stack.Count > 0)
            {
                return stack.Pop();
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        ///     Clears current thread NDC stack.
        /// </summary>
        public static void Clear()
        {
            ThreadStack.Clear();
        }

        /// <summary>
        ///     Gets all messages on the stack.
        /// </summary>
        /// <returns>Array of strings on the stack.</returns>
        public static string[] GetAllMessages()
        {
            return ThreadStack.ToArray();
        }

        /// <summary>
        ///     Resets the stack to the original count during <see cref="IDisposable.Dispose" />.
        /// </summary>
        private class StackPopper : IDisposable
        {
            private readonly int previousCount;
            private readonly Stack<string> stack;

            /// <summary>
            ///     Initializes a new instance of the <see cref="StackPopper" /> class.
            /// </summary>
            /// <param name="stack">The stack.</param>
            /// <param name="previousCount">The previous count.</param>
            public StackPopper(Stack<string> stack, int previousCount)
            {
                this.stack = stack;
                this.previousCount = previousCount;
            }

            /// <summary>
            ///     Reverts the stack to original item count.
            /// </summary>
            void IDisposable.Dispose()
            {
                while (stack.Count > previousCount)
                {
                    stack.Pop();
                }
            }
        }
    }
}