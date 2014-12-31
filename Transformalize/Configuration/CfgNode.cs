using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using Transformalize.Extensions;
using Transformalize.Libs.NanoXml;

namespace Transformalize.Configuration {
    public abstract class CfgNode {

        private static readonly Dictionary<Type, List<Tuple<string, PropertyInfo>>> PropertiesCache = new Dictionary<Type, List<Tuple<string, PropertyInfo>>>();

        private readonly List<string> _propertyKeys = new List<string>();
        private readonly List<string> _classKeys = new List<string>();
        private readonly Dictionary<string, TflProperty> _properties = new Dictionary<string, TflProperty>(StringComparer.Ordinal);
        private readonly Dictionary<string, Dictionary<string, TflProperty>> _classProperties = new Dictionary<string, Dictionary<string, TflProperty>>(StringComparer.Ordinal);
        private readonly List<string> _requiredProperties = new List<string>();
        private readonly List<string> _uniqueProperties = new List<string>();
        private readonly Dictionary<string, List<CfgNode>> _classes = new Dictionary<string, List<CfgNode>>();
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
        public CfgNode this[string element, int i] {
            get { return _classes[element][i]; }
        }

        // Get an attribute by n
        public TflProperty this[string name] {
            get { return _properties[name]; }
        }

        private readonly Dictionary<string, Func<CfgNode>> _elementLoaders = new Dictionary<string, Func<CfgNode>>();
        protected List<string> Problems { get { return _problems; } }

        protected void Class<T>(string element, bool required = false) {
            if (!_elementLoaders.ContainsKey(element)) {
                _classKeys.Add(element);
            }
            _elementLoaders[element] = () => (CfgNode)Activator.CreateInstance(typeof(T));
            if (required) {
                _requiredClasses.Add(element);
            }
        }

        protected void Class<T1, T2>(string element, bool required = false, string classProperty = null, T2 value = default(T2)) {
            this.Class<T1>(element, required);
            if (!string.IsNullOrEmpty(classProperty)) {
                ClassProperty(element, classProperty, value);
            }
        }

        protected void Property<T>(string n, T v, bool r, bool u) {
            if (!_properties.ContainsKey(n)) {
                _propertyKeys.Add(n);
            }
            _properties[n] = new TflProperty(n, v, r, u);
            if (r) {
                _requiredProperties.Add(n);
            }
            if (u) {
                _uniqueProperties.Add(n);
            }
        }

        protected void Property<T>(string n, T v) {
            Property(n, v, false, false);
        }

        protected void Property<T>(string n, T v, bool r) {
            Property(n, v, r, false);
        }

        protected void Property(string n) {
            Property(n, string.Empty, false, false);
        }

        protected void Property(TflProperty property) {
            Property(property.Name, property.Value, property.Required, property.Unique);
        }

        protected void ClassProperty<T>(string className, string propertyName, T value) {
            if (_classProperties.ContainsKey(className)) {
                _classProperties[className][propertyName] = new TflProperty(propertyName, value);
            } else {
                _classProperties[className] = new Dictionary<string, TflProperty>(StringComparer.Ordinal) { { propertyName, new TflProperty(propertyName, value) } };
            }
        }

        // Convenience method for keys
        protected void Key(string name, bool unique = true) {
            Property(name, string.Empty, true, unique);
        }

        public CfgNode Load(NanoXmlNode node, string parentName = null) {
            LoadProperties(node, parentName);
            LoadClasses(node, parentName);
            return this;
        }

