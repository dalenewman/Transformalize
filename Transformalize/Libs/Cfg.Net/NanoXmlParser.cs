// credits to http://www.codeproject.com/Tips/682245/NanoXML-Simple-and-fast-XML-parser

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Transformalize.Libs.Cfg.Net {
    /// <summary>
    ///     Base class containing useful features for all XML classes
    /// </summary>
    public class NanoXmlBase {
        protected static bool IsSpace(char c) {
            return c == ' ' || c == '\t' || c == '\n' || c == '\r';
        }

        protected static void SkipSpaces(string str, ref int i) {
            while (i < str.Length) {
                if (!IsSpace(str[i])) {
                    if (str[i] == '<' && i + 4 < str.Length && str[i + 1] == '!' && str[i + 2] == '-' && str[i + 3] == '-') {
                        i += 4; // skip <!--

                        while (i + 2 < str.Length && !(str[i] == '-' && str[i + 1] == '-'))
                            i++;

                        i += 2; // skip --
                    } else
                        break;
                }

                i++;
            }
        }

        protected static string GetValue(string str, ref int i, char endChar, char endChar2, bool stopOnSpace) {
            int start = i;
            while ((!stopOnSpace || !IsSpace(str[i])) && str[i] != endChar && str[i] != endChar2)
                i++;

            return str.Substring(start, i - start);
        }

        protected static bool IsQuote(char c) {
            return c == '"' || c == '\'';
        }

        // returns name
        protected static string ParseAttributes(string str, ref int i, List<NanoXmlAttribute> attributes, char endChar, char endChar2) {
            SkipSpaces(str, ref i);
            string name = GetValue(str, ref i, endChar, endChar2, true);

            SkipSpaces(str, ref i);

            while (str[i] != endChar && str[i] != endChar2) {
                string attrName = GetValue(str, ref i, '=', '\0', true);

                SkipSpaces(str, ref i);
                i++; // skip '='
                SkipSpaces(str, ref i);

                char quote = str[i];
                if (!IsQuote(quote))
                    throw new XmlParsingException("Unexpected token after " + attrName);

                i++; // skip quote
                string attrValue = GetValue(str, ref i, quote, '\0', false);
                i++; // skip quote

                attributes.Add(new NanoXmlAttribute(attrName, attrValue));

                SkipSpaces(str, ref i);
            }

            return name;
        }
    }

    /// <summary>
    ///     Class representing whole DOM XML document
    /// </summary>
    public class NanoXmlDocument : NanoXmlBase {
        private readonly List<NanoXmlAttribute> _declarations = new List<NanoXmlAttribute>();
        private readonly NanoXmlNode _rootNode;

        /// <summary>
        ///     Public constructor. Loads xml document from raw string
        /// </summary>
        /// <param name="xmlString">String with xml</param>
        public NanoXmlDocument(string xmlString) {
            var i = 0;

            while (true) {
                SkipSpaces(xmlString, ref i);

                if (xmlString[i] != '<')
                    throw new XmlParsingException("Unexpected token");

                i++; // skip <

                if (xmlString[i] == '?') // declaration
                {
                    i++; // skip ?
                    ParseAttributes(xmlString, ref i, _declarations, '?', '>');
                    i++; // skip ending ?
                    i++; // skip ending >

                    continue;
                }

                if (xmlString[i] == '!') // doctype
                {
                    while (xmlString[i] != '>') // skip doctype
                        i++;

                    i++; // skip >

                    continue;
                }

                _rootNode = new NanoXmlNode(xmlString, ref i);
                break;
            }
        }

        /// <summary>
        ///     Root document element
        /// </summary>
        public NanoXmlNode RootNode {
            get { return _rootNode; }
        }

        /// <summary>
        ///     List of XML Declarations as <see cref="NanoXmlAttribute" />
        /// </summary>
        public IEnumerable<NanoXmlAttribute> Declarations {
            get { return _declarations; }
        }
    }

    /// <summary>
    ///     Element node of document
    /// </summary>
    public class NanoXmlNode : NanoXmlBase {
        private readonly List<NanoXmlAttribute> attributes = new List<NanoXmlAttribute>();
        private readonly string _name;

        private readonly List<NanoXmlNode> _subNodes = new List<NanoXmlNode>();

        internal NanoXmlNode(string str, ref int i) {
            _name = ParseAttributes(str, ref i, attributes, '>', '/');

            if (str[i] == '/') // if this node has nothing inside
            {
                i++; // skip /
                i++; // skip >
                return;
            }

            i++; // skip >

            // temporary. to include all whitespaces into value, if any
            var tempI = i;

            SkipSpaces(str, ref tempI);

            if (str[tempI] == '<') {
                i = tempI;

                while (str[i + 1] != '/') // parse subnodes
                {
                    i++; // skip <
                    _subNodes.Add(new NanoXmlNode(str, ref i));

                    SkipSpaces(str, ref i);

                    if (i >= str.Length)
                        return; // EOF

                    if (str[i] != '<')
                        throw new XmlParsingException("Unexpected token");
                }

                i++; // skip <
            } else // parse value
            {
                Value = GetValue(str, ref i, '<', '\0', false);
                i++; // skip <

                if (str[i] != '/')
                    throw new XmlParsingException("Invalid ending on tag " + _name);
            }

            i++; // skip /
            SkipSpaces(str, ref i);

            var endName = GetValue(str, ref i, '>', '\0', true);
            if (endName != _name)
                throw new XmlParsingException("Start/end tag name mismatch: " + _name + " and " + endName);
            SkipSpaces(str, ref i);

            if (str[i] != '>')
                throw new XmlParsingException("Invalid ending on tag " + _name);

            i++; // skip >
        }

        /// <summary>
        ///     Element value
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        ///     Element name
        /// </summary>
        public string Name {
            get { return _name; }
        }

        /// <summary>
        ///     List of subelements
        /// </summary>
        public List<NanoXmlNode> SubNodes {
            get { return _subNodes; }
        }

        /// <summary>
        ///     List of attributes
        /// </summary>
        public List<NanoXmlAttribute> Attributes {
            get { return attributes; }
        }

        /// <summary>
        ///     Returns subelement by given name
        /// </summary>
        /// <param name="nodeName">Name of subelement to get</param>
        /// <returns>First subelement with given name or NULL if no such element</returns>
        public NanoXmlNode this[string nodeName] {
            get {
                for (var i = 0; i < _subNodes.Count; i++) {
                    var nanoXmlNode = _subNodes[i];
                    if (nanoXmlNode._name == nodeName)
                        return nanoXmlNode;
                }

                return null;
            }
        }

        /// <summary>
        ///     Returns attribute by given name
        /// </summary>
        /// <param name="attributeName">Attribute name to get</param>
        /// <returns><see cref="NanoXmlAttribute" /> with given name or null if no such attribute</returns>
        public NanoXmlAttribute GetAttribute(string attributeName) {
            for (var i = 0; i < attributes.Count; i++) {
                var nanoXmlAttribute = attributes[i];
                if (nanoXmlAttribute.Name == attributeName)
                    return nanoXmlAttribute;
            }

            return null;
        }

        public bool TryAttribute(string attributeName, out NanoXmlAttribute attribute) {
            for (var i = 0; i < attributes.Count; i++) {
                var nanoXmlAttribute = attributes[i];
                if (nanoXmlAttribute.Name != attributeName)
                    continue;
                attribute = nanoXmlAttribute;
                return true;
            }

            attribute = null;
            return false;
        }

        public bool HasAttribute(string attributeName) {
            return attributes.Any(nanoXmlAttribute => nanoXmlAttribute.Name == attributeName);
        }

        public string InnerText() {
            var builder = new StringBuilder();
            InnerText(ref builder);
            return builder.ToString();
        }

        private void InnerText(ref StringBuilder builder) {
            for (var i = 0; i < _subNodes.Count; i++) {
                var node = _subNodes[i];
                builder.Append("<");
                builder.Append(node.Name);
                foreach (var attribute in node.attributes) {
                    builder.AppendFormat(" {0}=\"{1}\"", attribute.Name, attribute.Value);
                }
                builder.Append(">");
                if (node.Value == null) {
                    node.InnerText(ref builder);
                } else {
                    builder.Append(node.Value);
                }
                builder.AppendFormat("</{0}>", node.Name);
            }
        }

        public override string ToString() {
            var builder = new StringBuilder("<");
            builder.Append(Name);
            foreach (var attribute in Attributes) {
                builder.AppendFormat(" {0}=\"{1}\"", attribute.Name, attribute.Value);
            }
            builder.Append(">");
            if (Value == null) {
                InnerText(ref builder);
            } else {
                builder.Append(Value);
            }

            builder.AppendFormat("</{0}>", Name);
            return builder.ToString();
        }

    }

    /// <summary>
    ///     XML element attribute
    /// </summary>
    public class NanoXmlAttribute {
        private readonly string _name;
        private readonly string _value;

        internal NanoXmlAttribute(string name, string value) {
            _name = name;
            _value = value;
        }

        /// <summary>
        ///     Attribute name
        /// </summary>
        public string Name {
            get { return _name; }
        }

        /// <summary>
        ///     Attribtue value
        /// </summary>
        public string Value {
            get { return _value; }
        }
    }

    internal class XmlParsingException : Exception {
        public XmlParsingException(string message) : base(message) { }
    }
}