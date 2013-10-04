#region License
// /*
// See license included in this library folder.
// */
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using Transformalize.Libs.NLog.Common;
using Transformalize.Libs.NLog.Config;
using Transformalize.Libs.NLog.Internal;
using Transformalize.Libs.NLog.Internal.FileAppenders;
using Transformalize.Libs.NLog.Layouts;

#if !SILVERLIGHT2 && !SILVERLIGHT3 && !WINDOWS_PHONE

namespace Transformalize.Libs.NLog.Targets
{
    /// <summary>
    ///     Writes log messages to one or more files.
    /// </summary>
    /// <seealso href="http://nlog-project.org/wiki/File_target">Documentation on NLog Wiki</seealso>
    [Target("File")]
    public class FileTarget : TargetWithLayoutHeaderAndFooter, ICreateFileParameters
    {
        private readonly Dictionary<string, DateTime> initializedFiles = new Dictionary<string, DateTime>();

        private LineEndingMode lineEndingMode = LineEndingMode.Default;
        private IFileAppenderFactory appenderFactory;
        private BaseFileAppender[] recentAppenders;
        private Timer autoClosingTimer;
        private int initializedFilesCounter;

        /// <summary>
        ///     Initializes a new instance of the <see cref="FileTarget" /> class.
        /// </summary>
        /// <remarks>
        ///     The default value of the layout is: <code>${longdate}|${level:uppercase=true}|${logger}|${message}</code>
        /// </remarks>
        public FileTarget()
        {
            ArchiveNumbering = ArchiveNumberingMode.Sequence;
            MaxArchiveFiles = 9;
            ConcurrentWriteAttemptDelay = 1;
            ArchiveEvery = FileArchivePeriod.None;
            ArchiveAboveSize = -1;
            ConcurrentWriteAttempts = 10;
            ConcurrentWrites = true;
#if SILVERLIGHT
            this.Encoding = Encoding.UTF8;
#else
            Encoding = Encoding.Default;
#endif
            BufferSize = 32768;
            AutoFlush = true;
#if !SILVERLIGHT && !NET_CF
            FileAttributes = Win32FileAttributes.Normal;
#endif
            NewLineChars = EnvironmentHelper.NewLine;
            EnableFileDelete = true;
            OpenFileCacheTimeout = -1;
            OpenFileCacheSize = 5;
            CreateDirs = true;
        }

