using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Main;

namespace Transformalize.Operations.Transform {
    public class FromXmlOperation : ShouldRunOperation {
        private readonly string _root;
        private readonly bool _findRoot;

        private const StringComparison IC = StringComparison.OrdinalIgnoreCase;
        private readonly bool _searchAttributes;
        private readonly Dictionary<string, Field> _nameMap = new Dictionary<string, Field>();
        private static readonly XmlReaderSettings Settings = new XmlReaderSettings {
            IgnoreWhitespace = true,
            IgnoreComments = true
        };

        public FromXmlOperation(string inKey, string root, IEnumerable<Field> fields)
            : base(inKey, string.Empty) {
            _root = root;
            _findRoot = !string.IsNullOrEmpty(root);

            foreach (var field in fields) {
                if (!_searchAttributes && field.NodeType.Equals("attribute", IC)) {
                    _searchAttributes = true;
                }
                _nameMap[field.Name] = field;
            }
            Name = string.Format("FromXmlOperation (in:{0})", inKey);
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {
                if (ShouldRun(row)) {
                    var outerRow = row;
                    var innerRow = new Row();
                    var innerRows = new List<Row>();
                    string startKey = null;

                    var xml = row[InKey].ToString().Trim();

                    if (!xml.Equals(string.Empty)) {
                        using (var reader = XmlReader.Create(new StringReader(xml), Settings)) {

                            if (_findRoot) {
                                do {
                                    reader.Read();
                                } while (reader.Name != _root);
                            } else {
                                reader.Read();
                            }

                            do {
                                if (_nameMap.ContainsKey(reader.Name)) {

                                    // must while here because reader.Read*Xml advances the reader
                                    while (_nameMap.ContainsKey(reader.Name) && reader.IsStartElement()) {
                                        InnerRow(ref startKey, reader.Name, ref innerRow, ref outerRow, ref innerRows);

                                        var field = _nameMap[reader.Name];
                                        var value = field.ReadInnerXml ? reader.ReadInnerXml() : reader.ReadOuterXml();
                                        if (value != string.Empty)
                                            innerRow[field.Alias] = Common.ConversionMap[field.SimpleType](value);
                                    }

                                } else if (_searchAttributes && reader.HasAttributes) {
                                    for (var i = 0; i < reader.AttributeCount; i++) {
                                        reader.MoveToNextAttribute();
                                        if (!_nameMap.ContainsKey(reader.Name))
                                            continue;

                                        InnerRow(ref startKey, reader.Name, ref innerRow, ref outerRow, ref innerRows);

                                        var field = _nameMap[reader.Name];
                                        if (!string.IsNullOrEmpty(reader.Value)) {
                                            innerRow[field.Alias] = Common.ConversionMap[field.SimpleType](reader.Value);
                                        }
                                    }
                                }
                                if (_findRoot && !reader.IsStartElement() && reader.Name == _root) {
                                    break;
                                }
                            } while (reader.Read());
                        }
                    }
                    AddInnerRow(ref innerRow, ref outerRow, ref innerRows);
                    foreach (var r in innerRows) {
                        yield return r;
                    }
                } else {
                    Interlocked.Increment(ref SkipCount);
                    yield return row;
                }
            }
        }

        private static bool ShouldYieldRow(ref string startKey, string key) {
            if (startKey == null) {
                startKey = key;
            } else if (startKey.Equals(key)) {
                return true;
            }
            return false;
        }

        private static void InnerRow(ref string startKey, string key, ref Row innerRow, ref Row outerRow, ref List<Row> innerRows) {
            if (!ShouldYieldRow(ref startKey, key))
                return;

            AddInnerRow(ref innerRow, ref outerRow, ref innerRows);
        }

        private static void AddInnerRow(ref Row innerRow, ref Row outerRow, ref List<Row> innerRows) {
            var r = innerRow.Clone();

            foreach (var column in outerRow.Columns.Where(column => !r.ContainsKey(column))) {
                r[column] = outerRow[column];
            }

            innerRows.Add(r);
            innerRow = new Row();
        }
    }

}
