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

        private const StringComparison IC = StringComparison.OrdinalIgnoreCase;
        private readonly bool _searchAttributes;
        private readonly Dictionary<string, Field> _nameMap = new Dictionary<string, Field>();
        private static readonly XmlReaderSettings Settings = new XmlReaderSettings {
            IgnoreWhitespace = true,
            IgnoreComments = true
        };

        public FromXmlOperation(string inKey, IEnumerable<KeyValuePair<string, Field>> fields)
            : base(inKey, string.Empty) {

            foreach (var field in fields) {
                if (!_searchAttributes && field.Value.NodeType.Equals("attribute", IC)) {
                    _searchAttributes = true;
                }
                _nameMap[field.Value.Name] = field.Value;
            }

        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {
                if (ShouldRun(row)) {
                    var outerRow = row;
                    var innerRow = new Row();
                    var innerRows = new List<Row>();
                    string startKey = null;

                    using (var reader = XmlReader.Create(new StringReader(row[InKey].ToString()), Settings)) {
                        while (reader.Read()) {

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
