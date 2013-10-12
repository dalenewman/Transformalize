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
using System.IO;

namespace Transformalize.Libs.EnterpriseLibrary.Common.Configuration.Storage
{
    /// <summary>
    /// <para>Represents an <see cref="IConfigurationChangeWatcher"/> that watches a file.</para>
    /// </summary>
    public class ConfigurationChangeFileWatcher : ConfigurationChangeWatcher
    {
        private const string eventSourceName = "Enterprise Library Configuration";
        private string configurationSectionName;
        private string configFilePath;

        /// <summary>
        /// <para>Initialize a new <see cref="ConfigurationChangeFileWatcher"/> class with the path to the configuration file and the name of the section</para>
        /// </summary>
        /// <param name="configFilePath">
        /// <para>The full path to the configuration file.</para>
        /// </param>
        /// <param name="configurationSectionName">
        /// <para>The name of the configuration section to watch.</para>
        /// </param>
        public ConfigurationChangeFileWatcher(string configFilePath, string configurationSectionName)
        {
            if (string.IsNullOrEmpty(configFilePath)) throw new ArgumentException(Resources.ExceptionStringNullOrEmpty, "configFilePath");
            if (null == configurationSectionName) throw new ArgumentNullException("configurationSectionName");

            this.configurationSectionName = configurationSectionName;
            this.configFilePath = configFilePath;
        }

        /// <summary>
        /// <para>Gets the name of the configuration section being watched.</para>
        /// </summary>
        /// <value>
        /// <para>The name of the configuration section being watched.</para>
        /// </value>
        public override string SectionName
        {
            get { return configurationSectionName; }
        }

        /// <summary>
        /// <para>Returns the <see cref="DateTime"/> of the last change of the information watched</para>
        /// <para>The information is retrieved using the watched file modification timestamp</para>
        /// </summary>
        /// <returns>The <see cref="DateTime"/> of the last modificaiton, or <code>DateTime.MinValue</code> if the information can't be retrieved</returns>
        protected override DateTime GetCurrentLastWriteTime()
        {
            if (File.Exists(configFilePath) == true)
            {
                return File.GetLastWriteTime(configFilePath);
            }
            else
            {
                return DateTime.MinValue;
            }
        }

        /// <summary>
        /// Builds the change event data, including the full path of the watched file
        /// </summary>
        /// <returns>The change event information</returns>
        protected override ConfigurationChangedEventArgs BuildEventData()
        {
            return new ConfigurationFileChangedEventArgs(Path.GetFullPath(configFilePath), configurationSectionName);
        }

        /// <summary>
        /// Returns the source name to use when logging events
        /// </summary>
        /// <returns>The event source name</returns>
        protected override string GetEventSourceName()
        {
            return eventSourceName;
        }
    }
}
