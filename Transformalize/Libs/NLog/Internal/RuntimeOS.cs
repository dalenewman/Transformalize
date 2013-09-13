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

namespace Transformalize.Libs.NLog.Internal
{
    /// <summary>
    ///     Supported operating systems.
    /// </summary>
    /// <remarks>
    ///     If you add anything here, make sure to add the appropriate detection
    ///     code to <see cref="PlatformDetector" />
    /// </remarks>
    internal enum RuntimeOS
    {
        /// <summary>
        ///     Any operating system.
        /// </summary>
        Any,

        /// <summary>
        ///     Unix/Linux operating systems.
        /// </summary>
        Unix,

        /// <summary>
        ///     Windows CE.
        /// </summary>
        WindowsCE,

        /// <summary>
        ///     Desktop versions of Windows (95,98,ME).
        /// </summary>
        Windows,

        /// <summary>
        ///     Windows NT, 2000, 2003 and future versions based on NT technology.
        /// </summary>
        WindowsNT,

        /// <summary>
        ///     Unknown operating system.
        /// </summary>
        Unknown,
    }
}