#region License
// /*
// See license included in this library folder.
// */
#endregion

using System;
using System.ComponentModel;

namespace Transformalize.Libs.FileHelpers.Events
{
    /// <summary>
    ///     Base class of <see cref="BeforeReadRecordEventArgs" /> and <see cref="AfterReadRecordEventArgs" />
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public abstract class ReadRecordEventArgs : EventArgs
    {
        private readonly int mLineNumber;
        private readonly string mRecordLine;

        internal ReadRecordEventArgs(string line, int lineNumber)
        {
            mRecordLine = line;
            mLineNumber = lineNumber;
        }

        /// <summary>The current line number.</summary>
        public int LineNumber
        {
            get { return mLineNumber; }
        }

        /// <summary>The just read record line.</summary>
        public string RecordLine
        {
            get { return mRecordLine; }
        }
    }

    /// <summary>
    ///     Arguments for the <see cref="BeforeReadRecordHandler" />
    /// </summary>
    public sealed class BeforeReadRecordEventArgs : BeforeReadRecordEventArgs<object>
    {
        internal BeforeReadRecordEventArgs(string line)
            : this(line, -1)
        {
        }

        internal BeforeReadRecordEventArgs(string line, int lineNumber)
            : base(line, lineNumber)
        {
        }
    }

    /// <summary>
    ///     Arguments for the <see cref="BeforeReadRecordHandler" />
    /// </summary>
    public class BeforeReadRecordEventArgs<T> : ReadRecordEventArgs
    {
        internal BeforeReadRecordEventArgs(string line) : this(line, -1)
        {
        }

        internal BeforeReadRecordEventArgs(string line, int lineNumber) : base(line, lineNumber)
        {
        }

        /// <summary>Set this property to true if you want to bypass the current line.</summary>
        public bool SkipThisRecord { get; set; }
    }

    /// <summary>
    ///     Arguments for the <see cref="AfterReadRecordHandler" />
    /// </summary>
    public sealed class AfterReadRecordEventArgs : AfterReadRecordEventArgs<object>
    {
        internal AfterReadRecordEventArgs(string line, object newRecord)
            : this(line, newRecord, -1)
        {
        }

        internal AfterReadRecordEventArgs(string line, object newRecord, int lineNumber)
            : base(line, newRecord, lineNumber)
        {
        }
    }

    /// <summary>
    ///     Arguments for the <see cref="AfterReadRecordHandler" />
    /// </summary>
    public class AfterReadRecordEventArgs<T> : ReadRecordEventArgs
    {
        internal AfterReadRecordEventArgs(string line, T newRecord) : this(line, newRecord, -1)
        {
        }

        internal AfterReadRecordEventArgs(string line, T newRecord, int lineNumber) : base(line, lineNumber)
        {
            Record = newRecord;
        }

        /// <summary>The current record.</summary>
        public T Record { get; set; }

        /// <summary>Set this property to true if you want to bypass the current record.</summary>
        public bool SkipThisRecord { get; set; }
    }
}