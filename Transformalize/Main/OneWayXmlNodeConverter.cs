#region License
// Copyright (c) 2007 James Newton-King
//
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Transformalize.Libs.Newtonsoft.Json;
using Transformalize.Libs.Newtonsoft.Json.Converters;
using Transformalize.Libs.Newtonsoft.Json.Utilities;
#if !PORTABLE40
#if !(NET20 || PORTABLE40)
#endif
#if NET20
using Newtonsoft.Json.Utilities.LinqBridge;
#else

#endif

namespace Transformalize.Main
{
    /// <summary>
    /// Converts XML to JSON.
    /// </summary>
    public class OneWayXmlNodeConverter : JsonConverter
    {
        private const string TextName = "#text";
        private const string CommentName = "#comment";
        private const string CDataName = "#cdata-section";
        private const string WhitespaceName = "#whitespace";
        private const string SignificantWhitespaceName = "#significant-whitespace";
        private const string DeclarationName = "?xml";
        private const string JsonNamespaceUri = "http://james.newtonking.com/projects/json";

        /// <summary>
        /// Gets or sets the name of the root element to insert when deserializing to XML if the JSON structure has produces multiple root elements.
        /// </summary>
        /// <value>The name of the deserialize root element.</value>
        public string DeserializeRootElementName { get; set; }

