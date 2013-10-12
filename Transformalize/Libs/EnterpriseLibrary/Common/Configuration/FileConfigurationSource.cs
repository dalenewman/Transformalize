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
using System.Globalization;
using System.IO;
using Transformalize.Libs.EnterpriseLibrary.Common.Configuration.Storage;

namespace Transformalize.Libs.EnterpriseLibrary.Common.Configuration
{
    /// <summary>
    /// Represents a configuration source that retrieves configuration information from an arbitrary file.
    /// </summary>
    /// <remarks>
    /// This configuration source uses a <see cref="System.Configuration.Configuration"/> object to deserialize 
    /// configuration, so the configuration file must be a valid .NET Framework configuration file.
    /// </remarks>
    [ConfigurationElementType(typeof(FileConfigurationSourceElement))]
    public class FileConfigurationSource : FileBasedConfigurationSource, IProtectedConfigurationSource
    {
        private readonly ExeConfigurationFileMap fileMap;
        private readonly object cachedConfigurationLock;
        private System.Configuration.Configuration cachedConfiguration;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileConfigurationSource"/> class.
        /// </summary>
        /// <param name="configurationFilepath">The configuration file path. The path can be absolute or relative.</param>
        public FileConfigurationSource(string configurationFilepath)
            : this(configurationFilepath, true)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileConfigurationSource"/> class that will refresh changes
        /// according to the value of the <paramref name="refresh"/> parameter.
        /// </summary>
        /// <param name="configurationFilepath">The configuration file path. The path can be absolute or relative.</param>
        /// <param name="refresh"><see langword="true"/> if changes to the configuration file should be notified.</param>
        public FileConfigurationSource(string configurationFilepath, bool refresh)
            : this(configurationFilepath, refresh, ConfigurationChangeWatcher.defaultPollDelayInMilliseconds)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileConfigurationSource"/> that will refresh changes
        /// according to the value of the <paramref name="refresh"/> parameter, polling every 
        /// <paramref name="refreshInterval"/> milliseconds.
        /// </summary>
        /// <param name="configurationFilepath">The configuration file path. The path can be absolute or relative.</param>
        /// <param name="refresh"><see langword="true"/> if changes to the configuration file should be notified.</param>
        /// <param name="refreshInterval">The poll interval in milliseconds.</param>
        public FileConfigurationSource(string configurationFilepath, bool refresh, int refreshInterval)
            : base(GetRootedCurrentConfigurationFile(configurationFilepath), refresh, refreshInterval)
        {
            this.cachedConfigurationLock = new object();
            this.fileMap = new ExeConfigurationFileMap() { ExeConfigFilename = this.ConfigurationFilePath };
        }


        /// <summary>
        /// Adds a <see cref="ConfigurationSection"/> to the configuration and saves the configuration source.
        /// </summary>
        /// <remarks>
        /// If a configuration section with the specified name already exists it will be replaced.
        /// </remarks>
        /// <param name="sectionName">The name by which the <paramref name="configurationSection"/> should be added.</param>
        /// <param name="configurationSection">The configuration section to add.</param>
        public override void DoAdd(string sectionName, ConfigurationSection configurationSection)
        {
            Save(sectionName, configurationSection);
        }

        /// <summary>
        /// Removes a <see cref="ConfigurationSection"/> from the configuration and saves the configuration source.
        /// </summary>
        /// <param name="sectionName">The name of the section to remove.</param>
        public override void DoRemove(string sectionName)
        {
            if (string.IsNullOrEmpty(sectionName)) throw new ArgumentException(Resources.ExceptionStringNullOrEmpty, "sectionName");

            var fileMap = new ExeConfigurationFileMap() { ExeConfigFilename = ConfigurationFilePath};
            var config = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);

            if (config.Sections.Get(sectionName) != null)
            {
                config.Sections.Remove(sectionName);
                config.Save();

                UpdateCache(true);
            }
        }

        /// <summary>
        /// Adds a <see cref="ConfigurationSection"/> to the configuration and saves the configuration source using encryption.
        /// </summary>
        /// <remarks>
        /// If a configuration section with the specified name already exists it will be replaced.<br/>
        /// If a configuration section was retrieved from an instance of <see cref="FileBasedConfigurationSource"/>, a <see cref="System.InvalidOperationException"/> will be thrown.
        /// </remarks>
        /// <param name="sectionName">The name by which the <paramref name="configurationSection"/> should be added.</param>
        /// <param name="configurationSection">The configuration section to add.</param>
        /// <param name="protectionProviderName">The name of the protection provider to use when encrypting the section.</param>
        /// <exception cref="System.InvalidOperationException">The configuration section was retrieved from an instance of  <see cref="FileBasedConfigurationSource"/> or <see cref="Configuration"/> and cannot be added to the current source.</exception>
        public void Add(
            string sectionName,
            ConfigurationSection configurationSection,
            string protectionProviderName)
        {
            Save(sectionName, configurationSection, protectionProviderName);
        }

