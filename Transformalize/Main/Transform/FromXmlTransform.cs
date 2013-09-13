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

using System.Collections.Generic;
using System.IO;
using System.Xml;
using Transformalize.Libs.Rhino.Etl;

namespace Transformalize.Main
{
    public class FromXmlTransform : AbstractTransform
    {
        private readonly Dictionary<string, string> _map = new Dictionary<string, string>();

        private readonly XmlReaderSettings _settings = new XmlReaderSettings
                                                           {
                                                               IgnoreWhitespace = true,
                                                               IgnoreComments = true
                                                           };

        private readonly Dictionary<string, string> _typeMap = new Dictionary<string, string>();
        private readonly string _xmlField;

        public FromXmlTransform(string xmlField, IParameters parameters)
            : base(parameters)
        {
            RequiresRow = true;
            Name = "From XML";

            _xmlField = xmlField;

            foreach (var field in Parameters)
            {
                _map[field.Value.Name] = field.Key;
                // in case of XML, the key should be the field's new alias (if present)
            }

            foreach (var field in Parameters)
            {
                _typeMap[field.Value.Name] = field.Value.SimpleType;
                // in case of XML, the name is the name of the XML element or attribute
            }
        }

        public override void Transform(ref Row row, string resultKey)
        {
            using (var reader = XmlReader.Create(new StringReader(row[_xmlField].ToString()), _settings))
            {
                while (reader.Read())
                {
                    if (!reader.IsStartElement()) continue;
                    while (_map.ContainsKey(reader.Name))
                    {
                        var name = reader.Name;
                        var value = reader.ReadElementContentAsString();
                        if (value != string.Empty)
                            row[_map[name]] = Common.ConversionMap[_typeMap[name]](value);
                    }
                }
            }
        }
    }
}