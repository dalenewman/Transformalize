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

#if !NET_CF && !SILVERLIGHT

namespace Transformalize.Libs.NLog.Internal
{
    /// <summary>
    ///     Win32-optimized implementation of <see cref="FileInfoHelper" />.
    /// </summary>
    internal class Win32FileInfoHelper : FileInfoHelper
    {
        /// <summary>
        ///     Gets the information about a file.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="fileHandle">The file handle.</param>
        /// <param name="lastWriteTime">The last write time of the file.</param>
        /// <param name="fileLength">Length of the file.</param>
        /// <returns>
        ///     A value of <c>true</c> if file information was retrieved successfully, <c>false</c> otherwise.
        /// </returns>
        public override bool GetFileInfo(string fileName, IntPtr fileHandle, out DateTime lastWriteTime, out long fileLength)
        {
            Win32FileNativeMethods.BY_HANDLE_FILE_INFORMATION fi;

            if (Win32FileNativeMethods.GetFileInformationByHandle(fileHandle, out fi))
            {
                lastWriteTime = DateTime.FromFileTime(fi.ftLastWriteTime);
                fileLength = fi.nFileSizeLow + (((long) fi.nFileSizeHigh) << 32);
                return true;
            }
            else
            {
                lastWriteTime = DateTime.MinValue;
                fileLength = -1;
                return false;
            }
        }
    }
}

#endif