        /// <summary>
        /// This method supports the Enterprise Library infrastructure and is not intended to be used directly from your code.
        /// Adds or replaces <paramref name="configurationSection"/> under name <paramref name="section"/> in the configuration and saves the configuration file.
        /// </summary>
        /// <param name="section">The name for the section.</param>
        /// <param name="configurationSection">The configuration section to add or replace.</param>
        public void Save(string section, ConfigurationSection configurationSection)
        {
            ValidateArgumentsAndFileExists(ConfigurationFilePath, section, configurationSection);

            InternalSave(ConfigurationFilePath, section, configurationSection, string.Empty);
        }

        /// <summary>
        /// This method supports the Enterprise Library infrastructure and is not intended to be used directly from your code.
        /// Adds or replaces <paramref name="configurationSection"/> under name <paramref name="section"/> in the configuration 
        /// file and saves the configuration file using encryption.
        /// </summary>
        /// <param name="section">The name for the section.</param>
        /// <param name="configurationSection">The configuration section to add or replace.</param>
        /// <param name="protectionProvider">The name of the protection provider to use when encrypting the section.</param>
        public void Save(string section, ConfigurationSection configurationSection, string protectionProvider)
        {
            ValidateArgumentsAndFileExists(ConfigurationFilePath, section, configurationSection);
            if (string.IsNullOrEmpty(protectionProvider)) throw new ArgumentException(Resources.ExceptionStringNullOrEmpty, "protectionProvider");

            InternalSave(ConfigurationFilePath, section, configurationSection, protectionProvider);
        }

        /// <summary>
        /// Retrieves the specified <see cref="ConfigurationSection"/> from the configuration file.
        /// </summary>
        /// <param name="sectionName">The section name.</param>
        /// <returns>The section, or <see langword="null"/> if it doesn't exist.</returns>
        protected override ConfigurationSection DoGetSection(string sectionName)
        {
            System.Configuration.Configuration configuration = GetConfiguration();

            return configuration.GetSection(sectionName) as ConfigurationSection;
        }


        /// <summary>
        /// Refreshes the configuration sections from the main configuration file and determines which sections have 
        /// suffered notifications and should be notified to registered handlers.
        /// </summary>
        /// <param name="localSectionsToRefresh">A dictionary with the configuration sections residing in the main 
        /// configuration file that must be refreshed.</param>
        /// <param name="externalSectionsToRefresh">A dictionary with the configuration sections residing in external 
        /// files that must be refreshed.</param>
        /// <param name="sectionsToNotify">A new collection with the names of the sections that suffered changes and 
        /// should be notified.</param>
        /// <param name="sectionsWithChangedConfigSource">A new dictionary with the names and file names of the sections 
        /// that have changed their location.</param>
        protected override void RefreshAndValidateSections(
            IDictionary<string, string> localSectionsToRefresh,
            IDictionary<string, string> externalSectionsToRefresh,
            out ICollection<string> sectionsToNotify,
            out IDictionary<string, string> sectionsWithChangedConfigSource)
        {
            UpdateCache(true);

            sectionsToNotify = new List<string>();
            sectionsWithChangedConfigSource = new Dictionary<string, string>();

            // refresh local sections and determine what to do.
            foreach (KeyValuePair<string, string> sectionMapping in localSectionsToRefresh)
            {
                ConfigurationSection section = DoGetSection(sectionMapping.Key);
                string refreshedConfigSource = section != null ? section.SectionInformation.ConfigSource : NullConfigSource;
                if (!sectionMapping.Value.Equals(refreshedConfigSource))
                {
                    sectionsWithChangedConfigSource.Add(sectionMapping.Key, refreshedConfigSource);
                }

                // notify anyway, since it might have been updated.
                sectionsToNotify.Add(sectionMapping.Key);
            }

            // refresh external sections and determine what to do.
            foreach (KeyValuePair<string, string> sectionMapping in externalSectionsToRefresh)
            {
                ConfigurationSection section = DoGetSection(sectionMapping.Key);
                string refreshedConfigSource = section != null ? section.SectionInformation.ConfigSource : NullConfigSource;
                if (!sectionMapping.Value.Equals(refreshedConfigSource))
                {
                    sectionsWithChangedConfigSource.Add(sectionMapping.Key, refreshedConfigSource);

                    // notify only if che config source changed
                    sectionsToNotify.Add(sectionMapping.Key);
                }
            }
        }

        /// <summary>
        /// Refreshes the configuration sections from an external configuration file.
        /// </summary>
        /// <param name="sectionsToRefresh">A collection with the names of the sections that suffered changes and should 
        /// be refreshed.</param>
        protected override void RefreshExternalSections(IEnumerable<string> sectionsToRefresh)
        {
            UpdateCache(true);
        }