        /// <summary>
        ///     Gets or sets the name of the file to write to.
        /// </summary>
        /// <remarks>
        ///     This FileName string is a layout which may include instances of layout renderers.
        ///     This lets you use a single target to write to multiple files.
        /// </remarks>
        /// <example>
        ///     The following value makes NLog write logging events to files based on the log level in the directory where
        ///     the application runs.
        ///     <code>${basedir}/${level}.log</code>
        ///     All <c>Debug</c> messages will go to <c>Debug.log</c>, all <c>Info</c> messages will go to <c>Info.log</c> and so on.
        ///     You can combine as many of the layout renderers as you want to produce an arbitrary log file name.
        /// </example>
        /// <docgen category='Output Options' order='1' />
        [RequiredParameter]
        public Layout FileName { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether to create directories if they don't exist.
        /// </summary>
        /// <remarks>
        ///     Setting this to false may improve performance a bit, but you'll receive an error
        ///     when attempting to write to a directory that's not present.
        /// </remarks>
        /// <docgen category='Output Options' order='10' />
        [DefaultValue(true)]
        [Advanced]
        public bool CreateDirs { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether to delete old log file on startup.
        /// </summary>
        /// <remarks>
        ///     This option works only when the "FileName" parameter denotes a single file.
        /// </remarks>
        /// <docgen category='Output Options' order='10' />
        [DefaultValue(false)]
        public bool DeleteOldFileOnStartup { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether to replace file contents on each write instead of appending log message at the end.
        /// </summary>
        /// <docgen category='Output Options' order='10' />
        [DefaultValue(false)]
        [Advanced]
        public bool ReplaceFileContentsOnEachWrite { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether to keep log file open instead of opening and closing it on each logging event.
        /// </summary>
        /// <remarks>
        ///     Setting this property to <c>True</c> helps improve performance.
        /// </remarks>
        /// <docgen category='Performance Tuning Options' order='10' />
        [DefaultValue(false)]
        public bool KeepFileOpen { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether to enable log file(s) to be deleted.
        /// </summary>
        /// <docgen category='Output Options' order='10' />
        [DefaultValue(true)]
        public bool EnableFileDelete { get; set; }

#if !NET_CF && !SILVERLIGHT
        /// <summary>
        ///     Gets or sets the file attributes (Windows only).
        /// </summary>
        /// <docgen category='Output Options' order='10' />
        [Advanced]
        public Win32FileAttributes FileAttributes { get; set; }
#endif

        /// <summary>
        ///     Gets or sets the line ending mode.
        /// </summary>
        /// <docgen category='Layout Options' order='10' />
        [Advanced]
        public LineEndingMode LineEnding
        {
            get { return lineEndingMode; }

            set
            {
                lineEndingMode = value;
                switch (value)
                {
                    case LineEndingMode.CR:
                        NewLineChars = "\r";
                        break;

                    case LineEndingMode.LF:
                        NewLineChars = "\n";
                        break;

                    case LineEndingMode.CRLF:
                        NewLineChars = "\r\n";
                        break;

                    case LineEndingMode.Default:
                        NewLineChars = EnvironmentHelper.NewLine;
                        break;

                    case LineEndingMode.None:
                        NewLineChars = string.Empty;
                        break;
                }
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether to automatically flush the file buffers after each log message.
        /// </summary>
        /// <docgen category='Performance Tuning Options' order='10' />
        [DefaultValue(true)]
        public bool AutoFlush { get; set; }

        /// <summary>
        ///     Gets or sets the number of files to be kept open. Setting this to a higher value may improve performance
        ///     in a situation where a single File target is writing to many files
        ///     (such as splitting by level or by logger).
        /// </summary>
        /// <remarks>
        ///     The files are managed on a LRU (least recently used) basis, which flushes
        ///     the files that have not been used for the longest period of time should the
        ///     cache become full. As a rule of thumb, you shouldn't set this parameter to
        ///     a very high value. A number like 10-15 shouldn't be exceeded, because you'd
        ///     be keeping a large number of files open which consumes system resources.
        /// </remarks>
        /// <docgen category='Performance Tuning Options' order='10' />
        [DefaultValue(5)]
        [Advanced]
        public int OpenFileCacheSize { get; set; }

        /// <summary>
        ///     Gets or sets the maximum number of seconds that files are kept open. If this number is negative the files are
        ///     not automatically closed after a period of inactivity.
        /// </summary>
        /// <docgen category='Performance Tuning Options' order='10' />
        [DefaultValue(-1)]
        [Advanced]
        public int OpenFileCacheTimeout { get; set; }

        /// <summary>
        ///     Gets or sets the log file buffer size in bytes.
        /// </summary>
        /// <docgen category='Performance Tuning Options' order='10' />
        [DefaultValue(32768)]
        public int BufferSize { get; set; }

        /// <summary>
        ///     Gets or sets the file encoding.
        /// </summary>
        /// <docgen category='Layout Options' order='10' />
        public Encoding Encoding { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether concurrent writes to the log file by multiple processes on the same host.
        /// </summary>
        /// <remarks>
        ///     This makes multi-process logging possible. NLog uses a special technique
        ///     that lets it keep the files open for writing.
        /// </remarks>
        /// <docgen category='Performance Tuning Options' order='10' />
        [DefaultValue(true)]
        public bool ConcurrentWrites { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether concurrent writes to the log file by multiple processes on different network hosts.
        /// </summary>
        /// <remarks>
        ///     This effectively prevents files from being kept open.
        /// </remarks>
        /// <docgen category='Performance Tuning Options' order='10' />
        [DefaultValue(false)]
        public bool NetworkWrites { get; set; }

        /// <summary>
        ///     Gets or sets the number of times the write is appended on the file before NLog
        ///     discards the log message.
        /// </summary>
        /// <docgen category='Performance Tuning Options' order='10' />
        [DefaultValue(10)]
        [Advanced]
        public int ConcurrentWriteAttempts { get; set; }

        /// <summary>
        ///     Gets or sets the delay in milliseconds to wait before attempting to write to the file again.
        /// </summary>
        /// <remarks>
        ///     The actual delay is a random value between 0 and the value specified
        ///     in this parameter. On each failed attempt the delay base is doubled
        ///     up to <see cref="ConcurrentWriteAttempts" /> times.
        /// </remarks>
        /// <example>
        ///     Assuming that ConcurrentWriteAttemptDelay is 10 the time to wait will be:<p />
        ///     a random value between 0 and 10 milliseconds - 1st attempt<br />
        ///     a random value between 0 and 20 milliseconds - 2nd attempt<br />
        ///     a random value between 0 and 40 milliseconds - 3rd attempt<br />
        ///     a random value between 0 and 80 milliseconds - 4th attempt<br />
        ///     ...<p />
        ///     and so on.
        /// </example>
        /// <docgen category='Performance Tuning Options' order='10' />
        [DefaultValue(1)]
        [Advanced]
        public int ConcurrentWriteAttemptDelay { get; set; }

        /// <summary>
        ///     Gets or sets the size in bytes above which log files will be automatically archived.
        /// </summary>
        /// <remarks>
        ///     Caution: Enabling this option can considerably slow down your file
        ///     logging in multi-process scenarios. If only one process is going to
        ///     be writing to the file, consider setting <c>ConcurrentWrites</c>
        ///     to <c>false</c> for maximum performance.
        /// </remarks>
        /// <docgen category='Archival Options' order='10' />
        public long ArchiveAboveSize { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether to automatically archive log files every time the specified time passes.
        /// </summary>
        /// <remarks>
        ///     Files are moved to the archive as part of the write operation if the current period of time changes. For example
        ///     if the current <c>hour</c> changes from 10 to 11, the first write that will occur
        ///     on or after 11:00 will trigger the archiving.
        ///     <p>
        ///         Caution: Enabling this option can considerably slow down your file
        ///         logging in multi-process scenarios. If only one process is going to
        ///         be writing to the file, consider setting <c>ConcurrentWrites</c>
        ///         to <c>false</c> for maximum performance.
        ///     </p>
        /// </remarks>
        /// <docgen category='Archival Options' order='10' />
        public FileArchivePeriod ArchiveEvery { get; set; }

        /// <summary>
        ///     Gets or sets the name of the file to be used for an archive.
        /// </summary>
        /// <remarks>
        ///     It may contain a special placeholder {#####}
        ///     that will be replaced with a sequence of numbers depending on
        ///     the archiving strategy. The number of hash characters used determines
        ///     the number of numerical digits to be used for numbering files.
        /// </remarks>
        /// <docgen category='Archival Options' order='10' />
        public Layout ArchiveFileName { get; set; }

        /// <summary>
        ///     Gets or sets the maximum number of archive files that should be kept.
        /// </summary>
        /// <docgen category='Archival Options' order='10' />
        [DefaultValue(9)]
        public int MaxArchiveFiles { get; set; }

        /// <summary>
        ///     Gets or sets the way file archives are numbered.
        /// </summary>
        /// <docgen category='Archival Options' order='10' />
        public ArchiveNumberingMode ArchiveNumbering { get; set; }

        /// <summary>
        ///     Gets the characters that are appended after each line.
        /// </summary>
        protected internal string NewLineChars { get; private set; }

        /// <summary>
        ///     Removes records of initialized files that have not been
        ///     accessed in the last two days.
        /// </summary>
        /// <remarks>
        ///     Files are marked 'initialized' for the purpose of writing footers when the logging finishes.
        /// </remarks>
        public void CleanupInitializedFiles()
        {
            CleanupInitializedFiles(DateTime.Now.AddDays(-2));
        }

        /// <summary>
        ///     Removes records of initialized files that have not been
        ///     accessed after the specified date.
        /// </summary>
        /// <param name="cleanupThreshold">The cleanup threshold.</param>
        /// <remarks>
        ///     Files are marked 'initialized' for the purpose of writing footers when the logging finishes.
        /// </remarks>
        public void CleanupInitializedFiles(DateTime cleanupThreshold)
        {
            // clean up files that are two days old
            var filesToUninitialize = new List<string>();

            foreach (var de in initializedFiles)
            {
                var fileName = de.Key;
                var lastWriteTime = de.Value;
                if (lastWriteTime < cleanupThreshold)
                {
                    filesToUninitialize.Add(fileName);
                }
            }

            foreach (var fileName in filesToUninitialize)
            {
                WriteFooterAndUninitialize(fileName);
            }
        }

        /// <summary>
        ///     Flushes all pending file operations.
        /// </summary>
        /// <param name="asyncContinuation">The asynchronous continuation.</param>
        /// <remarks>
        ///     The timeout parameter is ignored, because file APIs don't provide
        ///     the needed functionality.
        /// </remarks>
        protected override void FlushAsync(AsyncContinuation asyncContinuation)
        {
            try
            {
                for (var i = 0; i < recentAppenders.Length; ++i)
                {
                    if (recentAppenders[i] == null)
                    {
                        break;
                    }

                    recentAppenders[i].Flush();
                }

                asyncContinuation(null);
            }
            catch (Exception exception)
            {
                if (exception.MustBeRethrown())
                {
                    throw;
                }

                asyncContinuation(exception);
            }
        }

        /// <summary>
        ///     Initializes file logging by creating data structures that
        ///     enable efficient multi-file logging.
        /// </summary>
        protected override void InitializeTarget()
        {
            base.InitializeTarget();

            if (!KeepFileOpen)
            {
                appenderFactory = RetryingMultiProcessFileAppender.TheFactory;
            }
            else
            {
                if (ArchiveAboveSize != -1 || ArchiveEvery != FileArchivePeriod.None)
                {
                    if (NetworkWrites)
                    {
                        appenderFactory = RetryingMultiProcessFileAppender.TheFactory;
                    }
                    else if (ConcurrentWrites)
                    {
#if NET_CF || SILVERLIGHT
                        this.appenderFactory = RetryingMultiProcessFileAppender.TheFactory;
#elif MONO
    //
    // mono on Windows uses mutexes, on Unix - special appender
    //
                        if (PlatformDetector.IsUnix)
                        {
                            this.appenderFactory = UnixMultiProcessFileAppender.TheFactory;
                        }
                        else
                        {
                            this.appenderFactory = MutexMultiProcessFileAppender.TheFactory;
                        }
#else
                        appenderFactory = MutexMultiProcessFileAppender.TheFactory;
#endif
                    }
                    else
                    {
                        appenderFactory = CountingSingleProcessFileAppender.TheFactory;
                    }
                }
                else
                {
                    if (NetworkWrites)
                    {
                        appenderFactory = RetryingMultiProcessFileAppender.TheFactory;
                    }
                    else if (ConcurrentWrites)
                    {
#if NET_CF || SILVERLIGHT
                        this.appenderFactory = RetryingMultiProcessFileAppender.TheFactory;
#elif MONO
    //
    // mono on Windows uses mutexes, on Unix - special appender
    //
                        if (PlatformDetector.IsUnix)
                        {
                            this.appenderFactory = UnixMultiProcessFileAppender.TheFactory;
                        }
                        else
                        {
                            this.appenderFactory = MutexMultiProcessFileAppender.TheFactory;
                        }
#else
                        appenderFactory = MutexMultiProcessFileAppender.TheFactory;
#endif
                    }
                    else
                    {
                        appenderFactory = SingleProcessFileAppender.TheFactory;
                    }
                }
            }

            recentAppenders = new BaseFileAppender[OpenFileCacheSize];

            if ((OpenFileCacheSize > 0 || EnableFileDelete) && OpenFileCacheTimeout > 0)
            {
                autoClosingTimer = new Timer(
                    AutoClosingTimerCallback,
                    null,
                    OpenFileCacheTimeout*1000,
                    OpenFileCacheTimeout*1000);
            }

            // Console.Error.WriteLine("Name: {0} Factory: {1}", this.Name, this.appenderFactory.GetType().FullName);
        }

        /// <summary>
        ///     Closes the file(s) opened for writing.
        /// </summary>
        protected override void CloseTarget()
        {
            base.CloseTarget();

            foreach (var fileName in new List<string>(initializedFiles.Keys))
            {
                WriteFooterAndUninitialize(fileName);
            }

            if (autoClosingTimer != null)
            {
                autoClosingTimer.Change(Timeout.Infinite, Timeout.Infinite);
                autoClosingTimer.Dispose();
                autoClosingTimer = null;
            }

            for (var i = 0; i < recentAppenders.Length; ++i)
            {
                if (recentAppenders[i] == null)
                {
                    break;
                }

                recentAppenders[i].Close();
                recentAppenders[i] = null;
            }
        }

        /// <summary>
        ///     Writes the specified logging event to a file specified in the FileName
        ///     parameter.
        /// </summary>
        /// <param name="logEvent">The logging event.</param>
        protected override void Write(LogEventInfo logEvent)
        {
            var fileName = FileName.Render(logEvent);
            var bytes = GetBytesToWrite(logEvent);

            if (ShouldAutoArchive(fileName, logEvent, bytes.Length))
            {
                InvalidateCacheItem(fileName);
                DoAutoArchive(fileName, logEvent);
            }

            WriteToFile(fileName, bytes, false);
        }

        /// <summary>
        ///     Writes the specified array of logging events to a file specified in the FileName
        ///     parameter.
        /// </summary>
        /// <param name="logEvents">
        ///     An array of <see cref="LogEventInfo " /> objects.
        /// </param>
        /// <remarks>
        ///     This function makes use of the fact that the events are batched by sorting
        ///     the requests by filename. This optimizes the number of open/close calls
        ///     and can help improve performance.
        /// </remarks>
        protected override void Write(AsyncLogEventInfo[] logEvents)
        {
            var buckets = logEvents.BucketSort(c => FileName.Render(c.LogEvent));
            using (var ms = new MemoryStream())
            {
                var pendingContinuations = new List<AsyncContinuation>();

                foreach (var bucket in buckets)
                {
                    var fileName = bucket.Key;

                    ms.SetLength(0);
                    ms.Position = 0;

                    LogEventInfo firstLogEvent = null;

                    foreach (var ev in bucket.Value)
                    {
                        if (firstLogEvent == null)
                        {
                            firstLogEvent = ev.LogEvent;
                        }

                        var bytes = GetBytesToWrite(ev.LogEvent);
                        ms.Write(bytes, 0, bytes.Length);
                        pendingContinuations.Add(ev.Continuation);
                    }

                    FlushCurrentFileWrites(fileName, firstLogEvent, ms, pendingContinuations);
                }
            }
        }

        /// <summary>
        ///     Formats the log event for write.
        /// </summary>
        /// <param name="logEvent">The log event to be formatted.</param>
        /// <returns>A string representation of the log event.</returns>
        protected virtual string GetFormattedMessage(LogEventInfo logEvent)
        {
            return Layout.Render(logEvent);
        }

        /// <summary>
        ///     Gets the bytes to be written to the file.
        /// </summary>
        /// <param name="logEvent">Log event.</param>
        /// <returns>Array of bytes that are ready to be written.</returns>
        protected virtual byte[] GetBytesToWrite(LogEventInfo logEvent)
        {
            var renderedText = GetFormattedMessage(logEvent) + NewLineChars;
            return TransformBytes(Encoding.GetBytes(renderedText));
        }

        /// <summary>
        ///     Modifies the specified byte array before it gets sent to a file.
        /// </summary>
        /// <param name="value">The byte array.</param>
        /// <returns>The modified byte array. The function can do the modification in-place.</returns>
        protected virtual byte[] TransformBytes(byte[] value)
        {
            return value;
        }

        private static string ReplaceNumber(string pattern, int value)
        {
            var firstPart = pattern.IndexOf("{#", StringComparison.Ordinal);
            var lastPart = pattern.IndexOf("#}", StringComparison.Ordinal) + 2;
            var numDigits = lastPart - firstPart - 2;

            return pattern.Substring(0, firstPart) + Convert.ToString(value, 10).PadLeft(numDigits, '0') + pattern.Substring(lastPart);
        }

        private void FlushCurrentFileWrites(string currentFileName, LogEventInfo firstLogEvent, MemoryStream ms, List<AsyncContinuation> pendingContinuations)
        {
            Exception lastException = null;

            try
            {
                if (currentFileName != null)
                {
                    if (ShouldAutoArchive(currentFileName, firstLogEvent, (int) ms.Length))
                    {
                        WriteFooterAndUninitialize(currentFileName);
                        InvalidateCacheItem(currentFileName);
                        DoAutoArchive(currentFileName, firstLogEvent);
                    }

                    WriteToFile(currentFileName, ms.ToArray(), false);
                }
            }
            catch (Exception exception)
            {
                if (exception.MustBeRethrown())
                {
                    throw;
                }

                lastException = exception;
            }

            foreach (var cont in pendingContinuations)
            {
                cont(lastException);
            }

            pendingContinuations.Clear();
        }

        private void RecursiveRollingRename(string fileName, string pattern, int archiveNumber)
        {
            if (archiveNumber >= MaxArchiveFiles)
            {
                File.Delete(fileName);
                return;
            }

            if (!File.Exists(fileName))
            {
                return;
            }

            var newFileName = ReplaceNumber(pattern, archiveNumber);
            if (File.Exists(fileName))
            {
                RecursiveRollingRename(newFileName, pattern, archiveNumber + 1);
            }

            InternalLogger.Trace("Renaming {0} to {1}", fileName, newFileName);

            try
            {
                File.Move(fileName, newFileName);
            }
            catch (IOException)
            {
                var dir = Path.GetDirectoryName(newFileName);
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                File.Move(fileName, newFileName);
            }
        }

        private void SequentialArchive(string fileName, string pattern)
        {
            var baseNamePattern = Path.GetFileName(pattern);

            var firstPart = baseNamePattern.IndexOf("{#", StringComparison.Ordinal);
            var lastPart = baseNamePattern.IndexOf("#}", StringComparison.Ordinal) + 2;
            var trailerLength = baseNamePattern.Length - lastPart;

            var fileNameMask = baseNamePattern.Substring(0, firstPart) + "*" + baseNamePattern.Substring(lastPart);

            var dirName = Path.GetDirectoryName(Path.GetFullPath(pattern));
            var nextNumber = -1;
            var minNumber = -1;

            var number2name = new Dictionary<int, string>();

            try
            {
#if SILVERLIGHT
                foreach (string s in Directory.EnumerateFiles(dirName, fileNameMask))
#else
                foreach (var s in Directory.GetFiles(dirName, fileNameMask))
#endif
                {
                    var baseName = Path.GetFileName(s);
                    var number = baseName.Substring(firstPart, baseName.Length - trailerLength - firstPart);
                    int num;

                    try
                    {
                        num = Convert.ToInt32(number, CultureInfo.InvariantCulture);
                    }
                    catch (FormatException)
                    {
                        continue;
                    }

                    nextNumber = Math.Max(nextNumber, num);
                    if (minNumber != -1)
                    {
                        minNumber = Math.Min(minNumber, num);
                    }
                    else
                    {
                        minNumber = num;
                    }

                    number2name[num] = s;
                }

                nextNumber++;
            }
            catch (DirectoryNotFoundException)
            {
                Directory.CreateDirectory(dirName);
                nextNumber = 0;
            }

            if (minNumber != -1)
            {
                var minNumberToKeep = nextNumber - MaxArchiveFiles + 1;
                for (var i = minNumber; i < minNumberToKeep; ++i)
                {
                    string s;

                    if (number2name.TryGetValue(i, out s))
                    {
                        File.Delete(s);
                    }
                }
            }

            var newFileName = ReplaceNumber(pattern, nextNumber);
            File.Move(fileName, newFileName);
        }

        private void DoAutoArchive(string fileName, LogEventInfo ev)
        {
            var fi = new FileInfo(fileName);
            if (!fi.Exists)
            {
                return;
            }

            // Console.WriteLine("DoAutoArchive({0})", fileName);
            string fileNamePattern;

            if (ArchiveFileName == null)
            {
                var ext = Path.GetExtension(fileName);
                fileNamePattern = Path.ChangeExtension(fi.FullName, ".{#}" + ext);
            }
            else
            {
                fileNamePattern = ArchiveFileName.Render(ev);
            }

            switch (ArchiveNumbering)
            {
                case ArchiveNumberingMode.Rolling:
                    RecursiveRollingRename(fi.FullName, fileNamePattern, 0);
                    break;

                case ArchiveNumberingMode.Sequence:
                    SequentialArchive(fi.FullName, fileNamePattern);
                    break;
            }
        }

        private bool ShouldAutoArchive(string fileName, LogEventInfo ev, int upcomingWriteSize)
        {
            if (ArchiveAboveSize == -1 && ArchiveEvery == FileArchivePeriod.None)
            {
                return false;
            }

            DateTime lastWriteTime;
            long fileLength;

            if (!GetFileInfo(fileName, out lastWriteTime, out fileLength))
            {
                return false;
            }

            if (ArchiveAboveSize != -1)
            {
                if (fileLength + upcomingWriteSize > ArchiveAboveSize)
                {
                    return true;
                }
            }

            if (ArchiveEvery != FileArchivePeriod.None)
            {
                string formatString;

                switch (ArchiveEvery)
                {
                    case FileArchivePeriod.Year:
                        formatString = "yyyy";
                        break;

                    case FileArchivePeriod.Month:
                        formatString = "yyyyMM";
                        break;

                    default:
                    case FileArchivePeriod.Day:
                        formatString = "yyyyMMdd";
                        break;

                    case FileArchivePeriod.Hour:
                        formatString = "yyyyMMddHH";
                        break;

                    case FileArchivePeriod.Minute:
                        formatString = "yyyyMMddHHmm";
                        break;
                }

                var ts = lastWriteTime.ToString(formatString, CultureInfo.InvariantCulture);
                var ts2 = ev.TimeStamp.ToString(formatString, CultureInfo.InvariantCulture);

                if (ts != ts2)
                {
                    return true;
                }
            }

            return false;
        }

        private void AutoClosingTimerCallback(object state)
        {
            lock (SyncRoot)
            {
                if (!IsInitialized)
                {
                    return;
                }

                try
                {
                    var timeToKill = DateTime.Now.AddSeconds(-OpenFileCacheTimeout);
                    for (var i = 0; i < recentAppenders.Length; ++i)
                    {
                        if (recentAppenders[i] == null)
                        {
                            break;
                        }

                        if (recentAppenders[i].OpenTime < timeToKill)
                        {
                            for (var j = i; j < recentAppenders.Length; ++j)
                            {
                                if (recentAppenders[j] == null)
                                {
                                    break;
                                }

                                recentAppenders[j].Close();
                                recentAppenders[j] = null;
                            }

                            break;
                        }
                    }
                }
                catch (Exception exception)
                {
                    if (exception.MustBeRethrown())
                    {
                        throw;
                    }

                    InternalLogger.Warn("Exception in AutoClosingTimerCallback: {0}", exception);
                }
            }
        }

        private void WriteToFile(string fileName, byte[] bytes, bool justData)
        {
            if (ReplaceFileContentsOnEachWrite)
            {
                using (var fs = File.Create(fileName))
                {
                    var headerBytes = GetHeaderBytes();
                    var footerBytes = GetFooterBytes();

                    if (headerBytes != null)
                    {
                        fs.Write(headerBytes, 0, headerBytes.Length);
                    }

                    fs.Write(bytes, 0, bytes.Length);
                    if (footerBytes != null)
                    {
                        fs.Write(footerBytes, 0, footerBytes.Length);
                    }
                }

                return;
            }

            var writeHeader = false;

            if (!justData)
            {
                if (!initializedFiles.ContainsKey(fileName))
                {
                    if (DeleteOldFileOnStartup)
                    {
                        try
                        {
                            File.Delete(fileName);
                        }
                        catch (Exception exception)
                        {
                            if (exception.MustBeRethrown())
                            {
                                throw;
                            }

                            InternalLogger.Warn("Unable to delete old log file '{0}': {1}", fileName, exception);
                        }
                    }

                    initializedFiles[fileName] = DateTime.Now;
                    initializedFilesCounter++;
                    writeHeader = true;

                    if (initializedFilesCounter >= 100)
                    {
                        initializedFilesCounter = 0;
                        CleanupInitializedFiles();
                    }
                }

                initializedFiles[fileName] = DateTime.Now;
            }

            //
            // BaseFileAppender.Write is the most expensive operation here
            // so the in-memory data structure doesn't have to be 
            // very sophisticated. It's a table-based LRU, where we move 
            // the used element to become the first one.
            // The number of items is usually very limited so the 
            // performance should be equivalent to the one of the hashtable.
            //
            BaseFileAppender appenderToWrite = null;
            var freeSpot = recentAppenders.Length - 1;

            for (var i = 0; i < recentAppenders.Length; ++i)
            {
                if (recentAppenders[i] == null)
                {
                    freeSpot = i;
                    break;
                }

                if (recentAppenders[i].FileName == fileName)
                {
                    // found it, move it to the first place on the list
                    // (MRU)

                    // file open has a chance of failure
                    // if it fails in the constructor, we won't modify any data structures
                    var app = recentAppenders[i];
                    for (var j = i; j > 0; --j)
                    {
                        recentAppenders[j] = recentAppenders[j - 1];
                    }

                    recentAppenders[0] = app;
                    appenderToWrite = app;
                    break;
                }
            }

            if (appenderToWrite == null)
            {
                var newAppender = appenderFactory.Open(fileName, this);

                if (recentAppenders[freeSpot] != null)
                {
                    recentAppenders[freeSpot].Close();
                    recentAppenders[freeSpot] = null;
                }

                for (var j = freeSpot; j > 0; --j)
                {
                    recentAppenders[j] = recentAppenders[j - 1];
                }

                recentAppenders[0] = newAppender;
                appenderToWrite = newAppender;
            }

            if (writeHeader && !justData)
            {
                var headerBytes = GetHeaderBytes();
                if (headerBytes != null)
                {
                    appenderToWrite.Write(headerBytes);
                }
            }

            appenderToWrite.Write(bytes);
        }

        private byte[] GetHeaderBytes()
        {
            if (Header == null)
            {
                return null;
            }

            var renderedText = Header.Render(LogEventInfo.CreateNullEvent()) + NewLineChars;
            return TransformBytes(Encoding.GetBytes(renderedText));
        }

        private byte[] GetFooterBytes()
        {
            if (Footer == null)
            {
                return null;
            }

            var renderedText = Footer.Render(LogEventInfo.CreateNullEvent()) + NewLineChars;
            return TransformBytes(Encoding.GetBytes(renderedText));
        }

        private void WriteFooterAndUninitialize(string fileName)
        {
            var footerBytes = GetFooterBytes();
            if (footerBytes != null)
            {
                if (File.Exists(fileName))
                {
                    WriteToFile(fileName, footerBytes, true);
                }
            }

            initializedFiles.Remove(fileName);
        }

        private bool GetFileInfo(string fileName, out DateTime lastWriteTime, out long fileLength)
        {
            for (var i = 0; i < recentAppenders.Length; ++i)
            {
                if (recentAppenders[i] == null)
                {
                    break;
                }

                if (recentAppenders[i].FileName == fileName)
                {
                    recentAppenders[i].GetFileInfo(out lastWriteTime, out fileLength);
                    return true;
                }
            }

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

        private void InvalidateCacheItem(string fileName)
        {
            for (var i = 0; i < recentAppenders.Length; ++i)
            {
                if (recentAppenders[i] == null)
                {
                    break;
                }

                if (recentAppenders[i].FileName == fileName)
                {
                    recentAppenders[i].Close();
                    for (var j = i; j < recentAppenders.Length - 1; ++j)
                    {
                        recentAppenders[j] = recentAppenders[j + 1];
                    }

                    recentAppenders[recentAppenders.Length - 1] = null;
                    break;
                }
            }
        }
    }
}

#endif