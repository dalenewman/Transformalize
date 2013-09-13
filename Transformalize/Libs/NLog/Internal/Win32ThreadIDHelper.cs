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
using System.IO;
using System.Text;
using System.Threading;

#if !SILVERLIGHT

namespace Transformalize.Libs.NLog.Internal
{
    /// <summary>
    ///     Win32-optimized implementation of <see cref="ThreadIDHelper" />.
    /// </summary>
    internal class Win32ThreadIDHelper : ThreadIDHelper
    {
        private readonly string currentProcessBaseName;
        private readonly int currentProcessID;

        private readonly string currentProcessName;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Win32ThreadIDHelper" /> class.
        /// </summary>
        public Win32ThreadIDHelper()
        {
            currentProcessID = NativeMethods.GetCurrentProcessId();

            var sb = new StringBuilder(512);
            if (0 == NativeMethods.GetModuleFileName(IntPtr.Zero, sb, sb.Capacity))
            {
                throw new InvalidOperationException("Cannot determine program name.");
            }

            currentProcessName = sb.ToString();
            currentProcessBaseName = Path.GetFileNameWithoutExtension(currentProcessName);
        }

        /// <summary>
        ///     Gets current thread ID.
        /// </summary>
        /// <value></value>
        public override int CurrentThreadID
        {
            get { return Thread.CurrentThread.ManagedThreadId; }
        }

        /// <summary>
        ///     Gets current process ID.
        /// </summary>
        /// <value></value>
        public override int CurrentProcessID
        {
            get { return currentProcessID; }
        }

        /// <summary>
        ///     Gets current process name.
        /// </summary>
        /// <value></value>
        public override string CurrentProcessName
        {
            get { return currentProcessName; }
        }

        /// <summary>
        ///     Gets current process name (excluding filename extension, if any).
        /// </summary>
        /// <value></value>
        public override string CurrentProcessBaseName
        {
            get { return currentProcessBaseName; }
        }
    }
}

#endif