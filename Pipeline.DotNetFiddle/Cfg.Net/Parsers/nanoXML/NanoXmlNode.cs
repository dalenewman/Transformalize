#region license
// Cfg.Net
// Copyright 2015 Dale Newman
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//  
//      http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cfg.Net.Parsers.nanoXML {
    /// <summary>
    ///     Element node of document
    /// </summary>
    public class NanoXmlNode : NanoXmlBase {
        internal NanoXmlNode(string str, ref int i) {
            Name = ParseAttributes(str, ref i, Attributes, '>', '/');

            if (str[i] == '/') // if this node has nothing inside
            {
                i++; // skip /
                i++; // skip >
                return;
            }

            i++; // skip >

            // temporary. to include all whitespaces into value, if any
            int tempI = i;

            SkipSpaces(str, ref tempI);

            if (str[tempI] == '<') {
                i = tempI;

                while (str[i + 1] != '/') // parse subnodes
                {
                    i++; // skip <
                    SubNodes.Add(new NanoXmlNode(str, ref i));

                    SkipSpaces(str, ref i);

                    if (i >= str.Length)
                        return; // EOF

                    if (str[i] != '<')
                        throw new NanoXmlParsingException($"Unexpected token in XML.  Expecting <, but found {str[i]}.");
                }

                i++; // skip <
            } else // parse value
              {
                Value = GetValue(str, ref i, '<', '\0', false);
                i++; // skip <

                if (str[i] != '/')
                    throw new NanoXmlParsingException("Invalid ending on tag " + Name);
            }

            i++; // skip /
            SkipSpaces(str, ref i);

            string endName = GetValue(str, ref i, '>', '\0', true);
            if (endName != Name)
                throw new NanoXmlParsingException("Start/end tag name mismatch: " + Name + " and " + endName);
            SkipSpaces(str, ref i);

            if (str[i] != '>')
                throw new NanoXmlParsingException("Invalid ending on tag " + Name);

            i++; // skip >
        }

        /// <summary>
        ///     Element value
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        ///     Element name
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///     List of subelements
        /// </summary>
        public List<NanoXmlNode> SubNodes { get; } = new List<NanoXmlNode>();

        /// <summary>
        ///     List of attributes
        /// </summary>
        public List<NanoXmlAttribute> Attributes { get; } = new List<NanoXmlAttribute>();

        /// <summary>
        ///     Returns subelement by given name
        /// </summary>
        /// <param name="nodeName">Name of subelement to get</param>
        /// <returns>First subelement with given name or NULL if no such element</returns>
        public NanoXmlNode this[string nodeName] {
            get {
                for (var i = 0; i < SubNodes.Count; i++) {
                    var nanoXmlNode = SubNodes[i];
                    if (nanoXmlNode.Name == nodeName)
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
            for (var i = 0; i < Attributes.Count; i++) {
                var nanoXmlAttribute = Attributes[i];
                if (nanoXmlAttribute.Name == attributeName)
                    return nanoXmlAttribute;
            }

            return null;
        }

        public bool TryAttribute(string attributeName, out NanoXmlAttribute attribute) {
            for (var i = 0; i < Attributes.Count; i++) {
                var nanoXmlAttribute = Attributes[i];
                if (nanoXmlAttribute.Name != attributeName)
                    continue;
                attribute = nanoXmlAttribute;
                return true;
            }

            attribute = null;
            return false;
        }

        public bool HasAttribute(string attributeName) {
            return Attributes.Any(nanoXmlAttribute => nanoXmlAttribute.Name == attributeName);
        }

        public string InnerText() {
            var builder = new StringBuilder();
            InnerText(ref builder);
            return builder.ToString();
        }

        private void InnerText(ref StringBuilder builder) {
            for (int i = 0; i < SubNodes.Count; i++) {
                NanoXmlNode node = SubNodes[i];
                builder.Append("<");
                builder.Append(node.Name);
                foreach (NanoXmlAttribute attribute in node.Attributes) {
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
            foreach (NanoXmlAttribute attribute in Attributes) {
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

        public bool HasSubNode() {
            return SubNodes != null && SubNodes.Any();
        }
    }
}