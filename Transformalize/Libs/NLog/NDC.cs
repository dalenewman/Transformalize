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