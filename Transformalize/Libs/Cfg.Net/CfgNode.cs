using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Transformalize.Libs.Cfg.Net {

    public abstract class CfgNode {

        private const char HIGH_SURROGATE = '\uD800';
        private const char LOW_SURROGATE = '\uDC00';
        private const int UNICODE_00_END = 0x00FFFF;
        private const int UNICODE_01_START = 0x10000;

        private static readonly Dictionary<Type, Dictionary<string, PropertyInfo>> PropertiesCache = new Dictionary<Type, Dictionary<string, PropertyInfo>>();
        private static Dictionary<string, char> _entities;

        private readonly List<string> _propertyKeys = new List<string>();
        private readonly List<string> _classKeys = new List<string>();
        private readonly Dictionary<string, CfgProperty> _properties = new Dictionary<string, CfgProperty>(StringComparer.Ordinal);
        private readonly Dictionary<string, Dictionary<string, CfgProperty>> _classProperties = new Dictionary<string, Dictionary<string, CfgProperty>>(StringComparer.Ordinal);
        private readonly List<string> _requiredProperties = new List<string>();
        private readonly List<string> _uniqueProperties = new List<string>();
        private readonly Dictionary<string, List<CfgNode>> _classes = new Dictionary<string, List<CfgNode>>();
        private readonly List<string> _requiredClasses = new List<string>();
        private readonly List<string> _problems = new List<string>();
        private readonly Dictionary<string, Func<CfgNode>> _elementLoaders = new Dictionary<string, Func<CfgNode>>();

        protected List<string> Problems { get { return _problems; } }
        protected static bool TurnOffProperties { get; set; }

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

        // Get an attribute by name
        public CfgProperty this[string name] {
            get { return _properties[name]; }
        }

        public void Load(NanoXmlNode node) {
            LoadProperties(node, null);
            LoadCollections(node, null);
            if (!TurnOffProperties) {
                PopulateProperties();
            }
        }

        protected void Collection<T>(string name, bool required = false) {
            _elementLoaders[name] = () => (CfgNode)Activator.CreateInstance(typeof(T));
            if (required) {
                _requiredClasses.Add(name);
            }
        }

        protected void Collection<T>(Type type, string element, bool required = false, string sharedProperty = null, T sharedValue = default(T)) {
            this.Collection(type, element, required);
            if (!string.IsNullOrEmpty(sharedProperty)) {
                SharedProperty(element, sharedProperty, sharedValue);
            }
        }

        protected void Collection(Type type, string name, bool required = false) {
            _elementLoaders[name] = () => (CfgNode)Activator.CreateInstance(type);
            if (required) {
                _requiredClasses.Add(name);
            }
        }

        protected void Collection<T1, T2>(string element, bool required = false, string sharedProperty = null, T2 sharedValue = default(T2)) {
            this.Collection<T1>(element, required);
            if (!string.IsNullOrEmpty(sharedProperty)) {
                SharedProperty(element, sharedProperty, sharedValue);
            }
        }

        protected void Property<T>(string name, T value, bool required = false, bool unique = false, bool decode = false) {
            if (!_properties.ContainsKey(name)) {
                _propertyKeys.Add(name);
            }
            _properties[name] = new CfgProperty(name, value, required, unique, decode);
            if (required) {
                _requiredProperties.Add(name);
            }
            if (unique) {
                _uniqueProperties.Add(name);
            }
        }

        protected void Property(string name) {
            Property(name, string.Empty, false, false, false);
        }

        protected void Property(CfgProperty property) {
            Property(property.Name, property.Value, property.Required, property.Unique, property.Decode);
        }

        protected void SharedProperty<T>(string className, string propertyName, T value) {
            if (_classProperties.ContainsKey(className)) {
                _classProperties[className][propertyName] = new CfgProperty(propertyName, value);
            } else {
                _classProperties[className] = new Dictionary<string, CfgProperty>(StringComparer.Ordinal) { { propertyName, new CfgProperty(propertyName, value) } };
            }
        }

        // Convenience method for keys
        protected void Key(string name, bool unique = true) {
            Property(name, string.Empty, true, unique);
        }

        protected CfgNode Load(NanoXmlNode node, string parentName) {
            LoadProperties(node, parentName);
            LoadCollections(node, parentName);
            return this;
        }

        private void LoadCollections(NanoXmlNode node, string parentName) {

            if (_elementLoaders.Count == 0 && !TurnOffProperties) {
                var propertyInfos = GetProperties(GetType());
                foreach (var pair in propertyInfos) {
                    if (pair.Value.MemberType != MemberTypes.Property)
                        continue;
                    var attribute = (CfgAttribute)Attribute.GetCustomAttribute(pair.Value, typeof(CfgAttribute));
                    if (attribute == null)
                        continue;
                    if (!pair.Value.PropertyType.IsGenericType)
                        continue;
                    var listType = pair.Value.PropertyType.GetGenericArguments()[0];
                    if (attribute.sharedProperty == null) {
                        Collection(listType, ToXmlNameStyle(pair.Value.Name), attribute.required);
                    } else {
                        Collection(listType, ToXmlNameStyle(pair.Value.Name), attribute.required, attribute.sharedProperty, attribute.sharedValue);
                    }
                }
            }

            for (var i = 0; i < node.SubNodes.Count; i++) {
                var subNode = node.SubNodes[i];
                if (_elementLoaders.ContainsKey(subNode.Name)) {

                    if (!_classes.ContainsKey(subNode.Name)) {
                        _classKeys.Add(subNode.Name);
                    }
                    _classes[subNode.Name] = new List<CfgNode>();

                    for (var j = 0; j < subNode.SubNodes.Count; j++) {
                        var add = subNode.SubNodes[j];
                        if (add.Name.Equals("add")) {
                            var tflNode = _elementLoaders[subNode.Name]().Load(add, subNode.Name);
                            // check for duplicates of unique attributes
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
                        _problems.Add(string.Format("The '{0}' element is missing a{2} '{1}' element.", node.Name, _requiredClasses[i], _requiredClasses[i][0].IsVowel() ? "name" : string.Empty));
                    } else {
                        _problems.Add(string.Format("A{3} '{0}' '{1}' element is missing a{4} '{2}' element.", parentName, node.Name, _requiredClasses[i], parentName[0].IsVowel() ? "name" : string.Empty, _requiredClasses[i][0].IsVowel() ? "name" : string.Empty));
                    }
                } else if (_classes[_requiredClasses[i]].Count == 0) {
                    _problems.Add(string.Format("A{1} '{0}' element is missing an 'add' element.", _requiredClasses[i], _requiredClasses[i][0].IsVowel() ? "name" : string.Empty));
                }
            }
        }

        private void LoadProperties(NanoXmlNode node, string parentName) {

            if (_properties.Count == 0 && !TurnOffProperties) {
                var propertyInfos = GetProperties(this.GetType());
                foreach (var pair in propertyInfos) {
                    if (pair.Value.MemberType != MemberTypes.Property)
                        continue;
                    var attribute = (CfgAttribute)Attribute.GetCustomAttribute(pair.Value, typeof(CfgAttribute));
                    if (attribute == null)
                        continue;
                    if (pair.Value.PropertyType.IsGenericType)
                        continue;

                    Property(ToXmlNameStyle(pair.Value.Name), attribute.value, attribute.required, attribute.unique, attribute.decode);
                }
            }

            for (var i = 0; i < node.Attributes.Count; i++) {
                var attribute = node.Attributes[i];
                if (_properties.ContainsKey(attribute.Name)) {
                    if (attribute.Value == null)
                        continue;
                    var data = _properties[attribute.Name];

                    if (data.Value is string) {
                        data.Value = data.Decode && attribute.Value.IndexOf('&') >= 0 ? Decode(attribute.Value) : attribute.Value;
                        data.Set = true;
                    } else {
                        try {
                            data.Value = Converter[data.Value.GetType()](data.Decode && attribute.Value.IndexOf('&') >= 0 ? Decode(attribute.Value) : attribute.Value);
                            data.Set = true;
                        } catch (Exception ex) {
                            _problems.Add(string.Format("Could not set '{0}' to '{1}' inside '{2}' '{3}'. {4}", data.Name, attribute.Value, parentName, node.Name, ex.Message));
                        }
                    }
                } else {
                    _problems.Add(
                        string.Format(
                            "A{3} '{0}' '{1}' element contains an invalid '{2}' attribute.  Valid attributes are: {4}.",
                            parentName,
                            node.Name,
                            attribute.Name, parentName[0].IsVowel() ? "name" : string.Empty,
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
                    _problems.Add(string.Format("A{3} '{0}' '{1}' element is missing a '{2}' attribute.", parentName, node.Name, _requiredProperties[i], parentName[0].IsVowel() ? "name" : string.Empty));
                }
            }
        }

        protected Dictionary<string, CfgProperty> Properties {
            get { return _properties; }
        }

        protected Dictionary<string, List<CfgNode>> Classes {
            get { return _classes; }
        }

        private static Dictionary<string, char> Entities {
            get {
                return _entities ?? (_entities = new Dictionary<string, char>(StringComparer.Ordinal)
                {
                    {"Aacute", "\x00c1"[0]},
                    {"aacute", "\x00e1"[0]},
                    {"Acirc", "\x00c2"[0]},
                    {"acirc", "\x00e2"[0]},
                    {"acute", "\x00b4"[0]},
                    {"AElig", "\x00c6"[0]},
                    {"aelig", "\x00e6"[0]},
                    {"Agrave", "\x00c0"[0]},
                    {"agrave", "\x00e0"[0]},
                    {"alefsym", "\x2135"[0]},
                    {"Alpha", "\x0391"[0]},
                    {"alpha", "\x03b1"[0]},
                    {"amp", "\x0026"[0]},
                    {"and", "\x2227"[0]},
                    {"ang", "\x2220"[0]},
                    {"apos", "\x0027"[0]},
                    {"Aring", "\x00c5"[0]},
                    {"aring", "\x00e5"[0]},
                    {"asymp", "\x2248"[0]},
                    {"Atilde", "\x00c3"[0]},
                    {"atilde", "\x00e3"[0]},
                    {"Auml", "\x00c4"[0]},
                    {"auml", "\x00e4"[0]},
                    {"bdquo", "\x201e"[0]},
                    {"Beta", "\x0392"[0]},
                    {"beta", "\x03b2"[0]},
                    {"brvbar", "\x00a6"[0]},
                    {"bull", "\x2022"[0]},
                    {"cap", "\x2229"[0]},
                    {"Ccedil", "\x00c7"[0]},
                    {"ccedil", "\x00e7"[0]},
                    {"cedil", "\x00b8"[0]},
                    {"cent", "\x00a2"[0]},
                    {"Chi", "\x03a7"[0]},
                    {"chi", "\x03c7"[0]},
                    {"circ", "\x02c6"[0]},
                    {"clubs", "\x2663"[0]},
                    {"cong", "\x2245"[0]},
                    {"copy", "\x00a9"[0]},
                    {"crarr", "\x21b5"[0]},
                    {"cup", "\x222a"[0]},
                    {"curren", "\x00a4"[0]},
                    {"dagger", "\x2020"[0]},
                    {"Dagger", "\x2021"[0]},
                    {"darr", "\x2193"[0]},
                    {"dArr", "\x21d3"[0]},
                    {"deg", "\x00b0"[0]},
                    {"Delta", "\x0394"[0]},
                    {"delta", "\x03b4"[0]},
                    {"diams", "\x2666"[0]},
                    {"divide", "\x00f7"[0]},
                    {"Eacute", "\x00c9"[0]},
                    {"eacute", "\x00e9"[0]},
                    {"Ecirc", "\x00ca"[0]},
                    {"ecirc", "\x00ea"[0]},
                    {"Egrave", "\x00c8"[0]},
                    {"egrave", "\x00e8"[0]},
                    {"empty", "\x2205"[0]},
                    {"emsp", "\x2003"[0]},
                    {"ensp", "\x2002"[0]},
                    {"Epsilon", "\x0395"[0]},
                    {"epsilon", "\x03b5"[0]},
                    {"equiv", "\x2261"[0]},
                    {"Eta", "\x0397"[0]},
                    {"eta", "\x03b7"[0]},
                    {"ETH", "\x00d0"[0]},
                    {"eth", "\x00f0"[0]},
                    {"Euml", "\x00cb"[0]},
                    {"euml", "\x00eb"[0]},
                    {"euro", "\x20ac"[0]},
                    {"exist", "\x2203"[0]},
                    {"fnof", "\x0192"[0]},
                    {"forall", "\x2200"[0]},
                    {"frac12", "\x00bd"[0]},
                    {"frac14", "\x00bc"[0]},
                    {"frac34", "\x00be"[0]},
                    {"frasl", "\x2044"[0]},
                    {"Gamma", "\x0393"[0]},
                    {"gamma", "\x03b3"[0]},
                    {"ge", "\x2265"[0]},
                    {"gt", "\x003e"[0]},
                    {"harr", "\x2194"[0]},
                    {"hArr", "\x21d4"[0]},
                    {"hearts", "\x2665"[0]},
                    {"hellip", "\x2026"[0]},
                    {"Iacute", "\x00cd"[0]},
                    {"iacute", "\x00ed"[0]},
                    {"Icirc", "\x00ce"[0]},
                    {"icirc", "\x00ee"[0]},
                    {"iexcl", "\x00a1"[0]},
                    {"Igrave", "\x00cc"[0]},
                    {"igrave", "\x00ec"[0]},
                    {"image", "\x2111"[0]},
                    {"infin", "\x221e"[0]},
                    {"int", "\x222b"[0]},
                    {"Iota", "\x0399"[0]},
                    {"iota", "\x03b9"[0]},
                    {"iquest", "\x00bf"[0]},
                    {"isin", "\x2208"[0]},
                    {"Iuml", "\x00cf"[0]},
                    {"iuml", "\x00ef"[0]},
                    {"Kappa", "\x039a"[0]},
                    {"kappa", "\x03ba"[0]},
                    {"Lambda", "\x039b"[0]},
                    {"lambda", "\x03bb"[0]},
                    {"lang", "\x2329"[0]},
                    {"laquo", "\x00ab"[0]},
                    {"larr", "\x2190"[0]},
                    {"lArr", "\x21d0"[0]},
                    {"lceil", "\x2308"[0]},
                    {"ldquo", "\x201c"[0]},
                    {"le", "\x2264"[0]},
                    {"lfloor", "\x230a"[0]},
                    {"lowast", "\x2217"[0]},
                    {"loz", "\x25ca"[0]},
                    {"lrm", "\x200e"[0]},
                    {"lsaquo", "\x2039"[0]},
                    {"lsquo", "\x2018"[0]},
                    {"lt", "\x003c"[0]},
                    {"macr", "\x00af"[0]},
                    {"mdash", "\x2014"[0]},
                    {"micro", "\x00b5"[0]},
                    {"middot", "\x00b7"[0]},
                    {"minus", "\x2212"[0]},
                    {"Mu", "\x039c"[0]},
                    {"mu", "\x03bc"[0]},
                    {"nabla", "\x2207"[0]},
                    {"nbsp", "\x00a0"[0]},
                    {"ndash", "\x2013"[0]},
                    {"ne", "\x2260"[0]},
                    {"ni", "\x220b"[0]},
                    {"not", "\x00ac"[0]},
                    {"notin", "\x2209"[0]},
                    {"nsub", "\x2284"[0]},
                    {"Ntilde", "\x00d1"[0]},
                    {"ntilde", "\x00f1"[0]},
                    {"Nu", "\x039d"[0]},
                    {"nu", "\x03bd"[0]},
                    {"Oacute", "\x00d3"[0]},
                    {"oacute", "\x00f3"[0]},
                    {"Ocirc", "\x00d4"[0]},
                    {"ocirc", "\x00f4"[0]},
                    {"OElig", "\x0152"[0]},
                    {"oelig", "\x0153"[0]},
                    {"Ograve", "\x00d2"[0]},
                    {"ograve", "\x00f2"[0]},
                    {"oline", "\x203e"[0]},
                    {"Omega", "\x03a9"[0]},
                    {"omega", "\x03c9"[0]},
                    {"Omicron", "\x039f"[0]},
                    {"omicron", "\x03bf"[0]},
                    {"oplus", "\x2295"[0]},
                    {"or", "\x2228"[0]},
                    {"ordf", "\x00aa"[0]},
                    {"ordm", "\x00ba"[0]},
                    {"Oslash", "\x00d8"[0]},
                    {"oslash", "\x00f8"[0]},
                    {"Otilde", "\x00d5"[0]},
                    {"otilde", "\x00f5"[0]},
                    {"otimes", "\x2297"[0]},
                    {"Ouml", "\x00d6"[0]},
                    {"ouml", "\x00f6"[0]},
                    {"para", "\x00b6"[0]},
                    {"part", "\x2202"[0]},
                    {"permil", "\x2030"[0]},
                    {"perp", "\x22a5"[0]},
                    {"Phi", "\x03a6"[0]},
                    {"phi", "\x03c6"[0]},
                    {"Pi", "\x03a0"[0]},
                    {"pi", "\x03c0"[0]},
                    {"piv", "\x03d6"[0]},
                    {"plusmn", "\x00b1"[0]},
                    {"pound", "\x00a3"[0]},
                    {"prime", "\x2032"[0]},
                    {"Prime", "\x2033"[0]},
                    {"prod", "\x220f"[0]},
                    {"prop", "\x221d"[0]},
                    {"Psi", "\x03a8"[0]},
                    {"psi", "\x03c8"[0]},
                    {"quot", "\x0022"[0]},
                    {"radic", "\x221a"[0]},
                    {"rang", "\x232a"[0]},
                    {"raquo", "\x00bb"[0]},
                    {"rarr", "\x2192"[0]},
                    {"rArr", "\x21d2"[0]},
                    {"rceil", "\x2309"[0]},
                    {"rdquo", "\x201d"[0]},
                    {"real", "\x211c"[0]},
                    {"reg", "\x00ae"[0]},
                    {"rfloor", "\x230b"[0]},
                    {"Rho", "\x03a1"[0]},
                    {"rho", "\x03c1"[0]},
                    {"rlm", "\x200f"[0]},
                    {"rsaquo", "\x203a"[0]},
                    {"rsquo", "\x2019"[0]},
                    {"sbquo", "\x201a"[0]},
                    {"Scaron", "\x0160"[0]},
                    {"scaron", "\x0161"[0]},
                    {"sdot", "\x22c5"[0]},
                    {"sect", "\x00a7"[0]},
                    {"shy", "\x00ad"[0]},
                    {"Sigma", "\x03a3"[0]},
                    {"sigma", "\x03c3"[0]},
                    {"sigmaf", "\x03c2"[0]},
                    {"sim", "\x223c"[0]},
                    {"spades", "\x2660"[0]},
                    {"sub", "\x2282"[0]},
                    {"sube", "\x2286"[0]},
                    {"sum", "\x2211"[0]},
                    {"sup", "\x2283"[0]},
                    {"sup1", "\x00b9"[0]},
                    {"sup2", "\x00b2"[0]},
                    {"sup3", "\x00b3"[0]},
                    {"supe", "\x2287"[0]},
                    {"szlig", "\x00df"[0]},
                    {"Tau", "\x03a4"[0]},
                    {"tau", "\x03c4"[0]},
                    {"there4", "\x2234"[0]},
                    {"Theta", "\x0398"[0]},
                    {"theta", "\x03b8"[0]},
                    {"thetasym", "\x03d1"[0]},
                    {"thinsp", "\x2009"[0]},
                    {"THORN", "\x00de"[0]},
                    {"thorn", "\x00fe"[0]},
                    {"tilde", "\x02dc"[0]},
                    {"times", "\x00d7"[0]},
                    {"trade", "\x2122"[0]},
                    {"Uacute", "\x00da"[0]},
                    {"uacute", "\x00fa"[0]},
                    {"uarr", "\x2191"[0]},
                    {"uArr", "\x21d1"[0]},
                    {"Ucirc", "\x00db"[0]},
                    {"ucirc", "\x00fb"[0]},
                    {"Ugrave", "\x00d9"[0]},
                    {"ugrave", "\x00f9"[0]},
                    {"uml", "\x00a8"[0]},
                    {"upsih", "\x03d2"[0]},
                    {"Upsilon", "\x03a5"[0]},
                    {"upsilon", "\x03c5"[0]},
                    {"Uuml", "\x00dc"[0]},
                    {"uuml", "\x00fc"[0]},
                    {"weierp", "\x2118"[0]},
                    {"Xi", "\x039e"[0]},
                    {"xi", "\x03be"[0]},
                    {"Yacute", "\x00dd"[0]},
                    {"yacute", "\x00fd"[0]},
                    {"yen", "\x00a5"[0]},
                    {"yuml", "\x00ff"[0]},
                    {"Yuml", "\x0178"[0]},
                    {"Zeta", "\x0396"[0]},
                    {"zeta", "\x03b6"[0]},
                    {"zwj", "\x200d"[0]},
                    {"zwnj", "\x200c"[0]}
                });
            }
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

        protected void PopulateProperties() {

            var properties = GetProperties(this.GetType());

            for (var i = 0; i < _propertyKeys.Count; i++) {
                var key = _propertyKeys[i];
                if (properties.ContainsKey(key)) {
                    try {
                        properties[key].SetValue(this, _properties[key].Value, null);
                    } catch (Exception ex) {
                        _problems.Add(string.Format("Could not set property {0} to value {1} from attribute {2}. {3}", properties[key].Name, _properties[key].Value, _properties[key].Name, ex.Message));
                    }
                }
            }

            for (var i = 0; i < _classKeys.Count; i++) {
                var key = _classKeys[i];
                if (!properties.ContainsKey(key))
                    continue;
                var list = (IList)Activator.CreateInstance(properties[key].PropertyType);
                for (var j = 0; j < _classes[key].Count; j++) {
                    _classes[key][j].PopulateProperties();
                    list.Add(_classes[key][j]);
                }
                properties[key].SetValue(this, list, null);
            }
        }

        private static string ToXmlNameStyle(string input) {
            var sb = new StringBuilder();
            for (var i = 0; i < input.Length; i++) {
                var c = input[i];
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

        private static Dictionary<string, PropertyInfo> GetProperties(Type type) {
            Dictionary<string, PropertyInfo> properties;
            if (PropertiesCache.TryGetValue(type, out properties))
                return properties;

            properties = new Dictionary<string, PropertyInfo>();
            var propertyInfos = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            for (var i = 0; i < propertyInfos.Length; i++) {
                var propertyInfo = propertyInfos[i];
                properties[ToXmlNameStyle(propertyInfo.Name)] = propertyInfo;
            }
            PropertiesCache[type] = properties;
            return properties;
        }

        public static string Decode(string value) {

            var output = new StringWriter();
            var htmlEntityEndingChars = new[] { ';', '&' };
            var l = value.Length;
            for (var i = 0; i < l; i++) {
                var ch = value[i];

                if (ch == '&') {
                    // Found &. Look for the next ; or &. If & occurs before ;, then this is not entity, and next & may start another entity
                    var index = value.IndexOfAny(htmlEntityEndingChars, i + 1);
                    if (index > 0 && value[index] == ';') {
                        var entity = value.Substring(i + 1, index - i - 1);

                        if (entity.Length > 1 && entity[0] == '#') {

                            bool parsedSuccessfully;
                            uint parsedValue;
                            if (entity[1] == 'x' || entity[1] == 'X') {
                                parsedSuccessfully = UInt32.TryParse(entity.Substring(2), NumberStyles.AllowHexSpecifier, NumberFormatInfo.InvariantInfo, out parsedValue);
                            } else {
                                parsedSuccessfully = UInt32.TryParse(entity.Substring(1), NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out parsedValue);
                            }

                            if (parsedSuccessfully) {
                                parsedSuccessfully = (0 < parsedValue && parsedValue <= UNICODE_00_END);
                            }

                            if (parsedSuccessfully) {
                                if (parsedValue <= UNICODE_00_END) {
                                    // single character
                                    output.Write((char)parsedValue);
                                } else {
                                    // multi-character
                                    var utf32 = (int)(parsedValue - UNICODE_01_START);
                                    var leadingSurrogate = (char)((utf32 / 0x400) + HIGH_SURROGATE);
                                    var trailingSurrogate = (char)((utf32 % 0x400) + LOW_SURROGATE);

                                    output.Write(leadingSurrogate);
                                    output.Write(trailingSurrogate);
                                }

                                i = index; // already looked at everything until semicolon
                                continue;
                            }
                        } else {
                            i = index; // already looked at everything until semicolon

                            char entityChar;
                            Entities.TryGetValue(entity, out entityChar);

                            if (entityChar != (char)0) {
                                ch = entityChar;
                            } else {
                                output.Write('&');
                                output.Write(entity);
                                output.Write(';');
                                continue;
                            }
                        }
                    }
                }
                output.Write(ch);
            }
            return output.ToString();
        }

        public int Count(string n) {
            if (_classes.ContainsKey(n)) {
                return _classes[n].Count;
            }
            return _properties.ContainsKey(n) ? 1 : 0;
        }
    }

}