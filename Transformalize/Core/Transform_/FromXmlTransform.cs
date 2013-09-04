using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Transformalize.Core.Parameters_;
using Transformalize.Libs.Rhino.Etl.Core;

namespace Transformalize.Core.Transform_
{
    public class FromXmlTransform : AbstractTransform
    {
        private readonly string _xmlField;
        private readonly XmlReaderSettings _settings = new XmlReaderSettings { IgnoreWhitespace = true, IgnoreComments = true };
        private readonly Dictionary<string, string> _map = new Dictionary<string, string>();
        private readonly Dictionary<string, string> _typeMap = new Dictionary<string, string>();

        public FromXmlTransform(string xmlField, IParameters parameters)
            : base(parameters)
        {
            RequiresRow = true;
            Name = "From XML";

            _xmlField = xmlField;

            foreach (var field in Parameters)
            {
                _map[field.Value.Name] = field.Key;  // in case of XML, the key should be the field's new alias (if present)
            }

            foreach (var field in Parameters)
            {
                _typeMap[field.Value.Name] = field.Value.SimpleType; // in case of XML, the name is the name of the XML element or attribute
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