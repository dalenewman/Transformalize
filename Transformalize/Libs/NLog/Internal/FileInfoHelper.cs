#region License
// /*
// See license included in this library folder.
// */
#endregion

using System;

namespace Transformalize.Libs.NLog.Internal
{
    /// <summary>
    ///     Optimized routines to get the size and last write time of the specified file.
    /// </summary>
    internal abstract class FileInfoHelper
    {
        /// <summary>
        ///     Initializes static members of the FileInfoHelper class.
        /// </summary>
        static FileInfoHelper()
        {
#if NET_CF || SILVERLIGHT
            Helper = new PortableFileInfoHelper();
#else
            if (PlatformDetector.IsDesktopWin32)
            {
                Helper = new Win32FileInfoHelper();
            }
            else
            {
                Helper = new PortableFileInfoHelper();
            }
#endif
        }

        internal static FileInfoHelper Helper { get; private set; }

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
        public abstract bool GetFileInfo(string fileName, IntPtr fileHandle, out DateTime lastWriteTime, out long fileLength);
    }
}