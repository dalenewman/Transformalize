using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Transformalize.Core.Field_;
using Transformalize.Core.Fields_;
using Transformalize.Core.Parameter_;
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
        private readonly Dictionary<string, Func<string, object>> _conversionMap = new Dictionary<string, Func<string, object>> {
            { "string", (x => x) },
            { "xml" , (x => x)},
            { "int32", (x => Convert.ToInt32(x)) },
            { "double", (x => Convert.ToDouble(x)) },
            { "datetime", (x => Convert.ToDateTime(x)) },
            { "boolean", (x=> Convert.ToBoolean(x)) }
        };

        public override string Name { get { return "FromXml Transform"; } }
        public override bool RequiresParameters { get { return false; } }

        public FromXmlTransform(string xmlField, IParameters parameters) : base(parameters)
        {
            RequiresRow = true;

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
                        row[_map[reader.Name]] = _conversionMap[_typeMap[reader.Name]](reader.ReadElementContentAsString());
                    }
                }  
            }

            //var x = XDocument.Parse(row[_xmlField].ToString());
            //var nodes = _xmlRoot.Equals(string.Empty) ? x.Descendants() : x.Descendants(_xmlRoot);
            //foreach (var node in nodes) { }
        }

    }
}