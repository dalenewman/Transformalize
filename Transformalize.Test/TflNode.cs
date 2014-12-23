using System;
using System.Collections.Generic;
using System.Globalization;
using Transformalize.Libs.Lucene.Net.Util;
using Transformalize.Libs.NanoXml;
using Transformalize.Libs.Newtonsoft.Json.Converters;

namespace Transformalize.Test {

    public sealed class TflMeta {
        public object Value { get; set; }
        public bool Required { get; set; }
        public bool Unique { get; set; }
        public bool Set { get; set; }

        public TflMeta(object value, bool required = false, bool unique = false) {
            Value = value;
            Required = required;
            Unique = unique;
        }

    }

    public abstract class TflNode {

        private readonly NanoXmlNode _node;
        private readonly Dictionary<string, TflMeta> _attributes = new Dictionary<string, TflMeta>(StringComparer.Ordinal);
        private readonly List<string> _required = new List<string>();
        private readonly List<string> _unique = new List<string>();
        private readonly Dictionary<string, List<TflNode>> _elements = new Dictionary<string, List<TflNode>>();

        private static Dictionary<Type, Func<string, object>> Converter {
            get {
                return new Dictionary<Type, Func<string, object>> {
                {typeof(String), (x => x)},
                {typeof(Guid), (x => Guid.Parse(x))},
                {typeof(Int16), (x => Convert.ToInt16(x))},
                {typeof(Int32), (x => Convert.ToInt32(x))},
                {typeof(Int64), (x => Convert.ToInt64(x))},
                {typeof(Double), (x => Convert.ToDouble(x))},
                {typeof(Decimal), (x => Decimal.Parse(x, NumberStyles.Float | NumberStyles.AllowThousands | NumberStyles.AllowCurrencySymbol, (IFormatProvider)CultureInfo.CurrentCulture.GetFormat(typeof(NumberFormatInfo))))},
                {typeof(Char), (x => Convert.ToChar(x))},
                {typeof(DateTime), (x => Convert.ToDateTime(x))},
                {typeof(Boolean), (x => Convert.ToBoolean(x))},
                {typeof(Single), (x => Convert.ToSingle(x))},
                {typeof(Byte), (x => Convert.ToByte(x))}
            };
            }
        }

        // Get an element by index
        public TflNode this[string element, int i] {
            get { return _elements[element][i]; }
        }

        // Get an attribute by name
        public TflMeta this[string name] {
            get { return _attributes[name]; }
        }

        protected Dictionary<string, Func<NanoXmlNode, TflNode>> ElementLoaders = new Dictionary<string, Func<NanoXmlNode, TflNode>>();

        protected TflNode(NanoXmlNode node) {
            _node = node;
        }

        protected void Element<T>(string element) {
            ElementLoaders[element] = n => ((TflNode)Activator.CreateInstance(typeof(T), n));
        }

        protected void Attribute<T>(string name, T value, bool required = false, bool unique = false) {
            _attributes[name] = new TflMeta(value, required, unique);
            if (required) {
                _required.Add(name);
            }
            if (unique) {
                _unique.Add(name);
            }
        }

        // Convenience method for keys
        protected void Key(string name, bool unique = true) {
            Attribute(name, string.Empty, true, unique);
        }

        public TflNode Load() {
            if (_node.Attributes.Count > 0) {
                for (var i = 0; i < _node.Attributes.Count; i++) {
                    var attribute = _node.Attributes[i];
                    if (_attributes.ContainsKey(attribute.Name)) {

                        var data = _attributes[attribute.Name];

                        if (data.Value is string) {
                            if (attribute.Value != null) {
                                data.Set = true;
                                data.Value = attribute.Value;
                            }
                        } else {
                            if (attribute.Value != null) {
                                data.Set = true;
                                data.Value = Converter[data.Value.GetType()](attribute.Value);
                            }
                        }
                    } else {
                        //throw;
                        Console.WriteLine("Invalid attribute {0} in {1}.", attribute.Name, _node.Name);
                    }
                }

                //check to make sure required were set
                for (int i = 0; i < _required.Count; i++) {
                    if (_attributes[_required[i]].Set == false) {
                        //throw;
                        Console.WriteLine("Required attribute {0} in {1} must be set.", _required[i], _node.Name);
                    }
                }
            }

            for (var i = 0; i < _node.SubNodes.Count; i++) {
                var node = _node.SubNodes[i];
                if (ElementLoaders.ContainsKey(node.Name)) {
                    _elements[node.Name] = new List<TflNode>();
                    for (var j = 0; j < node.SubNodes.Count; j++) {
                        var add = node.SubNodes[j];
                        if (add.Name == "add") {
                            var tflNode = ElementLoaders[node.Name](add).Load();
                            // check for duplicates of unique attributes
                            if (_elements[node.Name].Count > 0) {
                                for (int k = 0; k < _elements[node.Name].Count; k++) {
                                    foreach (var pair in _elements[node.Name][k].Attributes) {
                                        if (pair.Value.Unique && tflNode.Attributes[pair.Key].Value.Equals(pair.Value.Value)) {
                                            //throw;
                                            Console.WriteLine("Duplicate {0} value {1} in {2}.", pair.Key, pair.Value.Value, node.Name);
                                        }
                                    }
                                }
                            }
                            _elements[node.Name].Add(tflNode);
                        } else {
                            Console.WriteLine("Invalid element {0} in {1}.", add.Name, node.Name);
                        }
                    }
                } else {
                    //throw;
                    Console.WriteLine("Invalid element {0} in {1}.", node.Name, _node.Name);
                }
            }

            return this;
        }

        public Dictionary<string, TflMeta> Attributes {
            get { return _attributes; }
        }

        public Dictionary<string, List<TflNode>> Elements {
            get { return _elements; }
        }

    }
}