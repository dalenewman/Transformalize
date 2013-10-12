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
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ComponentModel;
using Transformalize.Libs.EnterpriseLibrary.Common.Configuration.Design;

namespace Transformalize.Libs.EnterpriseLibrary.Common.Configuration
{
    /// <summary>
    /// Clones a <see cref="ConfigurationSection"/>.
    /// </summary>
    /// <remarks>
    /// This will perform a clone of a <see cref="ConfigurationSection"/> by evaluating each element in
    /// the <see cref="ElementInformation.Properties"/>.  If these properties are themselves <see cref="ConfigurationElement"/> they will 
    /// be cloned as well.
    /// 
    /// As <see cref="ConfigurationElementCollection"/> items do not expose the ability to add and remove, any configuration collections
    /// must implement <see cref="IMergeableConfigurationElementCollection"/> to be cloned.  If they do not implement this, they will be skipped
    /// during cloning. The enterprise library collections implement this interface and the cloner will properly handle the cloning 
    /// of <see cref="ConnectionStringSettingsCollection"/> and <see cref="KeyValueConfigurationCollection"/> with an internal wrapper that 
    /// implements <see cref="IMergeableConfigurationElementCollection"/> for these collections.
    /// </remarks>
    public class ConfigurationSectionCloner
    {
        ///<summary>
        /// Clones a <see cref="ConfigurationSection"/>
        ///</summary>
        ///<param name="section">The <see cref="ConfigurationSection"/> to clone.</param>
        ///<returns>A new, cloned <see cref="ConfigurationSection"/>.</returns>
        public ConfigurationSection Clone(ConfigurationSection section)
        {
            if (section == null) throw new ArgumentNullException("section");

            var clonedSection = (ConfigurationSection)Activator.CreateInstance(section.GetType());
            return (ConfigurationSection)CloneElement(section, clonedSection);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceElement"></param>
        /// <param name="targetElement"></param>
        /// <returns></returns>
        public static ConfigurationElement CloneElement(ConfigurationElement sourceElement, ConfigurationElement targetElement)
        {
            if (sourceElement is ICustomProviderData)
            {
                var targetAttributes = ((ICustomProviderData)targetElement).Attributes;
                var sourceAttributes = ((ICustomProviderData)sourceElement).Attributes;
                foreach (string key in sourceAttributes)
                {
                    targetAttributes.Add(key, sourceAttributes[key]);
                }
            }

            foreach (PropertyInformation property in sourceElement.ElementInformation.Properties)
            {
                
                if (property.ValueOrigin == PropertyValueOrigin.Default) continue;
                try
                {
                    if (property.Value == null) continue;
                }
                catch
                {
                    continue;
                }
                PropertyInformation targetProperty = targetElement.ElementInformation.Properties[property.Name];

                if (typeof(ConfigurationElement).IsAssignableFrom(property.Type) && typeof(ICloneableConfigurationElement).IsAssignableFrom(property.Type))
                {
                    targetProperty.Value = ((ICloneableConfigurationElement)property.Value).CreateFullClone();
                }
                if (typeof(ConfigurationElement).IsAssignableFrom(property.Type) && TypeDescriptor.GetAttributes(property.Type).OfType<CloneableConfigurationElementTypeAttribute>().Any())
                {
                    CloneableConfigurationElementTypeAttribute cloneableConfigurationElementTypeAttribute = TypeDescriptor.GetAttributes(property.Type).OfType<CloneableConfigurationElementTypeAttribute>().First();
                    ICloneableConfigurationElement cloneable = (ICloneableConfigurationElement)Activator.CreateInstance(cloneableConfigurationElementTypeAttribute.CloneableConfigurationElementType, property.Value);

                    targetProperty.Value = cloneable.CreateFullClone();
                }
                if (typeof(ConfigurationElementCollection).IsAssignableFrom(property.Type))
                {
                    ConfigurationElementCollection sourceCollection = (ConfigurationElementCollection)property.Value;
                    ConfigurationElementCollection targetCollection = (ConfigurationElementCollection)Activator.CreateInstance(sourceCollection.GetType());
                    targetCollection = CloneCollection(sourceCollection, targetCollection);

                    targetProperty.Value = targetCollection;
                }
                else if (typeof(ConfigurationElement).IsAssignableFrom(property.Type))
                {
                    ConfigurationElement sourceChildElement = (ConfigurationElement)property.Value;
                    ConfigurationElement targetChildElement = (ConfigurationElement)Activator.CreateInstance(sourceChildElement.GetType());

                    targetChildElement = CloneElement(sourceChildElement, targetChildElement);
                    targetProperty.Value = targetChildElement;
                }
                else
                {
                    targetProperty.Value = property.Value;
                }
            }

            return targetElement;
        }

        private static ConfigurationElementCollection CloneCollection(ConfigurationElementCollection sourceCollection, ConfigurationElementCollection targetCollection)
        {
            targetCollection = (ConfigurationElementCollection)CloneElement(sourceCollection, targetCollection);
            
            IMergeableConfigurationElementCollection mergeableSource = MergeableConfigurationCollectionFactory.GetCreateMergeableCollection(sourceCollection);
            IMergeableConfigurationElementCollection mergeableTarget = MergeableConfigurationCollectionFactory.GetCreateMergeableCollection(targetCollection);

            if (mergeableSource == null) return targetCollection;

            List<ConfigurationElement> targetCollectionContents = new List<ConfigurationElement>();

            foreach (ConfigurationElement sourceElement in sourceCollection)
            {
                ConfigurationElement targetElement;
                targetElement = CreateCopyOfCollectionElement(mergeableSource, sourceElement);
                targetElement = CloneElement(sourceElement, targetElement);

                targetCollectionContents.Add(targetElement);
            }

            mergeableTarget.ResetCollection(targetCollectionContents);

            targetCollection.EmitClear = sourceCollection.EmitClear;
            return targetCollection;
        }

        private static ConfigurationElement CreateCopyOfCollectionElement(IMergeableConfigurationElementCollection mergeableSource, ConfigurationElement sourceElement)
        {
            ConfigurationElement targetElement;
            Type sourceType = sourceElement.GetType();

            if (sourceElement is ICloneableConfigurationElement)
            {
                return ((ICloneableConfigurationElement)sourceElement).CreateFullClone();
            }
            if (TypeDescriptor.GetAttributes(sourceElement).OfType<CloneableConfigurationElementTypeAttribute>().Any())
            {
                CloneableConfigurationElementTypeAttribute cloneableConfigurationElementTypeAttribute = TypeDescriptor.GetAttributes(sourceElement).OfType<CloneableConfigurationElementTypeAttribute>().First();
                ICloneableConfigurationElement cloneable = (ICloneableConfigurationElement)Activator.CreateInstance(cloneableConfigurationElementTypeAttribute.CloneableConfigurationElementType, sourceElement);

                return cloneable.CreateFullClone();
            }

            targetElement = mergeableSource.CreateNewElement(sourceType);
            
            return targetElement;
        }
    }
}
