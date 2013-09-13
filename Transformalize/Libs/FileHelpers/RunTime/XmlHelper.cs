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

using System.IO;
using System.Xml;

namespace Transformalize.Libs.FileHelpers.RunTime
{
    internal sealed class XmlHelper
    {
        internal XmlTextReader mReader;
        internal XmlTextWriter mWriter;

        public void BeginWriteFile(string filename)
        {
            mWriter = new XmlTextWriter(new StreamWriter(filename));
            mWriter.Formatting = Formatting.Indented;
            mWriter.Indentation = 4;
        }

        public void BeginReadFile(string filename)
        {
            mReader = new XmlTextReader(new StreamReader(filename));
        }

        public void WriteElement(string element, string valueStr)
        {
            mWriter.WriteStartElement(element);
            mWriter.WriteString(valueStr);
            mWriter.WriteEndElement();
        }

        public void WriteElement(string element, string valueStr, string defaultVal)
        {
            if (valueStr != defaultVal)
                WriteElement(element, valueStr);
        }

        public void WriteElement(string element, bool mustWrite)
        {
            if (mustWrite)
            {
                mWriter.WriteStartElement(element);
                mWriter.WriteEndElement();
            }
        }

        public void WriteAttribute(string attb, string valueStr, string defaultVal)
        {
            if (valueStr != defaultVal)
                WriteAttribute(attb, valueStr);
        }

        public void WriteAttribute(string attb, string valueStr)
        {
            mWriter.WriteStartAttribute(attb, string.Empty);
            mWriter.WriteString(valueStr);
            mWriter.WriteEndAttribute();
        }

        public void EndWrite()
        {
            mWriter.Close();
            mWriter = null;
        }

        public void ReadToNextElement()
        {
            while (mReader.Read())
                if (mReader.NodeType == XmlNodeType.Element)
                    return;
        }
    }
}