        /// <summary>
        /// Gets or sets a flag to indicate whether to write the Json.NET array attribute.
        /// This attribute helps preserve arrays when converting the written XML back to JSON.
        /// </summary>
        /// <value><c>true</c> if the array attibute is written to the XML; otherwise, <c>false</c>.</value>
        public bool WriteArrayAttribute { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to write the root JSON object.
        /// </summary>
        /// <value><c>true</c> if the JSON root object is omitted; otherwise, <c>false</c>.</value>
        public bool OmitRootObject { get; set; }

        #region Writing
        /// <summary>
        /// Writes the JSON representation of the object.
        /// </summary>
        /// <param name="writer">The <see cref="JsonWriter"/> to write to.</param>
        /// <param name="serializer">The calling serializer.</param>
        /// <param name="value">The value.</param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            IXmlNode node = WrapXml(value);

            XmlNamespaceManager manager = new XmlNamespaceManager(new NameTable());
            PushParentNamespaces(node, manager);

            if (!OmitRootObject)
                writer.WriteStartObject();

            SerializeNode(writer, node, manager, !OmitRootObject);

            if (!OmitRootObject)
                writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        private IXmlNode WrapXml(object value)
        {
#if !NET20
            if (value is XObject)
                return XContainerWrapper.WrapNode((XObject)value);
#endif
#if !(NETFX_CORE || PORTABLE)
            if (value is XmlNode)
                return XmlNodeWrapper.WrapNode((XmlNode)value);
#endif

            throw new ArgumentException("Value must be an XML object.", "value");
        }

        private void PushParentNamespaces(IXmlNode node, XmlNamespaceManager manager)
        {
            List<IXmlNode> parentElements = null;

            IXmlNode parent = node;
            while ((parent = parent.ParentNode) != null)
            {
                if (parent.NodeType == XmlNodeType.Element)
                {
                    if (parentElements == null)
                        parentElements = new List<IXmlNode>();

                    parentElements.Add(parent);
                }
            }

            if (parentElements != null)
            {
                parentElements.Reverse();

                foreach (IXmlNode parentElement in parentElements)
                {
                    manager.PushScope();
                    foreach (IXmlNode attribute in parentElement.Attributes)
                    {
                        if (attribute.NamespaceUri == "http://www.w3.org/2000/xmlns/" && attribute.LocalName != "xmlns")
                            manager.AddNamespace(attribute.LocalName, attribute.Value);
                    }
                }
            }
        }

        private string ResolveFullName(IXmlNode node, XmlNamespaceManager manager)
        {
            string prefix = (node.NamespaceUri == null || (node.LocalName == "xmlns" && node.NamespaceUri == "http://www.w3.org/2000/xmlns/"))
                ? null
                : manager.LookupPrefix(node.NamespaceUri);

            if (!string.IsNullOrEmpty(prefix))
                return prefix + ":" + node.LocalName;
            else
                return node.LocalName;
        }

        private string GetPropertyName(IXmlNode node, XmlNamespaceManager manager)
        {
            switch (node.NodeType)
            {
                case XmlNodeType.Attribute:
                    if (node.NamespaceUri == JsonNamespaceUri)
                        return "$" + node.LocalName;
                    return ResolveFullName(node, manager);
                case XmlNodeType.CDATA:
                    return CDataName;
                case XmlNodeType.Comment:
                    return CommentName;
                case XmlNodeType.Element:
                    return ResolveFullName(node, manager);
                case XmlNodeType.ProcessingInstruction:
                    return "?" + ResolveFullName(node, manager);
                case XmlNodeType.DocumentType:
                    return "!" + ResolveFullName(node, manager);
                case XmlNodeType.XmlDeclaration:
                    return DeclarationName;
                case XmlNodeType.SignificantWhitespace:
                    return SignificantWhitespaceName;
                case XmlNodeType.Text:
                    return TextName;
                case XmlNodeType.Whitespace:
                    return WhitespaceName;
                default:
                    throw new JsonSerializationException("Unexpected XmlNodeType when getting node name: " + node.NodeType);
            }
        }

        private bool IsArray(IXmlNode node)
        {
            IXmlNode jsonArrayAttribute = (node.Attributes != null)
                ? node.Attributes.SingleOrDefault(a => a.LocalName == "Array" && a.NamespaceUri == JsonNamespaceUri)
                : null;

            return (jsonArrayAttribute != null && XmlConvert.ToBoolean(jsonArrayAttribute.Value));
        }

        private void SerializeGroupedNodes(JsonWriter writer, IXmlNode node, XmlNamespaceManager manager, bool writePropertyName)
        {
            // group nodes together by name
            Dictionary<string, List<IXmlNode>> nodesGroupedByName = new Dictionary<string, List<IXmlNode>>();

            for (int i = 0; i < node.ChildNodes.Count; i++)
            {
                IXmlNode childNode = node.ChildNodes[i];
                string nodeName = GetPropertyName(childNode, manager);

                List<IXmlNode> nodes;
                if (!nodesGroupedByName.TryGetValue(nodeName, out nodes))
                {
                    nodes = new List<IXmlNode>();
                    nodesGroupedByName.Add(nodeName, nodes);
                }

                nodes.Add(childNode);
            }

            // loop through grouped nodes. write single name instances as normal,
            // write multiple names together in an array
            foreach (KeyValuePair<string, List<IXmlNode>> nodeNameGroup in nodesGroupedByName)
            {
                List<IXmlNode> groupedNodes = nodeNameGroup.Value;
                bool writeArray;

                if (groupedNodes.Count == 1)
                {
                    writeArray = IsArray(groupedNodes[0]);
                }
                else
                {
                    writeArray = true;
                }

                if (!writeArray)
                {
                    SerializeNode(writer, groupedNodes[0], manager, writePropertyName);
                }
                else
                {
                    string elementNames = nodeNameGroup.Key;

                    if (writePropertyName)
                        writer.WritePropertyName(elementNames);

                    writer.WriteStartArray();

                    for (int i = 0; i < groupedNodes.Count; i++)
                    {
                        SerializeNode(writer, groupedNodes[i], manager, false);
                    }

                    writer.WriteEndArray();
                }
            }
        }

        private static IEnumerable<IXmlNode> ValueAttributes(IEnumerable<IXmlNode> c) {
            return c.Where(a => a.NamespaceUri != JsonNamespaceUri);
        }

        private void SerializeNode(JsonWriter writer, IXmlNode node, XmlNamespaceManager manager, bool writePropertyName)
        {
            switch (node.NodeType)
            {
                case XmlNodeType.Document:
                case XmlNodeType.DocumentFragment:
                    SerializeGroupedNodes(writer, node, manager, writePropertyName);
                    break;
                case XmlNodeType.Element:
                    if (IsArray(node) && node.ChildNodes.All(n => n.LocalName == node.LocalName) && node.ChildNodes.Count > 0)
                    {
                        SerializeGroupedNodes(writer, node, manager, false);
                    }
                    else
                    {
                        manager.PushScope();

                        foreach (IXmlNode attribute in node.Attributes)
                        {
                            if (attribute.NamespaceUri == "http://www.w3.org/2000/xmlns/")
                            {
                                string namespacePrefix = (attribute.LocalName != "xmlns")
                                    ? attribute.LocalName
                                    : string.Empty;
                                string namespaceUri = attribute.Value;

                                manager.AddNamespace(namespacePrefix, namespaceUri);
                            }
                        }

                        if (writePropertyName)
                            writer.WritePropertyName(GetPropertyName(node, manager));

                        if (!ValueAttributes(node.Attributes).Any() && node.ChildNodes.Count == 1
                            && node.ChildNodes[0].NodeType == XmlNodeType.Text)
                        {
                            // write elements with a single text child as a name value pair
                            writer.WriteValue(node.ChildNodes[0].Value);
                        }
                        else if (node.ChildNodes.Count == 0 && CollectionUtils.IsNullOrEmpty(node.Attributes))
                        {
                            IXmlElement element = (IXmlElement)node;

                            // empty element
                            if (element.IsEmpty)
                                writer.WriteNull();
                            else
                                writer.WriteValue(string.Empty);
                        }
                        else
                        {
                            writer.WriteStartObject();

                            for (int i = 0; i < node.Attributes.Count; i++)
                            {
                                SerializeNode(writer, node.Attributes[i], manager, true);
                            }

                            SerializeGroupedNodes(writer, node, manager, true);

                            writer.WriteEndObject();
                        }

                        manager.PopScope();
                    }

                    break;
                case XmlNodeType.Comment:
                    if (writePropertyName)
                        writer.WriteComment(node.Value);
                    break;
                case XmlNodeType.Attribute:
                case XmlNodeType.Text:
                case XmlNodeType.CDATA:
                case XmlNodeType.ProcessingInstruction:
                case XmlNodeType.Whitespace:
                case XmlNodeType.SignificantWhitespace:
                    if (node.NamespaceUri == "http://www.w3.org/2000/xmlns/" && node.Value == JsonNamespaceUri)
                        return;

                    if (node.NamespaceUri == JsonNamespaceUri)
                    {
                        if (node.LocalName == "Array")
                            return;
                    }

                    if (writePropertyName)
                        writer.WritePropertyName(GetPropertyName(node, manager));
                    writer.WriteValue(node.Value);
                    break;
                case XmlNodeType.XmlDeclaration:
                    IXmlDeclaration declaration = (IXmlDeclaration)node;
                    writer.WritePropertyName(GetPropertyName(node, manager));
                    writer.WriteStartObject();

                    if (!string.IsNullOrEmpty(declaration.Version))
                    {
                        writer.WritePropertyName("@version");
                        writer.WriteValue(declaration.Version);
                    }
                    if (!string.IsNullOrEmpty(declaration.Encoding))
                    {
                        writer.WritePropertyName("@encoding");
                        writer.WriteValue(declaration.Encoding);
                    }
                    if (!string.IsNullOrEmpty(declaration.Standalone))
                    {
                        writer.WritePropertyName("@standalone");
                        writer.WriteValue(declaration.Standalone);
                    }

                    writer.WriteEndObject();
                    break;
                case XmlNodeType.DocumentType:
                    IXmlDocumentType documentType = (IXmlDocumentType)node;
                    writer.WritePropertyName(GetPropertyName(node, manager));
                    writer.WriteStartObject();

                    if (!string.IsNullOrEmpty(documentType.Name))
                    {
                        writer.WritePropertyName("@name");
                        writer.WriteValue(documentType.Name);
                    }
                    if (!string.IsNullOrEmpty(documentType.Public))
                    {
                        writer.WritePropertyName("@public");
                        writer.WriteValue(documentType.Public);
                    }
                    if (!string.IsNullOrEmpty(documentType.System))
                    {
                        writer.WritePropertyName("@system");
                        writer.WriteValue(documentType.System);
                    }
                    if (!string.IsNullOrEmpty(documentType.InternalSubset))
                    {
                        writer.WritePropertyName("@internalSubset");
                        writer.WriteValue(documentType.InternalSubset);
                    }

                    writer.WriteEndObject();
                    break;
                default:
                    throw new JsonSerializationException("Unexpected XmlNodeType when serializing nodes: " + node.NodeType);
            }
        }
        #endregion

        /// <summary>
        /// Determines whether this instance can convert the specified value type.
        /// </summary>
        /// <param name="valueType">Type of the value.</param>
        /// <returns>
        /// 	<c>true</c> if this instance can convert the specified value type; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanConvert(Type valueType)
        {
#if !NET20
            if (typeof(XObject).IsAssignableFrom(valueType))
                return true;
#endif
#if !(NETFX_CORE || PORTABLE)
            if (typeof(XmlNode).IsAssignableFrom(valueType))
                return true;
#endif

            return false;
        }
    }
}

#endif
