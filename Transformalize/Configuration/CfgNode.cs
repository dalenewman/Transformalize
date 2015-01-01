using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Transformalize.Extensions;
using Transformalize.Libs.NanoXml;

namespace Transformalize.Configuration {

    [AttributeUsage(AttributeTargets.Property)]
    public class CfgAttribute : Attribute {
        public object v { get; set; }
        public bool r { get; set; }
        public bool u { get; set; }
        public bool d { get; set; }
    }

    public abstract class CfgNode {

        private static readonly Dictionary<Type, Dictionary<string, PropertyInfo>> PropertiesCache = new Dictionary<Type, Dictionary<string, PropertyInfo>>();

        private readonly List<string> _propertyKeys = new List<string>();
        private readonly List<string> _classKeys = new List<string>();
        private readonly Dictionary<string, CfgProperty> _properties = new Dictionary<string, CfgProperty>(StringComparer.Ordinal);
        private readonly Dictionary<string, Dictionary<string, CfgProperty>> _classProperties = new Dictionary<string, Dictionary<string, CfgProperty>>(StringComparer.Ordinal);
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
        public CfgProperty this[string name] {
            get { return _properties[name]; }
        }

        private readonly Dictionary<string, Func<CfgNode>> _elementLoaders = new Dictionary<string, Func<CfgNode>>();
        protected List<string> Problems { get { return _problems; } }

        protected void Class<T>(string n, bool r = false) {
            _elementLoaders[n] = () => (CfgNode)Activator.CreateInstance(typeof(T));
            if (r) {
                _requiredClasses.Add(n);
            }
        }

        protected void Class<T1, T2>(string element, bool required = false, string classProperty = null, T2 value = default(T2)) {
            this.Class<T1>(element, required);
            if (!string.IsNullOrEmpty(classProperty)) {
                ClassProperty(element, classProperty, value);
            }
        }

        protected void Property<T>(string n, T v, bool r = false, bool u = false, bool d = false) {
            if (!_properties.ContainsKey(n)) {
                _propertyKeys.Add(n);
            }
            _properties[n] = new CfgProperty(n, v, r, u, d);
            if (r) {
                _requiredProperties.Add(n);
            }
            if (u) {
                _uniqueProperties.Add(n);
            }
        }

        protected void Property(string n) {
            Property(n, string.Empty, false, false, false);
        }

        protected void Property(CfgProperty property) {
            Property(property.Name, property.Value, property.Required, property.Unique, property.Decode);
        }

