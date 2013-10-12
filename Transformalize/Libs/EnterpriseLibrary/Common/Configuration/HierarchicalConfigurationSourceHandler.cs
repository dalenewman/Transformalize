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
using System.Linq;
using System.Configuration;
using System.Globalization;

namespace Transformalize.Libs.EnterpriseLibrary.Common.Configuration
{
    /// <summary>
    /// Implements hierarchical merge behavior to <see cref="IConfigurationSource"/> implementations.<br/>
    /// </summary>
    /// <seealso cref="IConfigurationSource"/>
    /// <seealso cref="ConfigurationSourceHandler"/>
    public class HierarchicalConfigurationSourceHandler : ConfigurationSourceHandler
    {
        private const string CustomParentSourceName = "__CustomParentSource__";

        IConfigurationSource localSource;
        string parentSourceName;

        /// <summary>
        /// Creates a new instance of <see cref="HierarchicalConfigurationSourceHandler"/>.
        /// </summary>
        /// <param name="localSource">The <see cref="IConfigurationSource"/> instance that should be extended.</param>
        public HierarchicalConfigurationSourceHandler(IConfigurationSource localSource)
            :base(localSource)
        {
            if (localSource == null) throw new ArgumentNullException("localSource");

            this.localSource = localSource;

        }

        /// <summary>
        /// Creates a new instance of <see cref="HierarchicalConfigurationSourceHandler"/>.
        /// </summary>
        /// <param name="localSource">The <see cref="IConfigurationSource"/> instance that should be extended.</param>
        /// <param name="parentSource">An <see cref="IConfigurationSource"/> instance the <paramref name="localSource"/> should be merged with.</param>
        public HierarchicalConfigurationSourceHandler(IConfigurationSource localSource, IConfigurationSource parentSource)
            : base(localSource)
        {
            if (localSource == null) throw new ArgumentNullException("localSource");

            this.localSource = localSource;
            this.parentSourceName = CustomParentSourceName;

            AddCustomSubordinateSource(parentSourceName, parentSource);
        }


        /// <summary>
        /// Performs intialization logic for this <see cref="HierarchicalConfigurationSourceHandler"/>.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            if (parentSourceName != CustomParentSourceName)
            {
                this.parentSourceName = GetParentConfigurationSourceName(localSource);
            }
        }

        /// <summary>
        /// Performs re-intialization logic for this <see cref="HierarchicalConfigurationSourceHandler"/>.
        /// </summary>

        protected override void DoRefresh()
        {
            base.DoRefresh();
            if (parentSourceName != CustomParentSourceName)
            {
                parentSourceName = GetParentConfigurationSourceName(localSource);
            }
        }

        /// <summary>
        /// Checks whether the result of a call to <see cref="IConfigurationSource.GetSection(string)"/> should be merged.<br/>
        /// If the call should be merged, performs the merge behavior and returns the resulting <see cref="ConfigurationSection"/> intance.<br/>
        /// If the call should not be merged returns <paramref name="configurationSection"/>.
        /// </summary>
        /// <param name="sectionName">The name of the section that was retrieved from configuration.</param>
        /// <param name="configurationSection">The section that was retrieved from configuration.</param>
        /// <returns>The resulting <see cref="ConfigurationSection"/> instance.</returns>
        /// <seealso cref="IConfigurationSource.GetSection(string)"/>
        protected override ConfigurationSection DoCheckGetSection(string sectionName, ConfigurationSection configurationSection)
        {
            if (string.IsNullOrEmpty(parentSourceName))
            {
                return configurationSection;
            }

            IConfigurationSource parentSource = GetSubordinateSource(parentSourceName);
            if (parentSource == null)
            {
                return configurationSection;
            }

            ConfigurationSection parentSection = parentSource.GetSection(sectionName);

            if (configurationSection == null)
            {
                return parentSection;
            }

            if (parentSection == null)
            {
                return configurationSection;
            }

            ConfigurationSection localSection = CloneSection(configurationSection);
            ConfigurationSectionMerge configurationMerge = new ConfigurationSectionMerge(parentSection, localSection);

            ConfigurationSection mergedSection = configurationMerge.GetMergedSection();
            EnsurePropagatingSectionChangeEvents(parentSourceName, sectionName);

            return mergedSection;
            
        }

        private static ConfigurationSection CloneSection(ConfigurationSection sourceSection)
        {
            var cloner = new ConfigurationSectionCloner();
            return cloner.Clone(sourceSection);
        }

        private static string GetParentConfigurationSourceName(IConfigurationSource source)
        {
            ConfigurationSourceSection configurationSourcesSection = source.GetSection(ConfigurationSourceSection.SectionName) as ConfigurationSourceSection;
            if (configurationSourcesSection != null && !string.IsNullOrEmpty(configurationSourcesSection.ParentSource))
            {
                return configurationSourcesSection.ParentSource;
            }

            return null;
        }

        private class ConfigurationSectionMerge
        {
            ConfigurationSection parentSection;
            ConfigurationSection localSection;

            public ConfigurationSectionMerge(ConfigurationSection parentSection, ConfigurationSection localSection)
            {
                if (parentSection.GetType() != localSection.GetType())
                {
                    string message = String.Format(CultureInfo.CurrentCulture,
                        Resources.ExceptionIncompaitbleMergeElementType,
                        localSection.GetType(),
                        parentSection.GetType());


                    throw new ConfigurationSourceErrorsException(message);
                }

                this.parentSection = parentSection;
                this.localSection = localSection;

            }

