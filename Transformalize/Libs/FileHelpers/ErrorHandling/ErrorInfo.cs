#region License
// /*
// See license included in this library folder.
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