        private void LoadClasses(NanoXmlNode node, string parentName) {
            for (var i = 0; i < node.SubNodes.Count; i++) {
                var subNode = node.SubNodes[i];
                if (_elementLoaders.ContainsKey(subNode.Name)) {
                    _classes[subNode.Name] = new List<CfgNode>();
                    for (var j = 0; j < subNode.SubNodes.Count; j++) {
                        var add = subNode.SubNodes[j];
                        if (add.Name.Equals("add")) {
                            var tflNode = _elementLoaders[subNode.Name]().Load(add, subNode.Name);
                            // check for duplicates of u attributes
                            for (var k = 0; k < _classes[subNode.Name].Count; k++) {
                                foreach (var pair in _classes[subNode.Name][k].Properties) {
                                    if (!pair.Value.Unique || !tflNode.Properties[pair.Key].Value.Equals(pair.Value.Value))
                                        continue;

                                    if (tflNode.Properties[pair.Key].Set && pair.Value.Set) {
                                        _problems.Add(string.Format("Duplicate {0} v {1} in {2}.", pair.Key, pair.Value.Value, subNode.Name));
                                    } else {
                                        if (!pair.Value.Value.Equals(string.Empty)) {
                                            _problems.Add(string.Format("Possible duplicate {0} v {1} in {2}.", pair.Key, pair.Value.Value, subNode.Name));
                                        }
                                    }
                                }
                            }

                            // handle class properties
                            if (_classProperties.ContainsKey(subNode.Name)) {
                                foreach (var attribute in subNode.Attributes) {
                                    if (!_classProperties[subNode.Name].ContainsKey(attribute.Name))
                                        continue;
                                    var classProperty = _classProperties[subNode.Name][attribute.Name];
                                    classProperty.Value = attribute.Value ?? classProperty.Value;
                                    tflNode.Property(classProperty);
                                }
                            }

                            // add instance property
                            _classes[subNode.Name].Add(tflNode);
                        } else {
                            _problems.Add(string.Format("Invalid element {0} in {1}.  Only 'add' elements are allowed here.", add.Name, subNode.Name));
                        }
                    }
                } else {
                    _problems.Add(string.Format("Invalid element {0} in {1}.", subNode.Name, node.Name));
                }
            }

            CheckRequiredClasses(node, parentName);
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
                    _problems.Add(
                        string.Format(
                            "A{3} '{0}' '{1}' element contains an invalid '{2}' attribute.  Valid attributes are: {4}.",
                            parentName,
                            node.Name,
                            attribute.Name, parentName[0].IsVowel() ? "n" : string.Empty,
                            string.Join(", ", _properties.Select(kv => kv.Key))
                        )
                    );
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

        public Dictionary<string, List<CfgNode>> Classes {
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

        public void Populate() {

            for (int i = 0; i < _propertyKeys.Count; i++)
            {
                var property = _propertyKeys[i];

            }

            var properties = GetProperties(this.GetType());

            for (var i = 0; i < properties.Count; i++) {
                var property = properties[i];
                if (_properties.ContainsKey(property.Item1)) {
                    property.Item2.SetValue(this, _properties[property.Item1].Value, null);
                } else {
                    if (_classes.ContainsKey(property.Item1)) {
                        var list = (IList)Activator.CreateInstance(property.Item2.PropertyType);
                        foreach (var item in _classes[property.Item1]) {
                            item.Populate();
                            list.Add(item);
                        }
                        property.Item2.SetValue(this, list, null);
                    } else {
                        _problems.Add(string.Format("Property '{0}' not mapped correctly. A property named '{0}' should correspond to an xml node named '{1}'. ", property.Item2.Name, ToSlug(property.Item2.Name)));
                    }
                }
            }
        }

        private static string ToSlug(string properCase) {
            var sb = new StringBuilder();
            for (var i = 0; i < properCase.Length; i++) {
                var c = properCase[i];
                if (char.IsUpper(c)) {
                    if (i > 0) {
                        sb.Append('-');
                    }
                    sb.Append(char.ToLower(c));
                } else {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        //private static Dictionary<string, Tuple<string, PropertyInfo>> GetProps(Type type)
        //{
            
        //} 

        private static List<Tuple<string, PropertyInfo>> GetProperties(Type type) {
            List<Tuple<string, PropertyInfo>> properties;
            if (PropertiesCache.TryGetValue(type, out properties))
                return properties;

            properties = new List<Tuple<string, PropertyInfo>>();
            var propertyInfos = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            for (var i = 0; i < propertyInfos.Length; i++) {
                var propertyInfo = propertyInfos[i];
                properties.Add(new Tuple<string, PropertyInfo>(ToSlug(propertyInfo.Name), propertyInfo));
            }
            PropertiesCache[type] = properties;
            return properties;
        }

    }
}