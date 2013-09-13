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