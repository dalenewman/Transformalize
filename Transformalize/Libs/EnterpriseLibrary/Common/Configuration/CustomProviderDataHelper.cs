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
using System.Collections.Specialized;
using System.Text.RegularExpressions;

namespace Transformalize.Libs.EnterpriseLibrary.Common.Configuration
{
	/// <summary>
	/// Generic helper class for custom provider configuration objects.
	/// </summary>
	/// <remarks>
	/// The helper class encapsulates the logic to manage an unknown set of properties in <see cref="ConfigurationElement"/>s.
	/// This logic cannot be inherited by the configuration objects because these objects must inherit from the base configuration 
	/// object type for the hierarchy of providers the configuration object represents.
	/// </remarks>
	/// <typeparam name="T">The type of the custom provider configuration object.</typeparam>
	public class CustomProviderDataHelper<T>
		where T : NameTypeConfigurationElement, IHelperAssistedCustomConfigurationData<T>
	{
		private IHelperAssistedCustomConfigurationData<T> helpedCustomProviderData;

		private NameValueCollection attributes;
		private object lockObject = new object();

		/// <summary>
		/// Collection of managed properties
		/// </summary>
		protected internal ConfigurationPropertyCollection propertiesCollection;

		/// <summary>
		/// Initializes a new instance of the <see cref="CustomProviderDataHelper{T}"/> class for a configuration object.
		/// </summary>
		public CustomProviderDataHelper(T helpedCustomProviderData)
		{
			propertiesCollection = new ConfigurationPropertyCollection();
            foreach (ConfigurationProperty propertyInfo in helpedCustomProviderData.MetadataProperties)
            {
                propertiesCollection.Add(propertyInfo);
            }
			this.helpedCustomProviderData 
				= helpedCustomProviderData as IHelperAssistedCustomConfigurationData<T>;
		}

		/// <summary>
		/// Concrete implementation of <see cref="ConfigurationElement.IsModified()"/>.
		/// </summary>
		/// <returns><b>true</b> if the managed element has been modified.</returns>
		public bool HandleIsModified()
		{
			return UpdatePropertyCollection() || helpedCustomProviderData.BaseIsModified();
		}

		/// <summary>
		/// Concrete implementation of <see cref="ConfigurationElement.OnDeserializeUnrecognizedAttribute(string, string)"/>.
		/// </summary>
		/// <param name="name">The name of the unrecognized attribute.</param>
		/// <param name="value">The value of the unrecognized attribute.</param>
		/// <returns><code>true</code> when an unknown attribute is encountered while deserializing.</returns>
		public bool HandleOnDeserializeUnrecognizedAttribute(string name, string value)
		{
			Attributes.Add(name, value);
			return true;
		}

		/// <summary>
		/// Concrete implementation of <see cref="ConfigurationElement.Reset(ConfigurationElement)"/>.
		/// </summary>
		/// <param name="parentElement">The parent node of the configuration element.</param>
		public void HandleReset(ConfigurationElement parentElement)
		{
			T parentProviders = parentElement as T;
			if (parentProviders != null)
				parentProviders.Helper.UpdatePropertyCollection(); // before reseting make sure the bag is filled in

			helpedCustomProviderData.BaseReset(parentElement);
		}

		/// <summary>
		/// Sets the value to the specified attribute and updates the properties collection.
		/// </summary>
		/// <param name="key">The key of the attribute to set.</param>
		/// <param name="value">The value to set for the attribute.</param>
		public void HandleSetAttributeValue(string key, string value)
		{
			Attributes.Add(key, value);
			UpdatePropertyCollection();
		}

		/// <summary>
		/// Concrete implementation of <see cref="ConfigurationElement.Unmerge(ConfigurationElement, ConfigurationElement, ConfigurationSaveMode)"/>.
		/// </summary>
		/// <param name="sourceElement">A <see cref="ConfigurationElement"/> object at the current level containing a merged view of the properties.</param>
		/// <param name="parentElement">The parent <see cref="ConfigurationElement"/> object, or a <see langword="null"/> reference if this is the top level.</param>
		/// <param name="saveMode">A <see cref="ConfigurationSaveMode"/> object that determines which property values to include.</param>
		public void HandleUnmerge(ConfigurationElement sourceElement, ConfigurationElement parentElement, ConfigurationSaveMode saveMode)
		{
			T parentProviders = parentElement as T;
			if (parentProviders != null)
				parentProviders.Helper.UpdatePropertyCollection(); // before reseting make sure the bag is filled in

			T sourceProviders = sourceElement as T;
			if (sourceProviders != null)
				sourceProviders.Helper.UpdatePropertyCollection(); // before reseting make sure the bag is filled in

			helpedCustomProviderData.BaseUnmerge(sourceElement, parentElement, saveMode);

			UpdatePropertyCollection();
		}

		/// <summary>
		/// Gets the collection of custom attributes.
		/// </summary>
		public NameValueCollection Attributes
		{
			get
			{
				CreateAttributes();
				return (NameValueCollection)attributes;
			}
		}

		/// <summary>
		/// Gets a <see cref="ConfigurationPropertyCollection"/> of the properties that are defined for this configuration element when implemented in a derived class. 
		/// </summary>
		/// <value>
		/// A <see cref="ConfigurationPropertyCollection"/> of the properties that are defined for this configuration element when implemented in a derived class. 
		/// </value>
		public ConfigurationPropertyCollection Properties
		{
			get
			{
				UpdatePropertyCollection();
				return propertiesCollection;
			}
		}

		/// <summary>
		/// Returns whether the property name is known in advance, i.e. it is not a dynamic property.
		/// </summary>
		/// <param name="propertyName">The property name.</param>
		/// <returns><b>true</b> if the property is known in advance, <b>false</b> otherwise.</returns>
		protected internal virtual bool IsKnownPropertyName(string propertyName)
		{
            return ((NameTypeConfigurationElement)helpedCustomProviderData).MetadataProperties.Contains(propertyName);
		}

		private void AddAttributesFromConfigurationProperties()
		{
			foreach (ConfigurationProperty property in propertiesCollection)
			{
				if (!IsKnownPropertyName(property.Name))
				{
					attributes.Add(property.Name, (string)helpedCustomProviderData.BaseGetPropertyValue(property));
				}
			}
		}

		private ConfigurationProperty CreateProperty(string propertyName)
		{
			ConfigurationProperty property = new ConfigurationProperty(propertyName, typeof(string), null);
			propertiesCollection.Add(property);
			return property;
		}

		private void CreateAttributes()
		{
			if (attributes == null)
			{
				lock (lockObject)
				{
					if (attributes == null)
					{
						attributes = new NameValueCollection(StringComparer.Ordinal);
						AddAttributesFromConfigurationProperties();
					}
				}
			}
		}

		private bool CopyPropertiesToAttributes()
		{
			bool isModified = false;
			foreach (string key in attributes)
			{
                if (string.IsNullOrEmpty(key) || CheckForValidKey(key)) continue;

				string valueInCollection = attributes[key];
				string valueInBag = GetPropertyValue(key);

				if (valueInBag == null || valueInCollection != valueInBag)
				{
					SetPropertyValue(key, valueInCollection);
					isModified = true;
				}
			}
			return isModified;
		}

        private bool CheckForValidKey(string key)
        {
            return !AttributeKeyValidator.IsValid(key);
        }

		private void CreateRemoveList(List<string> removeList)
		{
			foreach (ConfigurationProperty property in propertiesCollection)
			{
				if (IsKnownPropertyName(property.Name)) continue;

				if (attributes.Get(property.Name) == null)
				{
					removeList.Add(property.Name);
				}
			}
		}

		private ConfigurationProperty GetProperty(string propertyName)
		{
			if (!propertiesCollection.Contains(propertyName)) return null;

			return propertiesCollection[propertyName];
		}

		private string GetPropertyValue(string propertyName)
		{
			ConfigurationProperty property = GetProperty(propertyName);
			if (property != null) return (string)helpedCustomProviderData.BaseGetPropertyValue(property);
			return string.Empty;
		}

		private bool RemoveDeletedConfigurationProperties()
		{
			List<string> removeList = new List<string>();

			CreateRemoveList(removeList);

			foreach (string propertyName in removeList)
			{
				propertiesCollection.Remove(propertyName);
			}
			return removeList.Count > 0;
		}

		private void SetPropertyValue(string propertyName, string value)
		{
			ConfigurationProperty property = GetProperty(propertyName);
			if (property == null)
			{
				property = CreateProperty(propertyName);
			}
			helpedCustomProviderData.BaseSetPropertyValue(property, value);
		}

		private bool UpdatePropertyCollection()
		{
			if (attributes == null) return false;

			// remove any data that has been delete from the collection
			bool isModified = RemoveDeletedConfigurationProperties();

			// then copy any data that has been changed in the collection
			isModified |= CopyPropertiesToAttributes();

			return isModified;
		}
	}

    internal static class AttributeKeyValidator
    {
        private static Regex expression;
        static AttributeKeyValidator()
        {
            string pattern = @"^[a-zA-Z_]\w*$";
            expression = new Regex(pattern);
        }

        public static bool IsValid(string key)
        {
            return expression.Match(key).Success;
        }
    }
}
