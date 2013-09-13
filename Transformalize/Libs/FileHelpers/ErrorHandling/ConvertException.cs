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

namespace Transformalize.Libs.FileHelpers.ErrorHandling
{
    /// <summary>
    ///     Indicates that a string value can't be converted to a dest type.
    /// </summary>
    public sealed class ConvertException : FileHelpersException
    {
#if NET_2_0
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
#endif
        internal string mFieldName = null;
#if NET_2_0
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
#endif
        internal int mLineNumber = -1;
#if NET_2_0
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
#endif
        internal int mColumnNumber = -1;
#if NET_2_0
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
#endif
        private readonly string mFieldStringValue;

#if NET_2_0
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
#endif
        private readonly string mMessageExtra = string.Empty;

#if NET_2_0
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
#endif
        private readonly Type mFieldType;

        /// <summary>The destination type.</summary>
        public Type FieldType
        {
            get { return mFieldType; }
        }

        /// <summary>The value that cant be converterd. (null for unknown)</summary>
        public string FieldStringValue
        {
            get { return mFieldStringValue; }
        }

        /// <summary>Extra info about the error.</summary>
        public string MessageExtra
        {
            get { return mMessageExtra; }
        }

        /// <summary>The name of the field related to the exception. (null for unknown)</summary>
        public string FieldName
        {
            get { return mFieldName; }
        }

        /// <summary>The line where the error was found. (-1 is unknown)</summary>
        public int LineNumber
        {
            get { return mLineNumber; }
        }

        /// <summary>The estimate column where the error was found. (-1 is unknown)</summary>
        public int ColumnNumber
        {
            get { return mColumnNumber; }
        }


        /// <summary>
        ///     Create a new ConvertException object
        /// </summary>
        /// <param name="origValue">The value to convert.</param>
        /// <param name="destType">The destination Type.</param>
        public ConvertException(string origValue, Type destType)
            : this(origValue, destType, string.Empty)
        {
        }


        /// <summary>
        ///     Create a new ConvertException object
        /// </summary>
        /// <param name="origValue">The value to convert.</param>
        /// <param name="destType">The destination Type.</param>
        /// <param name="extraInfo">Aditional info of the error.</param>
        public ConvertException(string origValue, Type destType, string extraInfo)
            : this(origValue, destType, string.Empty, -1, -1, extraInfo)
        {
        }

        /// <summary>
        ///     Create a new ConvertException object
        /// </summary>
        /// <param name="origValue">The value to convert.</param>
        /// <param name="destType">The destination Type.</param>
        /// <param name="extraInfo">Aditional info of the error.</param>
        /// <param name="columnNumber">The estimated column number.</param>
        /// <param name="lineNumber">The line where the error was found.</param>
        /// <param name="fieldName">The name of the field with the error</param>
        public ConvertException(string origValue, Type destType, string fieldName, int lineNumber, int columnNumber, string extraInfo)
            : base(MessageBuilder(origValue, destType, fieldName, lineNumber, columnNumber, extraInfo))
        {
            mFieldStringValue = origValue;
            mFieldType = destType;
            mLineNumber = lineNumber;
            mColumnNumber = columnNumber;
            mFieldName = fieldName;
            mMessageExtra = extraInfo;
        }

        private static string MessageBuilder(string origValue, Type destType, string fieldName, int lineNumber, int columnNumber, string extraInfo)
        {
            var res = string.Empty;
            if (lineNumber >= 0)
                res += "Line: " + lineNumber.ToString() + ". ";

            if (columnNumber >= 0)
                res += "Column: " + columnNumber.ToString() + ". ";

            if (!string.IsNullOrEmpty(fieldName))
                res += "Field: " + fieldName + ". ";

            if (origValue != null && destType != null)
                res += "Error Converting '" + origValue + "' to type: '" + destType.Name + "'. ";

            res += extraInfo;

            return res;
        }


        internal static ConvertException ReThrowException(ConvertException ex, string fieldName, int line, int column)
        {
            return new ConvertException(ex.FieldStringValue, ex.mFieldType, fieldName, line, column, ex.mMessageExtra);
        }
    }
}