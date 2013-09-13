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


#if !SILVERLIGHT

namespace Transformalize.Libs.NLog.Internal
{
    /// <summary>
    ///     Returns details about current process and thread in a portable manner.
    /// </summary>
    internal abstract class ThreadIDHelper
    {
        /// <summary>
        ///     Initializes static members of the ThreadIDHelper class.
        /// </summary>
        static ThreadIDHelper()
        {
#if NET_CF
            Instance = new Win32ThreadIDHelper();
#else
            if (PlatformDetector.IsWin32)
            {
                Instance = new Win32ThreadIDHelper();
            }
            else
            {
                Instance = new PortableThreadIDHelper();
            }
#endif
        }

        /// <summary>
        ///     Gets the singleton instance of PortableThreadIDHelper or
        ///     Win32ThreadIDHelper depending on runtime environment.
        /// </summary>
        /// <value>The instance.</value>
        public static ThreadIDHelper Instance { get; private set; }

        /// <summary>
        ///     Gets current thread ID.
        /// </summary>
        public abstract int CurrentThreadID { get; }

        /// <summary>
        ///     Gets current process ID.
        /// </summary>
        public abstract int CurrentProcessID { get; }

        /// <summary>
        ///     Gets current process name.
        /// </summary>
        public abstract string CurrentProcessName { get; }

        /// <summary>
        ///     Gets current process name (excluding filename extension, if any).
        /// </summary>
        public abstract string CurrentProcessBaseName { get; }
    }
}

#endif