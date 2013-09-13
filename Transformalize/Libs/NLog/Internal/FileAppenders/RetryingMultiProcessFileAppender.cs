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

namespace Transformalize.Libs.NLog.Internal.FileAppenders
{
    /// <summary>
    ///     Multi-process and multi-host file appender which attempts
    ///     to get exclusive write access and retries if it's not available.
    /// </summary>
    internal class RetryingMultiProcessFileAppender : BaseFileAppender
    {
        public static readonly IFileAppenderFactory TheFactory = new Factory();

        /// <summary>
        ///     Initializes a new instance of the <see cref="RetryingMultiProcessFileAppender" /> class.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="parameters">The parameters.</param>
        public RetryingMultiProcessFileAppender(string fileName, ICreateFileParameters parameters) : base(fileName, parameters)
        {
        }

        /// <summary>
        ///     Writes the specified bytes.
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        public override void Write(byte[] bytes)
        {
            using (var fileStream = CreateFileStream(false))
            {
                fileStream.Write(bytes, 0, bytes.Length);
            }

            FileTouched();
        }

        /// <summary>
        ///     Flushes this instance.
        /// </summary>
        public override void Flush()
        {
            // nothing to do
        }

        /// <summary>
        ///     Closes this instance.
        /// </summary>
        public override void Close()
        {
            // nothing to do
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
            var fi = new FileInfo(FileName);
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
                return new RetryingMultiProcessFileAppender(fileName, parameters);
            }
        }
    }
}