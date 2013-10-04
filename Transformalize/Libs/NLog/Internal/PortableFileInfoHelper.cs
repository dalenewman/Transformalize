#region License
// /*
// See license included in this library folder.
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