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
    ///     Base class of <see cref="BeforeWriteRecordEventArgs&lt;T&gt;" /> and <see cref="AfterWriteRecordEventArgs&lt;T&gt;" />
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public abstract class WriteRecordEventArgs<T> : EventArgs
    {
        private readonly int mLineNumber;
        private readonly T mRecord;

        internal WriteRecordEventArgs(T record, int lineNumber)
        {
            mRecord = record;
            mLineNumber = lineNumber;
        }

        /// <summary>The current record.</summary>
        public T Record
        {
            get { return mRecord; }
        }

        /// <summary>The current line number.</summary>
        public int LineNumber
        {
            get { return mLineNumber; }
        }
    }

    /// <summary>
    ///     Arguments for the <see cref="BeforeWriteRecordHandler" />
    /// </summary>
    public sealed class BeforeWriteRecordEventArgs : BeforeWriteRecordEventArgs<object>
    {
        internal BeforeWriteRecordEventArgs(object record, int lineNumber)
            : base(record, lineNumber)
        {
        }
    }

    /// <summary>
    ///     Arguments for the <see cref="BeforeWriteRecordHandler" />
    /// </summary>
    public class BeforeWriteRecordEventArgs<T> : WriteRecordEventArgs<T>
    {
        internal BeforeWriteRecordEventArgs(T record, int lineNumber)
            : base(record, lineNumber)
        {
        }

        /// <summary>Set this property as true if you want to bypass the current record.</summary>
        public bool SkipThisRecord { get; set; }
    }

    /// <summary>
    ///     Arguments for the <see cref="AfterWriteRecordHandler" />
    /// </summary>
    public sealed class AfterWriteRecordEventArgs : AfterWriteRecordEventArgs<object>
    {
        internal AfterWriteRecordEventArgs(object record, int lineNumber, string line)
            : base(record, lineNumber, line)
        {
        }
    }

    /// <summary>
    ///     Arguments for the <see cref="AfterWriteRecordHandler" />
    /// </summary>
    public class AfterWriteRecordEventArgs<T> : WriteRecordEventArgs<T>
    {
        internal AfterWriteRecordEventArgs(T record, int lineNumber, string line) : base(record, lineNumber)
        {
            RecordLine = line;
        }

        /// <summary>The line to be written to the file. WARNING: you can change this and the engines will write it to the file.</summary>
        public string RecordLine { get; set; }
    }
}