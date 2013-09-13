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

namespace Transformalize.Libs.NLog.Internal
{
    /// <summary>
    ///     Detects the platform the NLog is running on.
    /// </summary>
    internal static class PlatformDetector
    {
        private static readonly RuntimeOS currentOS = GetCurrentRuntimeOS();

        /// <summary>
        ///     Gets the current runtime OS.
        /// </summary>
        public static RuntimeOS CurrentOS
        {
            get { return currentOS; }
        }

        /// <summary>
        ///     Gets a value indicating whether current OS is a desktop version of Windows.
        /// </summary>
        public static bool IsDesktopWin32
        {
            get { return currentOS == RuntimeOS.Windows || currentOS == RuntimeOS.WindowsNT; }
        }

        /// <summary>
        ///     Gets a value indicating whether current OS is Win32-based (desktop or mobile).
        /// </summary>
        public static bool IsWin32
        {
            get { return currentOS == RuntimeOS.Windows || currentOS == RuntimeOS.WindowsNT || currentOS == RuntimeOS.WindowsCE; }
        }

        /// <summary>
        ///     Gets a value indicating whether current OS is Unix-based.
        /// </summary>
        public static bool IsUnix
        {
            get { return currentOS == RuntimeOS.Unix; }
        }

        private static RuntimeOS GetCurrentRuntimeOS()
        {
            var platformID = Environment.OSVersion.Platform;
            if ((int) platformID == 4 || (int) platformID == 128)
            {
                return RuntimeOS.Unix;
            }

            if ((int) platformID == 3)
            {
                return RuntimeOS.WindowsCE;
            }

            if (platformID == PlatformID.Win32Windows)
            {
                return RuntimeOS.Windows;
            }

            if (platformID == PlatformID.Win32NT)
            {
                return RuntimeOS.WindowsNT;
            }

            return RuntimeOS.Unknown;
        }
    }
}