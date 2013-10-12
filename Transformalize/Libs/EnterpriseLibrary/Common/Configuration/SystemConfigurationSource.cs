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
using System.Security;
using Transformalize.Libs.EnterpriseLibrary.Common.Configuration.Storage;

namespace Transformalize.Libs.EnterpriseLibrary.Common.Configuration
{
    /// <summary>
    /// Represents an <see cref="IConfigurationSource"/> that retrieves the configuration information from the 
    /// application's default configuration file using the <see cref="ConfigurationManager"/> API.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The <see cref="SystemConfigurationSource"/> is a wrapper over the static configuration access API provided by 
    /// <see cref="ConfigurationManager"/> and watches for changes in the configuration files to refresh the 
    /// configuration when a change is detected.
    /// </para>
    /// </remarks>
    /// <seealso cref="ConfigurationManager"/>
    [ConfigurationElementType(typeof(SystemConfigurationSourceElement))]
    public class SystemConfigurationSource : FileBasedConfigurationSource
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SystemConfigurationSource"/> class.
        /// </summary>
        public SystemConfigurationSource()
            : this(true)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SystemConfigurationSource"/> class that will refresh changes
        /// according to the value of the <paramref name="refresh"/> parameter.
        /// </summary>
        /// <param name="refresh"><see langword="true"/> if changes to the configuration file should be notified.</param>
        public SystemConfigurationSource(bool refresh)
            : this(refresh, ConfigurationChangeWatcher.defaultPollDelayInMilliseconds)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SystemConfigurationSource"/> class that will refresh changes
        /// according to the value of the <paramref name="refresh"/> parameter, polling every 
        /// <paramref name="refreshInterval"/> milliseconds.
        /// </summary>
        /// <param name="refresh"><see langword="true"/> if changes to the configuration file should be notified.</param>
        /// <param name="refreshInterval">The poll interval in milliseconds.</param>
        public SystemConfigurationSource(bool refresh, int refreshInterval)
            : base(SafeGetCurrentConfigurationFile(), refresh, refreshInterval)
        { }

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

            var fileMap = new ExeConfigurationFileMap { ExeConfigFilename = base.ConfigurationFilePath };
            var config = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);

            config.Sections.Remove(sectionName);
            config.Save();
            ConfigurationManager.RefreshSection(sectionName);
        }

        /// <summary>
        /// This method supports the Enterprise Library infrastructure and is not intended to be used directly from your code.
        /// Adds or replaces <paramref name="configurationSection"/> under name <paramref name="section"/> in the configuration 
        /// and saves the configuration file.
        /// </summary>
        /// <param name="section">The name for the section.</param>
        /// <param name="configurationSection">The configuration section to add or replace.</param>
        public void Save(string section, ConfigurationSection configurationSection)
        {
            ValidateArgumentsAndFileExists(ConfigurationFilePath, section, configurationSection);

            var fileMap = new ExeConfigurationFileMap { ExeConfigFilename = ConfigurationFilePath };
            var config = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);

            config.Sections.Remove(section);
            config.Sections.Add(section, configurationSection);

            config.Save();
            ConfigurationManager.RefreshSection(section);
        }

        /// <summary>
        /// Retrieves the specified <see cref="ConfigurationSection"/> from the configuration file.
        /// </summary>
        /// <param name="sectionName">The section name.</param>
        /// <returns>The section, or <see langword="null"/> if it doesn't exist.</returns>
        protected override ConfigurationSection DoGetSection(string sectionName)
        {
            return ConfigurationManager.GetSection(sectionName) as ConfigurationSection;
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
            sectionsToNotify = new List<string>();
            sectionsWithChangedConfigSource = new Dictionary<string, string>();

            // refresh local sections and determine what to do.
            foreach (KeyValuePair<string, string> sectionMapping in localSectionsToRefresh)
            {
                RefreshSingleSection(sectionMapping.Key);

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
                RefreshSingleSection(sectionMapping.Key);

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
            foreach (string sectionName in sectionsToRefresh)
            {
                RefreshSingleSection(sectionName);
            }
        }

        private static void RefreshSingleSection(string sectionName)
        {
            ConfigurationManager.RefreshSection(sectionName);
        }

        private static string SafeGetCurrentConfigurationFile()
        {
            try
            {
                return AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
            }
            catch (SecurityException)
            {
                return null;
            }
        }
    }
}
