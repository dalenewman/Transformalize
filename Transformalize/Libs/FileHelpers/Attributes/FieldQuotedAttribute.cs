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
using Transformalize.Libs.FileHelpers.Enums;

namespace Transformalize.Libs.FileHelpers.Attributes
{
    /// <summary>Indicates that the field must be read and written like a Quoted String. (by default "")</summary>
    /// <remarks>
    ///     See the <a href="attributes.html">Complete Attributes List</a> for more clear info and examples of each one.
    /// </remarks>
    /// <seealso href="attributes.html">Attributes List</seealso>
    /// <seealso href="quick_start.html">Quick Start Guide</seealso>
    /// <seealso href="examples.html">Examples of Use</seealso>
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class FieldQuotedAttribute : Attribute
    {
        internal char QuoteChar;

        internal QuoteMode QuoteMode = QuoteMode.AlwaysQuoted;

        internal MultilineMode QuoteMultiline = MultilineMode.AllowForBoth;

        /// <summary>Indicates that the field must be read and written like a Quoted String with double quotes.</summary>
        public FieldQuotedAttribute() : this('\"')
        {
        }

        /// <summary>Indicates that the field must be read and written like a Quoted String with the specified char.</summary>
        /// <param name="quoteChar">The char used to quote the string.</param>
        public FieldQuotedAttribute(char quoteChar) : this(quoteChar, QuoteMode.OptionalForRead, MultilineMode.AllowForBoth)
        {
        }

        /// <summary>Indicates that the field must be read and written like a "Quoted String"  (that can be optional depending of the mode).</summary>
        /// <param name="mode">Indicates if the handling of optionals in the quoted field.</param>
        public FieldQuotedAttribute(QuoteMode mode) : this('\"', mode)
        {
        }

        /// <summary>Indicates that the field must be read and written like a Quoted String (that can be optional).</summary>
        /// <param name="mode">Indicates if the handling of optionals in the quoted field.</param>
        /// <param name="multiline">Indicates if the field can span multiple lines.</param>
        public FieldQuotedAttribute(QuoteMode mode, MultilineMode multiline) : this('"', mode, multiline)
        {
        }

        /// <summary>Indicates that the field must be read and written like a Quoted String (that can be optional).</summary>
        /// <param name="quoteChar">The char used to quote the string.</param>
        /// <param name="mode">Indicates if the handling of optionals in the quoted field.</param>
        public FieldQuotedAttribute(char quoteChar, QuoteMode mode) : this(quoteChar, mode, MultilineMode.AllowForBoth)
        {
        }

        /// <summary>Indicates that the field must be read and written like a Quoted String (that can be optional).</summary>
        /// <param name="quoteChar">The char used to quote the string.</param>
        /// <param name="mode">Indicates if the handling of optionals in the quoted field.</param>
        /// <param name="multiline">Indicates if the field can span multiple lines.</param>
        public FieldQuotedAttribute(char quoteChar, QuoteMode mode, MultilineMode multiline)
        {
            QuoteChar = quoteChar;
            QuoteMode = mode;
            QuoteMultiline = multiline;
        }

        /// <summary>Indicates that the field must be read and written like a Quoted String with double quotes.</summary>
        /// <param name="multiline">Indicates if the field can span multiple lines.</param>
        public FieldQuotedAttribute(MultilineMode multiline) : this('\"', QuoteMode.OptionalForRead, multiline)
        {
        }
    }
}