#region License
// /*
// See license included in this library folder.
// */
#endregion
namespace Transformalize.Libs.NLog.Internal.FileAppenders
{
#if MONO

using System;
using System.Xml;
using System.IO;
using System.Threading;
using System.Text;
using System.Collections;
using System.Collections.Specialized;

using NLog;
using NLog.Config;
using NLog.Common;

using NLog.Internal;

using Mono.Unix;
using Mono.Unix.Native;

namespace NLog.Internal.FileAppenders
{
    /// <summary>
    /// Provides a multiprocess-safe atomic file appends while
    /// keeping the files open.
    /// </summary>
    /// <remarks>
    /// On Unix you can get all the appends to be atomic, even when multiple 
    /// processes are trying to write to the same file, because setting the file
    /// pointer to the end of the file and appending can be made one operation.
    /// </remarks>
    internal class UnixMultiProcessFileAppender : BaseFileAppender
    {
        private UnixStream file;

        public static readonly IFileAppenderFactory TheFactory = new Factory();

        public class Factory : IFileAppenderFactory
        {
            public BaseFileAppender Open(string fileName, ICreateFileParameters parameters)
            {
                return new UnixMultiProcessFileAppender(fileName, parameters);
            }
        }

        public UnixMultiProcessFileAppender(string fileName, ICreateFileParameters parameters) : base(fileName, parameters)
        {
            int fd = Syscall.open(fileName, OpenFlags.O_CREAT | OpenFlags.O_WRONLY | OpenFlags.O_APPEND, (FilePermissions)(6 | (6 << 3) | (6 << 6)));
            if (fd == -1)
            {
                if (Stdlib.GetLastError() == Errno.ENOENT && parameters.CreateDirs)
                {
                    string dirName = Path.GetDirectoryName(fileName);
                    if (!Directory.Exists(dirName) && parameters.CreateDirs)
                        Directory.CreateDirectory(dirName);
                    
                    fd = Syscall.open(fileName, OpenFlags.O_CREAT | OpenFlags.O_WRONLY | OpenFlags.O_APPEND, (FilePermissions)(6 | (6 << 3) | (6 << 6)));
                }
            }
            if (fd == -1)
                UnixMarshal.ThrowExceptionForLastError();

            try
            {
                this.file = new UnixStream(fd, true);
            }
            catch
            {
                Syscall.close(fd);
                throw;
            }
        }

        public override void Write(byte[] bytes)
        {
            if (this.file == null)
                return;
            this.file.Write(bytes, 0, bytes.Length);
            FileTouched();
        }

        public override void Close()
        {
            if (this.file == null)
                return;
            InternalLogger.Trace("Closing '{0}'", FileName);
            this.file.Close();
            this.file = null;
            FileTouched();
        }

        public override void Flush()
        {
            // do nothing, the stream is always flushed
        }

        public override bool GetFileInfo(out DateTime lastWriteTime, out long fileLength)
        {
            FileInfo fi = new FileInfo(FileName);
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

#endif
}