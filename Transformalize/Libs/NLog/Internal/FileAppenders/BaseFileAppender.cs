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
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.Win32.SafeHandles;
using Transformalize.Libs.NLog.Common;

namespace Transformalize.Libs.NLog.Internal.FileAppenders
{
    /// <summary>
    ///     Base class for optimized file appenders.
    /// </summary>
    internal abstract class BaseFileAppender : IDisposable
    {
        private readonly Random random = new Random();

        /// <summary>
        ///     Initializes a new instance of the <see cref="BaseFileAppender" /> class.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="createParameters">The create parameters.</param>
        public BaseFileAppender(string fileName, ICreateFileParameters createParameters)
        {
            CreateFileParameters = createParameters;
            FileName = fileName;
            OpenTime = CurrentTimeGetter.Now;
            LastWriteTime = DateTime.MinValue;
        }

        /// <summary>
        ///     Gets the name of the file.
        /// </summary>
        /// <value>The name of the file.</value>
        public string FileName { get; private set; }

        /// <summary>
        ///     Gets the last write time.
        /// </summary>
        /// <value>The last write time.</value>
        public DateTime LastWriteTime { get; private set; }

        /// <summary>
        ///     Gets the open time of the file.
        /// </summary>
        /// <value>The open time.</value>
        public DateTime OpenTime { get; private set; }

        /// <summary>
        ///     Gets the file creation parameters.
        /// </summary>
        /// <value>The file creation parameters.</value>
        public ICreateFileParameters CreateFileParameters { get; private set; }

        /// <summary>
        ///     Writes the specified bytes.
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        public abstract void Write(byte[] bytes);

        /// <summary>
        ///     Flushes this instance.
        /// </summary>
        public abstract void Flush();

        /// <summary>
        ///     Closes this instance.
        /// </summary>
        public abstract void Close();

        /// <summary>
        ///     Gets the file info.
        /// </summary>
        /// <param name="lastWriteTime">The last write time.</param>
        /// <param name="fileLength">Length of the file.</param>
        /// <returns>True if the operation succeeded, false otherwise.</returns>
        public abstract bool GetFileInfo(out DateTime lastWriteTime, out long fileLength);

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing">
        ///     True to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Close();
            }
        }

        /// <summary>
        ///     Records the last write time for a file.
        /// </summary>
        protected void FileTouched()
        {
            LastWriteTime = CurrentTimeGetter.Now;
        }

        /// <summary>
        ///     Records the last write time for a file to be specific date.
        /// </summary>
        /// <param name="dateTime">Date and time when the last write occurred.</param>
        protected void FileTouched(DateTime dateTime)
        {
            LastWriteTime = dateTime;
        }

        /// <summary>
        ///     Creates the file stream.
        /// </summary>
        /// <param name="allowConcurrentWrite">
        ///     If set to <c>true</c> allow concurrent writes.
        /// </param>
        /// <returns>
        ///     A <see cref="FileStream" /> object which can be used to write to the file.
        /// </returns>
        protected FileStream CreateFileStream(bool allowConcurrentWrite)
        {
            var currentDelay = CreateFileParameters.ConcurrentWriteAttemptDelay;

            InternalLogger.Trace("Opening {0} with concurrentWrite={1}", FileName, allowConcurrentWrite);
            for (var i = 0; i < CreateFileParameters.ConcurrentWriteAttempts; ++i)
            {
                try
                {
                    try
                    {
                        return TryCreateFileStream(allowConcurrentWrite);
                    }
                    catch (DirectoryNotFoundException)
                    {
                        if (!CreateFileParameters.CreateDirs)
                        {
                            throw;
                        }

                        Directory.CreateDirectory(Path.GetDirectoryName(FileName));
                        return TryCreateFileStream(allowConcurrentWrite);
                    }
                }
                catch (IOException)
                {
                    if (!CreateFileParameters.ConcurrentWrites || !allowConcurrentWrite || i + 1 == CreateFileParameters.ConcurrentWriteAttempts)
                    {
                        throw; // rethrow
                    }

                    var actualDelay = random.Next(currentDelay);
                    InternalLogger.Warn("Attempt #{0} to open {1} failed. Sleeping for {2}ms", i, FileName, actualDelay);
                    currentDelay *= 2;
                    Thread.Sleep(actualDelay);
                }
            }

            throw new InvalidOperationException("Should not be reached.");
        }

#if !NET_CF && !SILVERLIGHT
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Objects are disposed elsewhere")]
        private FileStream WindowsCreateFile(string fileName, bool allowConcurrentWrite)
        {
            var fileShare = Win32FileNativeMethods.FILE_SHARE_READ;

            if (allowConcurrentWrite)
            {
                fileShare |= Win32FileNativeMethods.FILE_SHARE_WRITE;
            }

            if (CreateFileParameters.EnableFileDelete && PlatformDetector.CurrentOS != RuntimeOS.Windows)
            {
                fileShare |= Win32FileNativeMethods.FILE_SHARE_DELETE;
            }

            var handle = Win32FileNativeMethods.CreateFile(
                fileName,
                Win32FileNativeMethods.FileAccess.GenericWrite,
                fileShare,
                IntPtr.Zero,
                Win32FileNativeMethods.CreationDisposition.OpenAlways,
                CreateFileParameters.FileAttributes,
                IntPtr.Zero);

            if (handle.ToInt32() == -1)
            {
                Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
            }

            var safeHandle = new SafeFileHandle(handle, true);
            var returnValue = new FileStream(safeHandle, FileAccess.Write, CreateFileParameters.BufferSize);
            returnValue.Seek(0, SeekOrigin.End);
            return returnValue;
        }
#endif

        private FileStream TryCreateFileStream(bool allowConcurrentWrite)
        {
            var fileShare = FileShare.Read;

            if (allowConcurrentWrite)
            {
                fileShare = FileShare.ReadWrite;
            }

#if !NET_CF
            if (CreateFileParameters.EnableFileDelete && PlatformDetector.CurrentOS != RuntimeOS.Windows)
            {
                fileShare |= FileShare.Delete;
            }
#endif

#if !NET_CF && !SILVERLIGHT
            if (PlatformDetector.IsDesktopWin32)
            {
                return WindowsCreateFile(FileName, allowConcurrentWrite);
            }
#endif

            return new FileStream(
                FileName,
                FileMode.Append,
                FileAccess.Write,
                fileShare,
                CreateFileParameters.BufferSize);
        }
    }
}