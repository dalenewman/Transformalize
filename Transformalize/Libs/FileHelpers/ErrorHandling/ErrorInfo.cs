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
using Transformalize.Libs.FileHelpers.Attributes;
using Transformalize.Libs.FileHelpers.Converters;
using Transformalize.Libs.FileHelpers.Engines;
using Transformalize.Libs.FileHelpers.Enums;
using Transformalize.Libs.FileHelpers.Helpers;

namespace Transformalize.Libs.FileHelpers.ErrorHandling
{
    /// <summary>
    ///     Contains error information of the <see cref="FileHelperEngine" /> class.
    /// </summary>
    [DelimitedRecord("|")]
    [IgnoreFirst(2)]
#if NET_2_0
    [DebuggerDisplay("Line: {LineNumber}. Error: {ExceptionInfo.Message}.")]
#endif
    public sealed class ErrorInfo
    {
        internal ErrorInfo()
        {
        }

#if NET_2_0
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
#endif
        internal int mLineNumber;

        /// <summary>The line number of the error</summary>
        public int LineNumber
        {
            get { return mLineNumber; }
        }

#if NET_2_0
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
#endif
        [FieldQuoted(QuoteMode.OptionalForBoth)] internal string mRecordString = string.Empty;

        /// <summary>The string of the record of the error.</summary>
        public string RecordString
        {
            get { return mRecordString; }
        }

#if NET_2_0
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
#endif
        [FieldConverter(typeof (ExceptionConverter))] [FieldQuoted(QuoteMode.OptionalForBoth)] internal Exception mExceptionInfo;

        /// <summary>The exception that indicates the error.</summary>
        public Exception ExceptionInfo
        {
            get { return mExceptionInfo; }
        }

        internal class ExceptionConverter : ConverterBase
        {
            public override string FieldToString(object from)
            {
                if (from == null)
                    return String.Empty;
                else
                {
                    if (from is ConvertException)
                        return "In the field '" + ((ConvertException) from).FieldName + "': " + ((ConvertException) from).Message.Replace(StringHelper.NewLine, " -> ");
                    else
                        return ((Exception) from).Message.Replace(StringHelper.NewLine, " -> ");
                }
            }

            public override object StringToField(string from)
            {
                return new Exception(from);
            }
        }
    }
}