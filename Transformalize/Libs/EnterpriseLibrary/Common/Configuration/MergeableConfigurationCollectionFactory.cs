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
    ///<summary>
    /// Factory for creating <see cref="IMergeableConfigurationElementCollection"/>s.
    ///</summary>
    public static class MergeableConfigurationCollectionFactory
    {
        ///<summary>
        /// Creates a <see cref="IMergeableConfigurationElementCollection"/> based on a ConfigurationElementCollection type.
        ///</summary>
        ///<param name="collection"></param>
        ///<returns></returns>
        public static IMergeableConfigurationElementCollection GetCreateMergeableCollection(ConfigurationElementCollection collection)
        {
            if (collection is IMergeableConfigurationElementCollection)
            {
                return (IMergeableConfigurationElementCollection)collection;
            }
            else if (collection is ConnectionStringSettingsCollection)
            {
                return new MergeableConnectionStringSettingsCollection((ConnectionStringSettingsCollection)collection);
            }
            else if (collection is KeyValueConfigurationCollection)
            {
                return new MergeableKeyValueConfigurationCollection((KeyValueConfigurationCollection)collection);
            }

            var mergeableConfigurationCollectionAttribute = TypeDescriptor.GetAttributes(collection).OfType<MergeableConfigurationCollectionTypeAttribute>().FirstOrDefault();
            if (mergeableConfigurationCollectionAttribute != null)
            {
                return Activator.CreateInstance(mergeableConfigurationCollectionAttribute.MergeableConfigurationCollectionType, collection) as IMergeableConfigurationElementCollection;
            }

            return null;
        }

        private class MergeableConnectionStringSettingsCollection : IMergeableConfigurationElementCollection
        {
            ConnectionStringSettingsCollection connectionStringCollection;

            public MergeableConnectionStringSettingsCollection(ConnectionStringSettingsCollection connectionStringCollection)
            {
                this.connectionStringCollection = connectionStringCollection;
            }

            public void ResetCollection(IEnumerable<ConfigurationElement> configurationElements)
            {
                this.connectionStringCollection.Clear();
                foreach (ConnectionStringSettings connectionString in configurationElements.OfType<ConnectionStringSettings>())
                {
                    this.connectionStringCollection.Add(connectionString);
                }
            }

            public ConfigurationElement CreateNewElement(Type configurationType)
            {
                return new ConnectionStringSettings() { Name = "Connection String" + connectionStringCollection.Count };
            }
        }

        private class MergeableKeyValueConfigurationCollection : IMergeableConfigurationElementCollection
        {
            KeyValueConfigurationCollection keyValueCollection;

            public MergeableKeyValueConfigurationCollection(KeyValueConfigurationCollection keyValueCollection)
            {
                this.keyValueCollection = keyValueCollection;
            }

            public void ResetCollection(IEnumerable<ConfigurationElement> configurationElements)
            {
                foreach (string key in keyValueCollection.AllKeys.ToArray())
                {
                    keyValueCollection.Remove(key);
                }
                foreach (KeyValueConfigurationElement keyValueElement in configurationElements.OfType<KeyValueConfigurationElement>())
                {
                    keyValueCollection.Add(keyValueElement.Key, keyValueElement.Value);
                }
            }


            public ConfigurationElement CreateNewElement(Type configurationType)
            {
                return new KeyValueConfigurationElement("", "");
            }
        }
    }
}
