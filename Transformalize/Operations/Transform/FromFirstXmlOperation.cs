using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Xml;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Main;

namespace Transformalize.Operations.Transform {
    public class FromFirstXmlOperation : ShouldRunOperation {

        private const StringComparison IC = StringComparison.OrdinalIgnoreCase;
        private readonly bool _searchAttributes;
        private readonly Dictionary<string, Field> _nameMap = new Dictionary<string, Field>();
        private static readonly XmlReaderSettings Settings = new XmlReaderSettings {
            IgnoreWhitespace = true,
            IgnoreComments = true
        };

        private readonly int _total;

        public FromFirstXmlOperation(string inKey, IEnumerable<Field> fields)
            : base(inKey, string.Empty) {

            foreach (var field in fields) {
                if (!_searchAttributes && field.NodeType.Equals("attribute", IC)) {
                    _searchAttributes = true;
                }
                _nameMap[field.Name] = field;
            }
            _total = _nameMap.Count;
            Name = string.Format("FromFirstXmlOperation (in:{0})", inKey);
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {
                if (ShouldRun(row)) {
                    var xml = row[InKey].ToString();

                    if (xml.Equals(string.Empty)) {
                        yield return row;
                        continue;
                    }

                    using (var reader = XmlReader.Create(new StringReader(xml), Settings)) {
                        var count = 0;
                        while (reader.Read() && count < _total) {
                            if (_nameMap.ContainsKey(reader.Name)) {
                                // must while here because reader.Read*Xml advances the reader
                                while (_nameMap.ContainsKey(reader.Name) && reader.IsStartElement()) {
                                    count++;
                                    var field = _nameMap[reader.Name];
                                    var value = field.ReadInnerXml ? reader.ReadInnerXml() : reader.ReadOuterXml();
                                    if (value != string.Empty) {
                                        row[field.Alias] = Common.ConversionMap[field.SimpleType](value);
                                    }
                                }
                            } else if (_searchAttributes && reader.HasAttributes) {
                                for (var i = 0; i < reader.AttributeCount; i++) {
                                    reader.MoveToNextAttribute();
                                    if (!_nameMap.ContainsKey(reader.Name))
                                        continue;

                                    count++;
                                    var field = _nameMap[reader.Name];
                                    if (!string.IsNullOrEmpty(reader.Value)) {
                                        row[field.Alias] = Common.ConversionMap[field.SimpleType](reader.Value);
                                    }
                                }
                            }
                        }
                    }
                    yield return row;
                } else {
                    Interlocked.Increment(ref SkipCount);
                    yield return row;
                }
            }
        }

    }

}
