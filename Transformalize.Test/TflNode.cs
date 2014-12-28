using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Transformalize.Extensions;
using Transformalize.Libs.NanoXml;
using Transformalize.Libs.SolrNet.Utils;

namespace Transformalize.Test {
    public abstract class TflNode {

        private readonly Dictionary<string, TflProperty> _properties = new Dictionary<string, TflProperty>(StringComparer.Ordinal);
        private readonly List<string> _requiredProperties = new List<string>();
        private readonly List<string> _uniqueProperties = new List<string>();
        private readonly Dictionary<string, List<TflNode>> _classes = new Dictionary<string, List<TflNode>>();
        private readonly List<string> _requiredClasses = new List<string>();
        private readonly List<string> _problems = new List<string>();

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
            get { return _classes[element][i]; }
        }

        // Get an attribute by name
        public TflProperty this[string name] {
            get { return _properties[name]; }
        }

        protected Dictionary<string, Func<TflNode>> ElementLoaders = new Dictionary<string, Func<TflNode>>();
        protected List<string> Problems { get { return _problems; } }

        protected void Class<T>(string element, bool required = false) {
            ElementLoaders[element] = () => (TflNode) Activator.CreateInstance(typeof(T));
            if (required) {
                _requiredClasses.Add(element);
            }
        }

        protected void Property<T>(string name, T value, bool required = false, bool unique = false) {
            _properties[name] = new TflProperty(value, required, unique);
            if (required) {
                _requiredProperties.Add(name);
            }
            if (unique) {
                _uniqueProperties.Add(name);
            }
        }

        // Convenience method for keys
        protected void Key(string name, bool unique = true) {
            Property(name, string.Empty, true, unique);
        }

        public TflNode Load(NanoXmlNode node, string parentName = null) {
            LoadProperties(node, parentName);
            LoadClasses(node, parentName);
            return this;
        }

        private void LoadClasses(NanoXmlNode n, string parentName) {
            for (var i = 0; i < n.SubNodes.Count; i++) {
                var subNode = n.SubNodes[i];
                if (ElementLoaders.ContainsKey(subNode.Name)) {
                    _classes[subNode.Name] = new List<TflNode>();
                    for (var j = 0; j < subNode.SubNodes.Count; j++) {
                        var add = subNode.SubNodes[j];
                        if (add.Name.Equals("add")) {
                            var tflNode = ElementLoaders[subNode.Name]().Load(add, subNode.Name);
                            // check for duplicates of unique attributes
                            if (_classes[subNode.Name].Count > 0) {
                                for (var k = 0; k < _classes[subNode.Name].Count; k++) {
                                    foreach (var pair in _classes[subNode.Name][k].Properties) {
                                        if (!pair.Value.Unique || !tflNode.Properties[pair.Key].Value.Equals(pair.Value.Value))
                                            continue;

                                        if (tflNode.Properties[pair.Key].Set && pair.Value.Set) {
                                            _problems.Add(string.Format("Duplicate {0} value {1} in {2}.", pair.Key, pair.Value.Value, subNode.Name));
                                        } else {
                                            if (!pair.Value.Value.Equals(string.Empty)) {
                                                _problems.Add(string.Format("Possible duplicate {0} value {1} in {2}.", pair.Key, pair.Value.Value, subNode.Name));
                                            }
                                        }
                                    }
                                }
                            }
                            _classes[subNode.Name].Add(tflNode);
                        } else {
                            _problems.Add(string.Format("Invalid element {0} in {1}.", add.Name, subNode.Name));
                        }
                    }
                } else {
                    _problems.Add(string.Format("Invalid element {0} in {1}.", subNode.Name, n.Name));
                }
            }

            CheckRequiredClasses(n, parentName);
        }

        private void CheckRequiredClasses(NanoXmlNode node, string parentName) {
            for (var i = 0; i < _requiredClasses.Count; i++) {
                if (!_classes.ContainsKey(_requiredClasses[i])) {
                    if (parentName == null) {
                        _problems.Add(string.Format("The '{0}' element is missing a{2} '{1}' element.", node.Name, _requiredClasses[i], _requiredClasses[i][0].IsVowel() ? "n" : string.Empty));
                    } else {
                        _problems.Add(string.Format("A{3} '{0}' '{1}' element is missing a{4} '{2}' element.", parentName, node.Name, _requiredClasses[i], parentName[0].IsVowel() ? "n" : string.Empty, _requiredClasses[i][0].IsVowel() ? "n" : string.Empty));
                    }
                } else if (_classes[_requiredClasses[i]].Count == 0) {
                    _problems.Add(string.Format("A{1} '{0}' element is missing an 'add' element.", _requiredClasses[i], _requiredClasses[i][0].IsVowel() ? "n" : string.Empty));
                }
            }
        }

        private void LoadProperties(NanoXmlNode node, string parentName) {
            for (var i = 0; i < node.Attributes.Count; i++) {
                var attribute = node.Attributes[i];
                if (_properties.ContainsKey(attribute.Name)) {
                    var data = _properties[attribute.Name];

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
                    _problems.Add(string.Format("A{3} '{0}' '{1}' element contains an invalid '{2}' attribute.", parentName, node.Name, attribute.Name, parentName[0].IsVowel() ? "n" : string.Empty));
                }
            }

            CheckRequiredProperties(node, parentName);
        }

        private void CheckRequiredProperties(NanoXmlNode node, string parentName) {
            for (var i = 0; i < _requiredProperties.Count; i++) {
                if (!_properties[_requiredProperties[i]].Set) {
                    _problems.Add(string.Format("A{3} '{0}' '{1}' element is missing a '{2}' attribute.", parentName, node.Name, _requiredProperties[i], parentName[0].IsVowel() ? "n" : string.Empty));
                }
            }
        }

        public Dictionary<string, TflProperty> Properties {
            get { return _properties; }
        }

        public Dictionary<string, List<TflNode>> Classes {
            get { return _classes; }
        }

        public List<string> AllProblems() {
            var allProblems = new List<string>();
            for (var i = 0; i < _problems.Count; i++) {
                allProblems.Add(_problems[i]);
            }
            foreach (var pair in Classes) {
                for (var i = 0; i < pair.Value.Count; i++) {
                    var @class = pair.Value[i];
                    allProblems.AddRange(@class.AllProblems());
                }
            }
            return allProblems;
        }

    }
}