            public ConfigurationSection GetMergedSection()
            {
                ConfigurationElementMerge elementMerge = new ConfigurationElementMerge(parentSection, localSection);

                return elementMerge.GetMergedElement() as ConfigurationSection;
            }
        }

        private class ConfigurationElementMerge
        {
            readonly ConfigurationElement parentElement;
            readonly ConfigurationElement localElement;

            public ConfigurationElementMerge(ConfigurationElement parentElement, ConfigurationElement localElement)
            {
                if (parentElement.GetType() != localElement.GetType())
                {
                    string message = String.Format(CultureInfo.CurrentCulture,
                        Resources.ExceptionIncompaitbleMergeElementType,
                        localElement.GetType(),
                        parentElement.GetType());


                    throw new ConfigurationSourceErrorsException(message);
                }

                this.parentElement = parentElement;
                this.localElement = localElement;
            }

            public ConfigurationElement GetMergedElement()
            {
                Dictionary<string, PropertyInformation> localProperties = localElement.ElementInformation.Properties.OfType<PropertyInformation>().ToDictionary(x => x.Name);
                Dictionary<string, PropertyInformation> parentProperties = parentElement.ElementInformation.Properties.OfType<PropertyInformation>().ToDictionary(x => x.Name);
                
                foreach (string propertyName in localProperties.Keys)
                {
                    PropertyInformation localProperty, parentProperty;
                    localProperty = localProperties[propertyName];
                    parentProperty = parentProperties[propertyName];

                    if (typeof(ConfigurationElement).IsAssignableFrom(localProperty.Type))
                    {
                        //either a collection or regular element

                        if (localProperty.Value == null)
                        {
                            localProperty.Value = parentProperty.Value;
                            continue;
                        }

                        if (parentProperty.Value != null)
                        {

                            if (typeof(ConfigurationElementCollection).IsAssignableFrom(localProperty.Type))
                            {
                                ConfigurationElementCollectionMerge merge = new ConfigurationElementCollectionMerge((ConfigurationElementCollection)parentProperty.Value, (ConfigurationElementCollection)localProperty.Value);
                                localProperty.Value = merge.GetMergedElementCollection();
                            }
                            else
                            {
                                ConfigurationElementMerge merge = new ConfigurationElementMerge((ConfigurationElement)parentProperty.Value, (ConfigurationElement)localProperty.Value);
                                localProperty.Value = merge.GetMergedElement();
                            }
                        }
                    }
                    else
                    {   
                        //just a regular property

                        if (ShouldOverrideLocalProperty(localProperty, parentProperty))
                        {
                            localProperty.Value = parentProperty.Value;
                        }
                    }
                }


                return localElement;
            }

            private static bool ShouldOverrideLocalProperty(PropertyInformation localProperty, PropertyInformation parentProperty)
            {
                return (localProperty.ValueOrigin == PropertyValueOrigin.Default);
            }
        }

        private class ConfigurationElementCollectionMerge
        {
            readonly ConfigurationElementCollection parentCollection;
            readonly ConfigurationElementCollection localCollection;
            readonly IMergeableConfigurationElementCollection mergeableLocalElement;
            List<ConfigurationElement> allElements = new List<ConfigurationElement>();

            public ConfigurationElementCollectionMerge(ConfigurationElementCollection parentCollection, ConfigurationElementCollection localCollection)
            {
                this.parentCollection = parentCollection;
                
                ConfigurationElementMerge elementMerge = new ConfigurationElementMerge(parentCollection, localCollection);
                this.localCollection = (ConfigurationElementCollection) elementMerge.GetMergedElement();

                this.mergeableLocalElement = MergeableConfigurationCollectionFactory.GetCreateMergeableCollection(localCollection);

                //TODO : what to do around: localCollection.CollectionType
            }

            public ConfigurationElementCollection GetMergedElementCollection()
            {
                if (mergeableLocalElement == null) return localCollection;
                if (localCollection.EmitClear) return localCollection;

                List<ConfigurationElement> elementsFromParent = parentCollection.Cast<ConfigurationElement>().ToList();

                foreach (ConfigurationElement elementFromLocal in localCollection.Cast<ConfigurationElement>())
                {
                    var keyPredicate = CreateMatchKeyPredicate(elementFromLocal);
                    ConfigurationElement matchingElement = elementsFromParent.Where(keyPredicate).FirstOrDefault();

                    if (matchingElement != null)
                    {
                        ConfigurationElementMerge elementMerge = new ConfigurationElementMerge(matchingElement, elementFromLocal);
                        ConfigurationElement mergedElement = elementMerge.GetMergedElement();
                        int index = elementsFromParent.IndexOf(matchingElement);
                        elementsFromParent.RemoveAt(index);
                        elementsFromParent.Insert(index, mergedElement);
                    }
                    else
                    {
                        elementsFromParent.Add(elementFromLocal);
                    }
                }

                mergeableLocalElement.ResetCollection(elementsFromParent);
                return localCollection;
            }

            private Func<ConfigurationElement, bool> CreateMatchKeyPredicate(ConfigurationElement other)
            {
                string[] keyPropertyNames = other.ElementInformation.Properties.Cast<PropertyInformation>().Where(x => x.IsKey).Select(x => x.Name).ToArray();

                return x =>
                    {
                        int keyCount = other.ElementInformation.Properties.Keys.Count;
                        foreach(string keyProperty in keyPropertyNames)
                        {
                            if (!object.Equals(other.ElementInformation.Properties[keyProperty].Value, x.ElementInformation.Properties[keyProperty].Value))
                            {
                                return false;
                            }
                        }
                        return true;
                    };
            }
        }
    }
}
