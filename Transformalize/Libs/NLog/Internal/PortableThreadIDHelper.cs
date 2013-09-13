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
using System.Diagnostics;
using System.IO;
using System.Threading;

#if !SILVERLIGHT && !NET_CF

namespace Transformalize.Libs.NLog.Internal
{
    /// <summary>
    ///     Portable implementation of <see cref="ThreadIDHelper" />.
    /// </summary>
    internal class PortableThreadIDHelper : ThreadIDHelper
    {
        private const string UnknownProcessName = "<unknown>";

        private readonly int currentProcessID;

        private string currentProcessBaseName;
        private string currentProcessName;

        /// <summary>
        ///     Initializes a new instance of the <see cref="PortableThreadIDHelper" /> class.
        /// </summary>
        public PortableThreadIDHelper()
        {
            currentProcessID = Process.GetCurrentProcess().Id;
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
            get
            {
                GetProcessName();
                return currentProcessName;
            }
        }

        /// <summary>
        ///     Gets current process name (excluding filename extension, if any).
        /// </summary>
        /// <value></value>
        public override string CurrentProcessBaseName
        {
            get
            {
                GetProcessName();
                return currentProcessBaseName;
            }
        }

        /// <summary>
        ///     Gets the name of the process.
        /// </summary>
        private void GetProcessName()
        {
            if (currentProcessName == null)
            {
                try
                {
                    currentProcessName = Process.GetCurrentProcess().MainModule.FileName;
                }
                catch (Exception exception)
                {
                    if (exception.MustBeRethrown())
                    {
                        throw;
                    }

                    currentProcessName = UnknownProcessName;
                }

                currentProcessBaseName = Path.GetFileNameWithoutExtension(currentProcessName);
            }
        }
    }
}

#endif