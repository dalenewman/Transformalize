//===============================================================================
// Microsoft patterns & practices Enterprise Library
// Core
//===============================================================================
// Copyright © Microsoft Corporation.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===============================================================================

using System;
using System.Configuration;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Transformalize.Libs.EnterpriseLibrary.Common.Configuration
{
    /// <summary>
    /// Represents a configuration section that can be serialized and deserialized to XML.
    /// </summary>
    public class SerializableConfigurationSection : ConfigurationSection, IXmlSerializable
    {
        private const string SourceProperty = "source";

        /// <summary>
        /// Returns the XML schema for the configuration section.
        /// </summary>
        /// <returns>A string with the XML schema, or <see langword="null"/> (<b>Nothing</b> 
        /// in Visual Basic) if there is no schema.</returns>
        public XmlSchema GetSchema()
        {
            return null;
        }

        /// <summary>
        /// Updates the configuration section with the values from an <see cref="XmlReader"/>.
        /// </summary>
        /// <param name="reader">The <see cref="XmlReader"/> that reads the configuration source located at the element that describes the configuration section.</param>
        public void ReadXml(XmlReader reader)
        {
            if (reader == null) throw new ArgumentNullException("reader");

            reader.Read();
            DeserializeSection(reader);

        }

        /// <summary>
        /// Writes the configuration section values as an XML element to an <see cref="XmlWriter"/>.
        /// </summary>
        /// <param name="writer">The <see cref="XmlWriter"/> that writes to the configuration store.</param>
        public void WriteXml(XmlWriter writer)
        {
            if (writer == null) throw new ArgumentNullException("writer");

            String serialized = SerializeSection(this, "SerializableConfigurationSection", ConfigurationSaveMode.Full);
            writer.WriteRaw(serialized);
        }
    }
}
