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
using System.Text;
using System.Xml;
using Transformalize.Libs.FileHelpers.Enums;

namespace Transformalize.Libs.FileHelpers.RunTime
{
    /// <summary>Used to build the ConverterAttribute for the run time classes.</summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public sealed class ConverterBuilder
    {
        private string mArg1 = string.Empty;
        private string mArg2 = string.Empty;
        private string mArg3 = string.Empty;
        private ConverterKind mKind = ConverterKind.None;
        private string mTypeName = string.Empty;

        internal ConverterBuilder()
        {
        }

        /// <summary>The ConverterKind to be used , like the one in the FieldConverterAttribute.</summary>
        public ConverterKind Kind
        {
            get { return mKind; }
            set { mKind = value; }
        }

        /// <summary>The type name of your custom converter.</summary>
        public string TypeName
        {
            get { return mTypeName; }
            set { mTypeName = value; }
        }

        /// <summary>The first argument pased to the converter.</summary>
        public string Arg1
        {
            get { return mArg1; }
            set { mArg1 = value; }
        }

        /// <summary>The first argument pased to the converter.</summary>
        public string Arg2
        {
            get { return mArg2; }
            set { mArg2 = value; }
        }


        /// <summary>The first argument pased to the converter.</summary>
        public string Arg3
        {
            get { return mArg3; }
            set { mArg3 = value; }
        }


        internal void WriteXml(XmlHelper writer)
        {
            if (mKind == ConverterKind.None && mTypeName == string.Empty) return;

            writer.mWriter.WriteStartElement("Converter");

            writer.WriteAttribute("Kind", Kind.ToString(), "None");
            writer.WriteAttribute("TypeName", mTypeName, string.Empty);

            writer.WriteAttribute("Arg1", Arg1, string.Empty);
            writer.WriteAttribute("Arg2", Arg2, string.Empty);
            writer.WriteAttribute("Arg3", Arg3, string.Empty);

            writer.mWriter.WriteEndElement();
        }

        internal void LoadXml(XmlNode node)
        {
            var attb = node.Attributes["Kind"];
            if (attb != null) Kind = (ConverterKind) Enum.Parse(typeof (ConverterKind), attb.InnerText);

            attb = node.Attributes["TypeName"];
            if (attb != null) TypeName = attb.InnerText;

            attb = node.Attributes["Arg1"];
            if (attb != null) Arg1 = attb.InnerText;

            attb = node.Attributes["Arg2"];
            if (attb != null) Arg2 = attb.InnerText;

            attb = node.Attributes["Arg3"];
            if (attb != null) Arg3 = attb.InnerText;
        }


        internal string GetConverterCode(NetLanguage leng)
        {
            var sb = new StringBuilder();

            if (mKind != ConverterKind.None)
                sb.Append("FieldConverter(ConverterKind." + mKind.ToString());
            else if (mTypeName != string.Empty)
            {
                if (leng == NetLanguage.CSharp)
                    sb.Append("FieldConverter(typeof(" + mTypeName + ")");
                else if (leng == NetLanguage.VbNet)
                    sb.Append("FieldConverter(GetType(" + mTypeName + ")");
            }
            else
                return string.Empty;

            if (mArg1 != null && mArg1 != string.Empty)
            {
                sb.Append(", \"" + mArg1 + "\"");

                if (mArg2 != null && mArg2 != string.Empty)
                {
                    sb.Append(", \"" + mArg2 + "\"");

                    if (mArg3 != null && mArg3 != string.Empty)
                    {
                        sb.Append(", \"" + mArg3 + "\"");
                    }
                }
            }

            sb.Append(")");

            return sb.ToString();
        }
    }
}