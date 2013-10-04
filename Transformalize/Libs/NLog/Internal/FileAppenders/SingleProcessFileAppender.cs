#region License
// /*
// See license included in this library folder.
// */
#endregion

using System;
using System.IO;
using Transformalize.Libs.NLog.Common;

namespace Transformalize.Libs.NLog.Internal.FileAppenders
{
    /// <summary>
    ///     Optimized single-process file appender which keeps the file open for exclusive write.
    /// </summary>
    internal class SingleProcessFileAppender : BaseFileAppender
    {
        public static readonly IFileAppenderFactory TheFactory = new Factory();

        private FileStream file;

        /// <summary>
        ///     Initializes a new instance of the <see cref="SingleProcessFileAppender" /> class.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="parameters">The parameters.</param>
        public SingleProcessFileAppender(string fileName, ICreateFileParameters parameters) : base(fileName, parameters)
        {
            file = CreateFileStream(false);
        }

        /// <summary>
        ///     Writes the specified bytes.
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        public override void Write(byte[] bytes)
        {
            if (file == null)
            {
                return;
            }

            file.Write(bytes, 0, bytes.Length);
            FileTouched();
        }

        /// <summary>
        ///     Flushes this instance.
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
        ///     Closes this instance.
        /// </summary>
        public override void Close()
        {
            if (file == null)
            {
                return;
            }

            InternalLogger.Trace("Closing '{0}'", FileName);
            file.Close();
            file = null;
        }

        /// <summary>
        ///     Gets the file info.
        /// </summary>
        /// <param name="lastWriteTime">The last write time.</param>
        /// <param name="fileLength">Length of the file.</param>
        /// <returns>
        ///     True if the operation succeeded, false otherwise.
        /// </returns>
        public override bool GetFileInfo(out DateTime lastWriteTime, out long fileLength)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        ///     Factory class.
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
                return new SingleProcessFileAppender(fileName, parameters);
            }
        }
    }
}