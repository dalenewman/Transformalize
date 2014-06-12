using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Transformalize.Libs;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Main;

namespace Transformalize.Operations.Transform {
    public class FromNanoXmlOperation : ShouldRunOperation {

        private const StringComparison IC = StringComparison.OrdinalIgnoreCase;
        private readonly Dictionary<string, Field> _attributes = new Dictionary<string, Field>();
        private readonly Dictionary<string, Field> _elements = new Dictionary<string, Field>();
        private readonly bool _searchAttributes;
        private readonly Dictionary<string, Func<string, object>> _converter = Common.ConversionMap;
        private readonly int _total;

        public FromNanoXmlOperation(string inKey, IEnumerable<Field> fields)
            : base(inKey, string.Empty) {


            foreach (var field in fields) {
                if (field.NodeType.Equals("attribute", IC)) {
                    _attributes[field.Name] = field;
                } else {
                    _elements[field.Name] = field;
                }
            }

            _searchAttributes = _attributes.Count > 0;
            _total = _elements.Count + _attributes.Count;

            Name = string.Format("FromNanoXmlOperation (in:{0})", inKey);
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {
                if (ShouldRun(row)) {
                    var count = 0;
                    var xml = new NanoXmlDocument(row[InKey].ToString());
                    if (_elements.ContainsKey(xml.RootNode.Name)) {
                        var field = _elements[xml.RootNode.Name];
                        row[field.Alias] = _converter[field.SimpleType](xml.RootNode.Value ?? (field.ReadInnerXml ? xml.RootNode.InnerText() : xml.RootNode.OuterText()));
                        count++;
                    }

                    var subNodes = xml.RootNode.SubNodes.ToArray();
                    while (subNodes.Any()) {
                        var nextNodes = new List<NanoXmlNode>();
                        foreach (var node in subNodes) {
                            if (_elements.ContainsKey(node.Name)) {
                                var field = _elements[node.Name];
                                count++;
                                var value = node.Value ?? (field.ReadInnerXml ? node.InnerText() : node.OuterText());
                                if (!string.IsNullOrEmpty(value)) {
                                    row[field.Alias] = _converter[field.SimpleType](value);
                                }
                            }
                            if (_searchAttributes) {
                                foreach (var attribute in node.Attributes.Where(attribute => _attributes.ContainsKey(attribute.Name))) {
                                    var field = _attributes[attribute.Name];
                                    count++;
                                    if (!string.IsNullOrEmpty(attribute.Value)) {
                                        row[field.Alias] = _converter[field.SimpleType](attribute.Value);
                                    }
                                }
                            }
                            if (count < _total) {
                                nextNodes.AddRange(node.SubNodes);
                            }
                        }
                        subNodes = nextNodes.ToArray();
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
