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

namespace Transformalize.Libs.NLog.Internal
{
    /// <summary>
    ///     Portable implementation of <see cref="FileInfoHelper" />.
    /// </summary>
    internal class PortableFileInfoHelper : FileInfoHelper
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
            var fi = new FileInfo(fileName);
            if (fi.Exists)
            {
                fileLength = fi.Length;
                lastWriteTime = fi.LastWriteTime;
                return true;
            }
            else
            {
                fileLength = -1;
                lastWriteTime = DateTime.MinValue;
                return false;
            }
        }
    }
}