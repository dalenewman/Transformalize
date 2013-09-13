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
using System.Xml;
using Transformalize.Libs.FileHelpers.Enums;

namespace Transformalize.Libs.FileHelpers.RunTime
{
    /// <summary>Used to create fields that are part of a dilimited record class.</summary>
    public sealed class DelimitedFieldBuilder : FieldBuilder
    {
        internal DelimitedFieldBuilder(string fieldName, string fieldType) : base(fieldName, fieldType)
        {
        }

        internal DelimitedFieldBuilder(string fieldName, Type fieldType) : base(fieldName, fieldType)
        {
        }

#if NET_2_0
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
#endif
        private bool mFieldQuoted;

        /// <summary>Indicates if the field is quoted with some char. (works with QuoteMode and QuoteChar)</summary>
        public bool FieldQuoted
        {
            get { return mFieldQuoted; }
            set { mFieldQuoted = value; }
        }

#if NET_2_0
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
#endif
        private char mQuoteChar = '"';

        /// <summary>Indicates the char used to quote this field. (only used when FieldQuoted is true)</summary>
        public char QuoteChar
        {
            get { return mQuoteChar; }
            set { mQuoteChar = value; }
        }

#if NET_2_0
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
#endif
        private QuoteMode mQuoteMode = QuoteMode.OptionalForRead;

        /// <summary>Indicates the QuoteMode for this field. (only used when FieldQuoted is true)</summary>
        public QuoteMode QuoteMode
        {
            get { return mQuoteMode; }
            set { mQuoteMode = value; }
        }

#if NET_2_0
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
#endif
        private MultilineMode mQuoteMultiline = MultilineMode.AllowForRead;

        /// <summary>Indicates if this quoted field can span multiple lines. (only used when FieldQuoted is true)</summary>
        public MultilineMode QuoteMultiline
        {
            get { return mQuoteMultiline; }
            set { mQuoteMultiline = value; }
        }


        internal override void AddAttributesCode(AttributesBuilder attbs, NetLanguage leng)
        {
            if (mFieldQuoted)
            {
                if (leng == NetLanguage.CSharp)
                {
                    var quoteStr = mQuoteChar.ToString();
                    if (mQuoteChar == '\'') quoteStr = @"\'";

                    attbs.AddAttribute("FieldQuoted('" + quoteStr + "', QuoteMode." + mQuoteMode.ToString() + ", MultilineMode." + mQuoteMultiline.ToString() + ")");
                }
                else if (leng == NetLanguage.VbNet)
                {
                    var quoteStr = mQuoteChar.ToString();
                    if (mQuoteChar == '"') quoteStr = "\"\"";

                    attbs.AddAttribute("FieldQuoted(\"" + quoteStr + "\"c, QuoteMode." + mQuoteMode.ToString() + ", MultilineMode." + mQuoteMultiline.ToString() + ")");
                }
            }
        }

        internal override void WriteHeaderAttributes(XmlHelper writer)
        {
        }

        internal override void WriteExtraElements(XmlHelper writer)
        {
            writer.WriteElement("FieldQuoted", FieldQuoted);
            writer.WriteElement("QuoteChar", QuoteChar.ToString(), "\"");
            writer.WriteElement("QuoteMode", QuoteMode.ToString(), "OptionalForRead");
            writer.WriteElement("QuoteMultiline", QuoteMultiline.ToString(), "AllowForRead");
        }

        internal override void ReadFieldInternal(XmlNode node)
        {
            XmlNode ele;

            FieldQuoted = node["FieldQuoted"] != null;

            ele = node["QuoteChar"];
            if (ele != null) QuoteChar = ele.InnerText[0];

            ele = node["QuoteMode"];
            if (ele != null) QuoteMode = (QuoteMode) Enum.Parse(typeof (QuoteMode), ele.InnerText);

            ele = node["QuoteMultiline"];
            if (ele != null) QuoteMultiline = (MultilineMode) Enum.Parse(typeof (MultilineMode), ele.InnerText);
        }
    }
}