        protected void ClassProperty<T>(string className, string propertyName, T value) {
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

        public CfgNode Load(NanoXmlNode node, string parentName = null) {
            LoadProperties(node, parentName);
            LoadClasses(node, parentName);
            return this;
        }

        private void LoadClasses(NanoXmlNode node, string parentName) {
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
                        if (attribute.Value == null)
                            continue;

                        try {
                            data.Value = Converter[data.Value.GetType()](data.Decode ? HtmlDecode(attribute.Value) : attribute.Value);
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

        protected Dictionary<string, CfgProperty> Properties {
            get { return _properties; }
        }

        protected Dictionary<string, List<CfgNode>> Classes {
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
                    _classes[key][j].Populate();
                    list.Add(_classes[key][j]);
                }
                properties[key].SetValue(this, list, null);
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

        private static Dictionary<string, PropertyInfo> GetProperties(Type type) {
            Dictionary<string, PropertyInfo> properties;
            if (PropertiesCache.TryGetValue(type, out properties))
                return properties;

            properties = new Dictionary<string, PropertyInfo>();
            var propertyInfos = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            for (var i = 0; i < propertyInfos.Length; i++) {
                var propertyInfo = propertyInfos[i];
                properties[ToSlug(propertyInfo.Name)] = propertyInfo;
            }
            PropertiesCache[type] = properties;
            return properties;
        }

        private const char HIGH_SURROGATE_START = '\uD800';
        private const char LOW_SURROGATE_START = '\uDC00';
        private const int UNICODE_PLANE00_END = 0x00FFFF;
        private const int UNICODE_PLANE01_START = 0x10000;

        private static void ConvertSmpToUtf16(uint smpChar, out char leadingSurrogate, out char trailingSurrogate) {
            int utf32 = (int)(smpChar - UNICODE_PLANE01_START);
            leadingSurrogate = (char)((utf32 / 0x400) + HIGH_SURROGATE_START);
            trailingSurrogate = (char)((utf32 % 0x400) + LOW_SURROGATE_START);
        }

        public static string HtmlDecode(string value) {
            if (value == null) {
                return string.Empty;
            }

            if (value.IndexOf('&') < 0) {
                return value;
            }

            var output = new StringWriter();
            var htmlEntityEndingChars = new char[] {';', '&'};
            int l = value.Length;
            for (int i = 0; i < l; i++) {
                char ch = value[i];

                if (ch == '&') {
                    // We found a '&'. Now look for the next ';' or '&'. The idea is that
                    // if we find another '&' before finding a ';', then this is not an entity,
                    // and the next '&' might start a real entity (VSWhidbey 275184)
                    int index = value.IndexOfAny(htmlEntityEndingChars, i + 1);
                    if (index > 0 && value[index] == ';') {
                        string entity = value.Substring(i + 1, index - i - 1);

                        if (entity.Length > 1 && entity[0] == '#') {
                            // The # syntax can be in decimal or hex, e.g.
                            //      &#229;  --> decimal
                            //      &#xE5;  --> same char in hex
                            // See http://www.w3.org/TR/REC-html40/charset.html#entities

                            bool parsedSuccessfully;
                            uint parsedValue;
                            if (entity[1] == 'x' || entity[1] == 'X') {
                                parsedSuccessfully = UInt32.TryParse(entity.Substring(2), NumberStyles.AllowHexSpecifier, NumberFormatInfo.InvariantInfo, out parsedValue);
                            } else {
                                parsedSuccessfully = UInt32.TryParse(entity.Substring(1), NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out parsedValue);
                            }

                            if (parsedSuccessfully) {
                                parsedSuccessfully = (0 < parsedValue && parsedValue <= UNICODE_PLANE00_END);
                            }

                            if (parsedSuccessfully) {
                                if (parsedValue <= UNICODE_PLANE00_END) {
                                    // single character
                                    output.Write((char)parsedValue);
                                } else {
                                    // multi-character
                                    char leadingSurrogate, trailingSurrogate;
                                    ConvertSmpToUtf16(parsedValue, out leadingSurrogate, out trailingSurrogate);
                                    output.Write(leadingSurrogate);
                                    output.Write(trailingSurrogate);
                                }

                                i = index; // already looked at everything until semicolon
                                continue;
                            }
                        } else {
                            i = index; // already looked at everything until semicolon

                            char entityChar = HtmlEntities.Lookup(entity);
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
        
    }

    public static class HtmlEntities {

        // The list is from http://www.w3.org/TR/REC-html40/sgml/entities.html, except for &apos;, which
        // is defined in http://www.w3.org/TR/2008/REC-xml-20081126/#sec-predefined-ent.

        private static readonly String[] EntitiesList = new String[] {
                "\x0022-quot",
                "\x0026-amp",
                "\x0027-apos",
                "\x003c-lt",
                "\x003e-gt",
                "\x00a0-nbsp",
                "\x00a1-iexcl",
                "\x00a2-cent",
                "\x00a3-pound",
                "\x00a4-curren",
                "\x00a5-yen",
                "\x00a6-brvbar",
                "\x00a7-sect",
                "\x00a8-uml",
                "\x00a9-copy",
                "\x00aa-ordf",
                "\x00ab-laquo",
                "\x00ac-not",
                "\x00ad-shy",
                "\x00ae-reg",
                "\x00af-macr",
                "\x00b0-deg",
                "\x00b1-plusmn",
                "\x00b2-sup2",
                "\x00b3-sup3",
                "\x00b4-acute",
                "\x00b5-micro",
                "\x00b6-para",
                "\x00b7-middot",
                "\x00b8-cedil",
                "\x00b9-sup1",
                "\x00ba-ordm",
                "\x00bb-raquo",
                "\x00bc-frac14",
                "\x00bd-frac12",
                "\x00be-frac34",
                "\x00bf-iquest",
                "\x00c0-Agrave",
                "\x00c1-Aacute",
                "\x00c2-Acirc",
                "\x00c3-Atilde",
                "\x00c4-Auml",
                "\x00c5-Aring",
                "\x00c6-AElig",
                "\x00c7-Ccedil",
                "\x00c8-Egrave",
                "\x00c9-Eacute",
                "\x00ca-Ecirc",
                "\x00cb-Euml",
                "\x00cc-Igrave",
                "\x00cd-Iacute",
                "\x00ce-Icirc",
                "\x00cf-Iuml",
                "\x00d0-ETH",
                "\x00d1-Ntilde",
                "\x00d2-Ograve",
                "\x00d3-Oacute",
                "\x00d4-Ocirc",
                "\x00d5-Otilde",
                "\x00d6-Ouml",
                "\x00d7-times",
                "\x00d8-Oslash",
                "\x00d9-Ugrave",
                "\x00da-Uacute",
                "\x00db-Ucirc",
                "\x00dc-Uuml",
                "\x00dd-Yacute",
                "\x00de-THORN",
                "\x00df-szlig",
                "\x00e0-agrave",
                "\x00e1-aacute",
                "\x00e2-acirc",
                "\x00e3-atilde",
                "\x00e4-auml",
                "\x00e5-aring",
                "\x00e6-aelig",
                "\x00e7-ccedil",
                "\x00e8-egrave",
                "\x00e9-eacute",
                "\x00ea-ecirc",
                "\x00eb-euml",
                "\x00ec-igrave",
                "\x00ed-iacute",
                "\x00ee-icirc",
                "\x00ef-iuml",
                "\x00f0-eth",
                "\x00f1-ntilde",
                "\x00f2-ograve",
                "\x00f3-oacute",
                "\x00f4-ocirc",
                "\x00f5-otilde",
                "\x00f6-ouml",
                "\x00f7-divide",
                "\x00f8-oslash",
                "\x00f9-ugrave",
                "\x00fa-uacute",
                "\x00fb-ucirc",
                "\x00fc-uuml",
                "\x00fd-yacute",
                "\x00fe-thorn",
                "\x00ff-yuml",
                "\x0152-OElig",
                "\x0153-oelig",
                "\x0160-Scaron",
                "\x0161-scaron",
                "\x0178-Yuml",
                "\x0192-fnof",
                "\x02c6-circ",
                "\x02dc-tilde",
                "\x0391-Alpha",
                "\x0392-Beta",
                "\x0393-Gamma",
                "\x0394-Delta",
                "\x0395-Epsilon",
                "\x0396-Zeta",
                "\x0397-Eta",
                "\x0398-Theta",
                "\x0399-Iota",
                "\x039a-Kappa",
                "\x039b-Lambda",
                "\x039c-Mu",
                "\x039d-Nu",
                "\x039e-Xi",
                "\x039f-Omicron",
                "\x03a0-Pi",
                "\x03a1-Rho",
                "\x03a3-Sigma",
                "\x03a4-Tau",
                "\x03a5-Upsilon",
                "\x03a6-Phi",
                "\x03a7-Chi",
                "\x03a8-Psi",
                "\x03a9-Omega",
                "\x03b1-alpha",
                "\x03b2-beta",
                "\x03b3-gamma",
                "\x03b4-delta",
                "\x03b5-epsilon",
                "\x03b6-zeta",
                "\x03b7-eta",
                "\x03b8-theta",
                "\x03b9-iota",
                "\x03ba-kappa",
                "\x03bb-lambda",
                "\x03bc-mu",
                "\x03bd-nu",
                "\x03be-xi",
                "\x03bf-omicron",
                "\x03c0-pi",
                "\x03c1-rho",
                "\x03c2-sigmaf",
                "\x03c3-sigma",
                "\x03c4-tau",
                "\x03c5-upsilon",
                "\x03c6-phi",
                "\x03c7-chi",
                "\x03c8-psi",
                "\x03c9-omega",
                "\x03d1-thetasym",
                "\x03d2-upsih",
                "\x03d6-piv",
                "\x2002-ensp",
                "\x2003-emsp",
                "\x2009-thinsp",
                "\x200c-zwnj",
                "\x200d-zwj",
                "\x200e-lrm",
                "\x200f-rlm",
                "\x2013-ndash",
                "\x2014-mdash",
                "\x2018-lsquo",
                "\x2019-rsquo",
                "\x201a-sbquo",
                "\x201c-ldquo",
                "\x201d-rdquo",
                "\x201e-bdquo",
                "\x2020-dagger",
                "\x2021-Dagger",
                "\x2022-bull",
                "\x2026-hellip",
                "\x2030-permil",
                "\x2032-prime",
                "\x2033-Prime",
                "\x2039-lsaquo",
                "\x203a-rsaquo",
                "\x203e-oline",
                "\x2044-frasl",
                "\x20ac-euro",
                "\x2111-image",
                "\x2118-weierp",
                "\x211c-real",
                "\x2122-trade",
                "\x2135-alefsym",
                "\x2190-larr",
                "\x2191-uarr",
                "\x2192-rarr",
                "\x2193-darr",
                "\x2194-harr",
                "\x21b5-crarr",
                "\x21d0-lArr",
                "\x21d1-uArr",
                "\x21d2-rArr",
                "\x21d3-dArr",
                "\x21d4-hArr",
                "\x2200-forall",
                "\x2202-part",
                "\x2203-exist",
                "\x2205-empty",
                "\x2207-nabla",
                "\x2208-isin",
                "\x2209-notin",
                "\x220b-ni",
                "\x220f-prod",
                "\x2211-sum",
                "\x2212-minus",
                "\x2217-lowast",
                "\x221a-radic",
                "\x221d-prop",
                "\x221e-infin",
                "\x2220-ang",
                "\x2227-and",
                "\x2228-or",
                "\x2229-cap",
                "\x222a-cup",
                "\x222b-int",
                "\x2234-there4",
                "\x223c-sim",
                "\x2245-cong",
                "\x2248-asymp",
                "\x2260-ne",
                "\x2261-equiv",
                "\x2264-le",
                "\x2265-ge",
                "\x2282-sub",
                "\x2283-sup",
                "\x2284-nsub",
                "\x2286-sube",
                "\x2287-supe",
                "\x2295-oplus",
                "\x2297-otimes",
                "\x22a5-perp",
                "\x22c5-sdot",
                "\x2308-lceil",
                "\x2309-rceil",
                "\x230a-lfloor",
                "\x230b-rfloor",
                "\x2329-lang",
                "\x232a-rang",
                "\x25ca-loz",
                "\x2660-spades",
                "\x2663-clubs",
                "\x2665-hearts",
                "\x2666-diams",
            };

        private static readonly Dictionary<string, char> LookupTable = GenerateLookupTable();

        private static Dictionary<string, char> GenerateLookupTable() {
            // e[0] is unicode char, e[1] is '-', e[2+] is entity string

            var lookupTable = new Dictionary<string, char>(StringComparer.Ordinal);
            foreach (var e in EntitiesList) {
                lookupTable.Add(e.Substring(2), e[0]);
            }

            return lookupTable;
        }

        public static char Lookup(string entity) {
            char theChar;
            LookupTable.TryGetValue(entity, out theChar);
            return theChar;
        }
    }
}