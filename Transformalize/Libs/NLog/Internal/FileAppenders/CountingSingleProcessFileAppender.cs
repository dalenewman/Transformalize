#region License
// /*
// See license included in this library folder.
// */
#endregion

using System;
using System.IO;

namespace Transformalize.Libs.NLog.Internal.FileAppenders
{
    /// <summary>
    ///     Implementation of <see cref="BaseFileAppender" /> which caches
    ///     file information.
    /// </summary>
    internal class CountingSingleProcessFileAppender : BaseFileAppender
    {
        public static readonly IFileAppenderFactory TheFactory = new Factory();

        private long currentFileLength;
        private FileStream file;

        /// <summary>
        ///     Initializes a new instance of the <see cref="CountingSingleProcessFileAppender" /> class.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="parameters">The parameters.</param>
        public CountingSingleProcessFileAppender(string fileName, ICreateFileParameters parameters)
            : base(fileName, parameters)
        {
            var fi = new FileInfo(fileName);
            if (fi.Exists)
            {
                FileTouched(fi.LastWriteTime);
                currentFileLength = fi.Length;
            }
            else
            {
                FileTouched();
                currentFileLength = 0;
            }

            file = CreateFileStream(false);
        }

        /// <summary>
        ///     Closes this instance of the appender.
        /// </summary>
        public override void Close()
        {
            if (file != null)
            {
                file.Close();
                file = null;
            }
        }

        /// <summary>
        ///     Flushes this current appender.
        /// </summary>
        public override void Flush()
        {
            if (file == null)
            {
                return;
            }

            file.Flush();
            FileTouched();
        }

        /// <summary>
        ///     Gets the file info.
        /// </summary>
        /// <param name="lastWriteTime">The last write time.</param>
        /// <param name="fileLength">Length of the file.</param>
        /// <returns>True if the operation succeeded, false otherwise.</returns>
        public override bool GetFileInfo(out DateTime lastWriteTime, out long fileLength)
        {
            lastWriteTime = LastWriteTime;
            fileLength = currentFileLength;
            return true;
        }

        /// <summary>
        ///     Writes the specified bytes to a file.
        /// </summary>
        /// <param name="bytes">The bytes to be written.</param>
        public override void Write(byte[] bytes)
        {
            if (file == null)
            {
                return;
            }

            currentFileLength += bytes.Length;
            file.Write(bytes, 0, bytes.Length);
            FileTouched();
        }

        /// <summary>
        ///     Factory class which creates <see cref="CountingSingleProcessFileAppender" /> objects.
        /// </summary>
        private class Factory : IFileAppenderFactory
        {
            /// <summary>
            ///     Opens the appender for given file name and parameters.
            /// </summary>
            /// <param name="fileName">Name of the file.</param>
            /// <param name="parameters">Creation parameters.</param>
            /// <returns>
            ///     Instance of <see cref="BaseFileAppender" /> which can be used to write to the file.
            /// </returns>
            BaseFileAppender IFileAppenderFactory.Open(string fileName, ICreateFileParameters parameters)
            {
                return new CountingSingleProcessFileAppender(fileName, parameters);
            }
        }
    }
}