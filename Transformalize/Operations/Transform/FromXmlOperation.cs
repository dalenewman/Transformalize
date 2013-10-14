using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Main;

namespace Transformalize.Operations.Transform
{
    public class FromXmlOperation : AbstractOperation {
        private readonly string _outKey;
        private readonly Dictionary<string, string> _map = new Dictionary<string, string>();
        private readonly Dictionary<string, string> _typeMap = new Dictionary<string, string>();
        private static readonly XmlReaderSettings Settings = new XmlReaderSettings {
            IgnoreWhitespace = true,
            IgnoreComments = true
        };

        public FromXmlOperation(string outKey, IParameters parameters) {

            _outKey = outKey;

            foreach (var field in parameters) {
                _map[field.Value.Name] = field.Key;
                // in case of XML, the key should be the field's new alias (if present)
            }

            foreach (var field in parameters) {
                _typeMap[field.Value.Name] = field.Value.SimpleType;
                // in case of XML, the name is the name of the XML element or attribute
            }
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {
                using (var reader = XmlReader.Create(new StringReader(row[_outKey].ToString()), Settings)) {
                    while (reader.Read()) {
                        if (!reader.IsStartElement())
                            continue;
                        while (_map.ContainsKey(reader.Name)) {
                            var name = reader.Name;
                            var value = reader.ReadElementContentAsString();
                            if (value != string.Empty)
                                row[_map[name]] = Common.ConversionMap[_typeMap[name]](value);
                        }
                    }
                }
                yield return row;
            }
        }
    }
}
