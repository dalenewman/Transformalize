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

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Transformalize.Libs.NLog
{
    /// <summary>
    ///     Specialized LogFactory that can return instances of custom logger types.
    /// </summary>
    /// <typeparam name="T">
    ///     The type of the logger to be returned. Must inherit from <see cref="Logger" />.
    /// </typeparam>
    public class LogFactory<T> : LogFactory
        where T : Logger
    {
        /// <summary>
        ///     Gets the logger.
        /// </summary>
        /// <param name="name">The logger name.</param>
        /// <returns>
        ///     An instance of <typeparamref name="T" />.
        /// </returns>
        public new T GetLogger(string name)
        {
            return (T) GetLogger(name, typeof (T));
        }

#if !NET_CF
        /// <summary>
        ///     Gets the logger named after the currently-being-initialized class.
        /// </summary>
        /// <returns>The logger.</returns>
        /// <remarks>
        ///     This is a slow-running method.
        ///     Make sure you're not doing this in a loop.
        /// </remarks>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Backwards compatibility")]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public new T GetCurrentClassLogger()
        {
#if SILVERLIGHT
            StackFrame frame = new StackFrame(1);
#else
            var frame = new StackFrame(1, false);
#endif

            return GetLogger(frame.GetMethod().DeclaringType.FullName);
        }
#endif
    }
}