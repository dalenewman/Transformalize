#region License
// /*
// See license included in this library folder.
// */
#endregion

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;

namespace Transformalize.Libs.NLog.Config
{
    /// <summary>
    ///     Represents simple XML element with case-insensitive attribute semantics.
    /// </summary>
    internal class NLogXmlElement
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="NLogXmlElement" /> class.
        /// </summary>
        /// <param name="inputUri">The input URI.</param>
        public NLogXmlElement(string inputUri)
            : this()
        {
            using (var reader = XmlReader.Create(inputUri))
            {
                reader.MoveToContent();
                Parse(reader);
            }
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="NLogXmlElement" /> class.
        /// </summary>
        /// <param name="reader">The reader to initialize element from.</param>
        public NLogXmlElement(XmlReader reader)
            : this()
        {
            Parse(reader);
        }

        /// <summary>
        ///     Prevents a default instance of the <see cref="NLogXmlElement" /> class from being created.
        /// </summary>
        private NLogXmlElement()
        {
            AttributeValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            Children = new List<NLogXmlElement>();
        }

        /// <summary>
        ///     Gets the element name.
        /// </summary>
        public string LocalName { get; private set; }

        /// <summary>
        ///     Gets the dictionary of attribute values.
        /// </summary>
        public Dictionary<string, string> AttributeValues { get; private set; }

        /// <summary>
        ///     Gets the collection of child elements.
        /// </summary>
        public IList<NLogXmlElement> Children { get; private set; }

        /// <summary>
        ///     Gets the value of the element.
        /// </summary>
        public string Value { get; private set; }

        /// <summary>
        ///     Returns children elements with the specified element name.
        /// </summary>
        /// <param name="elementName">Name of the element.</param>
        /// <returns>Children elements with the specified element name.</returns>
        public IEnumerable<NLogXmlElement> Elements(string elementName)
        {
            var result = new List<NLogXmlElement>();

            foreach (var ch in Children)
            {
                if (ch.LocalName.Equals(elementName, StringComparison.OrdinalIgnoreCase))
                {
                    result.Add(ch);
                }
            }

            return result;
        }

        /// <summary>
        ///     Gets the required attribute.
        /// </summary>
        /// <param name="attributeName">Name of the attribute.</param>
        /// <returns>Attribute value.</returns>
        /// <remarks>Throws if the attribute is not specified.</remarks>
        public string GetRequiredAttribute(string attributeName)
        {
            var value = GetOptionalAttribute(attributeName, null);
            if (value == null)
            {
                throw new NLogConfigurationException("Expected " + attributeName + " on <" + LocalName + " />");
            }

            return value;
        }

        /// <summary>
        ///     Gets the optional boolean attribute value.
        /// </summary>
        /// <param name="attributeName">Name of the attribute.</param>
        /// <param name="defaultValue">Default value to return if the attribute is not found.</param>
        /// <returns>Boolean attribute value or default.</returns>
        public bool GetOptionalBooleanAttribute(string attributeName, bool defaultValue)
        {
            string value;

            if (!AttributeValues.TryGetValue(attributeName, out value))
            {
                return defaultValue;
            }

            return Convert.ToBoolean(value, CultureInfo.InvariantCulture);
        }

        /// <summary>
        ///     Gets the optional attribute value.
        /// </summary>
        /// <param name="attributeName">Name of the attribute.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>Value of the attribute or default value.</returns>
        public string GetOptionalAttribute(string attributeName, string defaultValue)
        {
            string value;

            if (!AttributeValues.TryGetValue(attributeName, out value))
            {
                value = defaultValue;
            }

            return value;
        }

        /// <summary>
        ///     Asserts that the name of the element is among specified element names.
        /// </summary>
        /// <param name="allowedNames">The allowed names.</param>
        public void AssertName(params string[] allowedNames)
        {
            foreach (var en in allowedNames)
            {
                if (LocalName.Equals(en, StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }
            }

            throw new InvalidOperationException("Assertion failed. Expected element name '" + string.Join("|", allowedNames) + "', actual: '" + LocalName + "'.");
        }

        private void Parse(XmlReader reader)
        {
            if (reader.MoveToFirstAttribute())
            {
                do
                {
                    AttributeValues.Add(reader.LocalName, reader.Value);
                } while (reader.MoveToNextAttribute());

                reader.MoveToElement();
            }

            LocalName = reader.LocalName;

            if (!reader.IsEmptyElement)
            {
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.EndElement)
                    {
                        break;
                    }

                    if (reader.NodeType == XmlNodeType.CDATA || reader.NodeType == XmlNodeType.Text)
                    {
                        Value += reader.Value;
                        continue;
                    }

                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        Children.Add(new NLogXmlElement(reader));
                    }
                }
            }
        }
    }
}