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
using Transformalize.Libs.EnterpriseLibrary.Common.Configuration.Storage;

namespace Transformalize.Libs.EnterpriseLibrary.Common.Configuration
{
    /// <summary>
    /// Watcher for configuration sections in configuration files.
    /// </summary>
    /// <remarks>
    /// This implementation uses a <see cref="ConfigurationChangeFileWatcher"/> to watch for changes 
    /// in the configuration files.
    /// </remarks>
    public class ConfigurationFileSourceWatcher : ConfigurationSourceWatcher, IDisposable
    {
        private readonly string configurationFilepath;
        private ConfigurationChangeFileWatcher configWatcher;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationFileSourceWatcher"/> class.
        /// </summary>
        /// <param name="configurationFilepath">The path for the configuration file to watch.</param>
        /// <param name="configSource">The identification of the configuration source.</param>
        /// <param name="refresh"><b>true</b> if changes should be notified, <b>false</b> otherwise.</param>
        /// <param name="refreshInterval">The poll interval in milliseconds.</param>
        /// <param name="changed">The callback for changes notification.</param>
        public ConfigurationFileSourceWatcher(
            string configurationFilepath,
            string configSource,
            bool refresh,
            int refreshInterval,
            ConfigurationChangedEventHandler changed)
            : base(configSource, refresh, changed)
        {
            this.configurationFilepath = configurationFilepath;

            if (refresh)
            {
                SetUpWatcher(refreshInterval, changed);
            }
        }

        private void SetUpWatcher(int refreshInterval, ConfigurationChangedEventHandler changed)
        {
            this.configWatcher =
                new ConfigurationChangeFileWatcher(
                    GetFullFileName(this.configurationFilepath, this.ConfigSource),
                    this.ConfigSource);
            this.configWatcher.SetPollDelayInMilliseconds(refreshInterval);
            this.configWatcher.ConfigurationChanged += changed;
        }

        /// <summary>
        /// Gets the full file name associated to the configuration source.
        /// </summary>
        /// <param name="configurationFilepath">The path for the main configuration file.</param>
        /// <param name="configSource">The configuration source to watch.</param>
        /// <returns>The path to the configuration file to watch. It will be the same as <paramref name="configurationFilepath"/>
        /// if <paramref name="configSource"/> is empty, or the full path for <paramref name="configSource"/> considered as a 
        /// file name relative to the main configuration file.</returns>
        public static string GetFullFileName(string configurationFilepath, string configSource)
        {
            if (string.Empty == configSource)
            {
                // watch app.config/web.config
                return configurationFilepath;
            }
            else
            {
                // watch an external file
                if (!Path.IsPathRooted(configSource))
                {
                    // REVIEW - this is ok?
                    return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, configSource);
                }
                else
                {
                    return configSource;
                }
            }
        }

        /// <summary>
        /// Gets the watcher over the serialization medium.
        /// </summary>
        public override ConfigurationChangeWatcher Watcher
        {
            get { return this.configWatcher; }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.configWatcher != null)
                {
                    this.configWatcher.Dispose();
                    this.configWatcher = null;
                }
            }
        }
    }
}