        private void InternalSave(string fileName, string section, ConfigurationSection configurationSection, string protectionProvider)
        {
            var fileMap = new ExeConfigurationFileMap { ExeConfigFilename = fileName };
            var config = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);

            if (typeof(ConnectionStringsSection) == configurationSection.GetType())
            {
                config.Sections.Remove(section);
                UpdateConnectionStrings(section, configurationSection, config, protectionProvider);
            }
            else if (typeof(AppSettingsSection) == configurationSection.GetType())
            {
                UpdateApplicationSettings(section, configurationSection, config, protectionProvider);
            }
            else
            {
                config.Sections.Remove(section);
                config.Sections.Add(section, configurationSection);
                ProtectConfigurationSection(configurationSection, protectionProvider);
            }

            config.Save();

            UpdateCache(true);
        }

        private static void ProtectConfigurationSection(ConfigurationSection configurationSection, string protectionProvider)
        {
            if (!string.IsNullOrEmpty(protectionProvider))
            {
                if (configurationSection.SectionInformation.ProtectionProvider == null
                    || configurationSection.SectionInformation.ProtectionProvider.Name != protectionProvider)
                {
                    configurationSection.SectionInformation.ProtectSection(protectionProvider);
                }
            }
            else
            {
                if (configurationSection.SectionInformation.ProtectionProvider != null)
                {
                    configurationSection.SectionInformation.UnprotectSection();
                }
            }
        }

        private void UpdateApplicationSettings(
            string section,
            ConfigurationSection configurationSection,
            System.Configuration.Configuration config,
            string protectionProvider)
        {
            AppSettingsSection current = config.AppSettings;
            if (current == null)
            {
                config.Sections.Add(section, configurationSection);
                ProtectConfigurationSection(configurationSection, protectionProvider);
            }
            else
            {
                AppSettingsSection newApplicationSettings = configurationSection as AppSettingsSection;
                if (current.File != newApplicationSettings.File)
                {
                    current.File = newApplicationSettings.File;
                }

                List<string> newKeys = new List<string>(newApplicationSettings.Settings.AllKeys);
                List<string> currentKeys = new List<string>(current.Settings.AllKeys);

                foreach (string keyInCurrent in currentKeys)
                {
                    if (!newKeys.Contains(keyInCurrent))
                    {
                        current.Settings.Remove(keyInCurrent);
                    }
                }

                foreach (string newKey in newKeys)
                {
                    if (!currentKeys.Contains(newKey))
                    {
                        current.Settings.Add(newKey, newApplicationSettings.Settings[newKey].Value);
                    }
                    else
                    {
                        if (current.Settings[newKey].Value != newApplicationSettings.Settings[newKey].Value)
                        {
                            current.Settings[newKey].Value = newApplicationSettings.Settings[newKey].Value;
                        }
                    }
                }
                ProtectConfigurationSection(current, protectionProvider);
            }

        }

        private void UpdateConnectionStrings(
            string section,
            ConfigurationSection configurationSection,
            System.Configuration.Configuration config,
            string protectionProvider)
        {
            ConnectionStringsSection current = config.ConnectionStrings;
            if (current == null)
            {
                config.Sections.Add(section, configurationSection);
                ProtectConfigurationSection(configurationSection, protectionProvider);
            }
            else
            {
                ConnectionStringsSection newConnectionStrings = (ConnectionStringsSection)configurationSection;
                foreach (ConnectionStringSettings connectionString in newConnectionStrings.ConnectionStrings)
                {
                    if (current.ConnectionStrings[connectionString.Name] == null)
                    {
                        current.ConnectionStrings.Add(connectionString);
                    }
                }
                ProtectConfigurationSection(current, protectionProvider);
            }
        }

        private static string GetRootedCurrentConfigurationFile(string configurationFile)
        {
            if (string.IsNullOrEmpty(configurationFile))
                throw new ArgumentException(Resources.ExceptionStringNullOrEmpty, "configurationFile");

            if (!Path.IsPathRooted(configurationFile))
            {
                configurationFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, configurationFile);
            }

            if (!File.Exists(configurationFile))
            {
                throw new FileNotFoundException(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        Resources.ExceptionConfigurationLoadFileNotFound,
                        configurationFile));
            }

            return configurationFile;
        }

        private System.Configuration.Configuration GetConfiguration()
        {
            if (cachedConfiguration == null)
            {
                UpdateCache(false);
            }

            return cachedConfiguration;
        }

        internal void UpdateCache(bool forceUpdate)
        {
            lock (cachedConfigurationLock)
            {
                if (forceUpdate || cachedConfiguration == null)
                {
                    System.Configuration.Configuration newConfiguration
                        = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);

                    cachedConfiguration = newConfiguration;
                }
            }
        }
    }
}
