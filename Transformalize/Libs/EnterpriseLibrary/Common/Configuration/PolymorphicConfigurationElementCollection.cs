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
using System.Collections.Generic;
using System.Xml;

namespace Transformalize.Libs.EnterpriseLibrary.Common.Configuration
{
	/// <summary>
	/// Represents the base class from which all implementations of polymorphic configuration collections must derive. 
	/// </summary>
	/// <typeparam name="T">The type contained in the collection.</typeparam>	
	public abstract class PolymorphicConfigurationElementCollection<T> : NamedElementCollection<T>
		where T : NamedConfigurationElement, new()
	{
		private Dictionary<string, Type> configurationElementTypeMapping;
		private T currentElement;

       
        /// <summary>
        /// Resets the internal state of the <see cref="ConfigurationElement"/> object, including the locks and the properties collections.
        /// </summary>
        /// <param name="parentElement">The parent node of the configuration element.</param>
        protected override void Reset(ConfigurationElement parentElement)
        {
            CreateTypesMap((PolymorphicConfigurationElementCollection<T>)parentElement);

            base.Reset(parentElement);

            ReleaseTypesMap();
        }

		/// <summary>
		/// Called when an unknown element is encountered while deserializing the <see cref="ConfigurationElement"/> object.
		/// </summary>
		/// <param name="elementName">The name of the element.</param>
		/// <param name="reader">The <see cref="XmlReader"/> used to deserialize the element.</param>
		/// <returns><see langword="true"/> if the element was handled; otherwise, <see langword="false"/>.</returns>
		protected override bool OnDeserializeUnrecognizedElement(string elementName, XmlReader reader)
		{
			if (this.AddElementName.Equals(elementName))
			{
				Type configurationElementType = RetrieveConfigurationElementType(reader);
				currentElement = (T)Activator.CreateInstance(configurationElementType);
				currentElement.DeserializeElement(reader);
				base.Add(currentElement);
				return true;
			}
			return base.OnDeserializeUnrecognizedElement(elementName, reader);
		}

		/// <summary>
		/// When overriden in a class, get the configuration object for each <see cref="NameTypeConfigurationElement"/> object in the collection.
		/// </summary>
		/// <param name="reader">The <see cref="XmlReader"/> that is deserializing the element.</param>
		protected abstract Type RetrieveConfigurationElementType(XmlReader reader);			

		/// <summary>
		/// Creates a new <see cref="ConfigurationElement"/>. 
		/// </summary>
		/// <returns>A new <see cref="ConfigurationElement"/>.</returns>
		protected override ConfigurationElement CreateNewElement()
		{
			// create a new instance of the type we need...
			if (currentElement != null)
			{
				return currentElement;
			}
			else
			{
				return new T();
			}
		}

		/// <summary>
		/// Creates a new named <see cref="ConfigurationElement"/>.
		/// </summary>
		/// <param name="elementName">The name of the element to create.</param>
		/// <returns>A new <see cref="ConfigurationElement"/>.</returns>
		protected override ConfigurationElement CreateNewElement(string elementName)
		{
			if (configurationElementTypeMapping != null)
			{
				Type configurationElementType = configurationElementTypeMapping[elementName];
				if (configurationElementType != null)
				{
					return Activator.CreateInstance(configurationElementType) as ConfigurationElement;
				}
			}
			return base.CreateNewElement(elementName);
		}
		
		/// <summary>
		/// Reverses the effect of merging configuration information from different levels of the configuration hierarchy.
		/// </summary>
		/// <param name="sourceElement">A <see cref="ConfigurationElement"/> object at the current level containing a merged view of the properties.</param>
		/// <param name="parentElement">The parent <see cref="ConfigurationElement"/> object of the current element, or a <see langword="null"/> reference (Nothing in Visual Basic) if this is the top level.</param>		
		/// <param name="saveMode">One of the <see cref="ConfigurationSaveMode"/> values.</param>
		protected override void Unmerge(ConfigurationElement sourceElement, ConfigurationElement parentElement, ConfigurationSaveMode saveMode)
		{
		    CreateTypesMap((PolymorphicConfigurationElementCollection<T>)sourceElement);
		    base.Unmerge(sourceElement, parentElement, saveMode);
		    ReleaseTypesMap();
		}

		private void CreateTypesMap(PolymorphicConfigurationElementCollection<T> sourceCollection)
		{
		    configurationElementTypeMapping = new Dictionary<string, Type>(sourceCollection.Count);

		    foreach (T configurationElementSettings in sourceCollection)
		    {
		        configurationElementTypeMapping.Add(configurationElementSettings.Name, configurationElementSettings.GetType());
		    }
		}

		private void ReleaseTypesMap()
		{
		    configurationElementTypeMapping = null;
		}
	}
}
