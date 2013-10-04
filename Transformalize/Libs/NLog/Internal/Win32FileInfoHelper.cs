#region License
// /*
// See license included in this library folder.
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