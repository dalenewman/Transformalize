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
using System.ComponentModel;
using System.Configuration;
using System.Globalization;
using System.IO;

namespace Transformalize.Libs.EnterpriseLibrary.Common.Configuration
{
    /// <summary>
    /// Represents the implementation details for file-based configuration sources.
    /// </summary>
    /// <remarks>
    /// This implementation deals with setting up the watcher over the configuration files to detect changes and update
    /// the configuration representation. It also manages the change notification features provided by the file based 
    /// configuration sources.
    /// </remarks>
    public abstract class FileBasedConfigurationSource : IDisposable, IConfigurationSourceTest
    {
        /// <summary>
        /// ConfigSource value for sections that existed in configuration but were later removed.
        /// </summary>
        public const string NullConfigSource = "__null__";

        private readonly string configurationFilepath;
        private readonly bool refresh;
        private readonly int refreshInterval;

        private readonly object refreshLock;        // lock used to process one file update notification at a time

        private readonly object eventHandlersLock;  // lock used to protect the event handlers list
        private readonly EventHandlerList eventHandlers;

        private readonly object watchersLock;       // lock used to protect the watcher data structures
        private ConfigurationSourceWatcher configFileWatcher;
        private readonly Dictionary<string, ConfigurationSourceWatcher> watchedConfigSourceMapping;
        private readonly Dictionary<string, ConfigurationSourceWatcher> watchedSectionMapping;

        private readonly CompositeConfigurationSourceHandler CompositeConfigurationHandler;
        private readonly HierarchicalConfigurationSourceHandler HierarchicalConfigurationHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileBasedConfigurationSource"/> class.
        /// </summary>
        /// <param name="configurationFilepath">The path for the main configuration file.</param>
        /// <param name="refresh"><b>true</b>if runtime changes should be refreshed, <b>false</b> otherwise.</param>
        /// <param name="refreshInterval">The poll interval in milliseconds.</param>
        protected FileBasedConfigurationSource(string configurationFilepath, bool refresh, int refreshInterval)
        {
            this.configurationFilepath = configurationFilepath;
            this.refresh = refresh && !string.IsNullOrEmpty(configurationFilepath);
            this.refreshInterval = refreshInterval;
            this.refreshLock = new object();

            this.eventHandlersLock = new object();
            this.eventHandlers = new EventHandlerList();

            this.watchersLock = new object();
            this.watchedConfigSourceMapping = new Dictionary<string, ConfigurationSourceWatcher>();
            this.watchedSectionMapping = new Dictionary<string, ConfigurationSourceWatcher>();

            CompositeConfigurationHandler = new CompositeConfigurationSourceHandler(this);
            CompositeConfigurationHandler.ConfigurationSectionChanged += new ConfigurationChangedEventHandler(handler_ConfigurationSectionChanged);
            CompositeConfigurationHandler.ConfigurationSourceChanged += new EventHandler<ConfigurationSourceChangedEventArgs>(handler_ConfigurationSourceChanged);

            HierarchicalConfigurationHandler = new HierarchicalConfigurationSourceHandler(this);
            HierarchicalConfigurationHandler.ConfigurationSectionChanged += new ConfigurationChangedEventHandler(handler_ConfigurationSectionChanged);
            HierarchicalConfigurationHandler.ConfigurationSourceChanged += new EventHandler<ConfigurationSourceChangedEventArgs>(handler_ConfigurationSourceChanged);
        }

        void handler_ConfigurationSourceChanged(object sender, ConfigurationSourceChangedEventArgs e)
        {
            this.OnSourceChanged(new ConfigurationSourceChangedEventArgs(this, e.ChangedSectionNames));
        }

        void handler_ConfigurationSectionChanged(object sender, ConfigurationChangedEventArgs e)
        {
            this.DoNotifyUpdatedSection(e.SectionName);
        }

        #region IConfigurationSource members

        /// <summary>
        /// Retrieves the specified <see cref="ConfigurationSection"/> from the configuration file, and starts watching for 
        /// its changes if not watching already.
        /// </summary>
        /// <param name="sectionName">The section name.</param>
        /// <returns>The section, or <see langword="null"/> if it doesn't exist.</returns>
        public ConfigurationSection GetSection(string sectionName)
        {
            ConfigurationSection configurationSection = DoGetSection(sectionName);

            SetConfigurationWatchers(sectionName, configurationSection);

            configurationSection = CompositeConfigurationHandler.CheckGetSection(sectionName, configurationSection);

            return HierarchicalConfigurationHandler.CheckGetSection(sectionName, configurationSection);

        }

        /// <summary>
        /// Event raised when any section in this configuration source has changed.
        /// </summary>
        public event EventHandler<ConfigurationSourceChangedEventArgs> SourceChanged;

        /// <summary>
        /// Adds a handler to be called when changes to section <code>sectionName</code> are detected.
        /// </summary>
        /// <param name="sectionName">The name of the section to watch for.</param>
        /// <param name="handler">The handler.</param>
        public void AddSectionChangeHandler(string sectionName, ConfigurationChangedEventHandler handler)
        {
            lock (eventHandlersLock)
            {
                eventHandlers.AddHandler(sectionName, handler);
            }
        }

        /// <summary>
        /// Remove a handler to be called when changes to section <code>sectionName</code> are detected.
        /// </summary>
        /// <param name="sectionName">The name of the section to watch for.</param>
        /// <param name="handler">The handler.</param>
        public void RemoveSectionChangeHandler(string sectionName, ConfigurationChangedEventHandler handler)
        {
            lock (eventHandlersLock)
            {
                eventHandlers.RemoveHandler(sectionName, handler);
            }
        }

        /// <summary>
        /// Adds a <see cref="ConfigurationSection"/> to the configuration and saves the configuration source.
        /// </summary>
        /// <remarks>
        /// If a configuration section with the specified name already exists it will be replaced.
        /// If a configuration section was retrieved from an instance of <see cref="FileBasedConfigurationSource"/>, a <see cref="System.InvalidOperationException"/> will be thrown.
        /// </remarks>
        /// <param name="sectionName">The name by which the <paramref name="configurationSection"/> should be added.</param>
        /// <param name="configurationSection">The configuration section to add.</param>
        /// <exception cref="System.InvalidOperationException">The configuration section was retrieved from an instance of  <see cref="FileBasedConfigurationSource"/> or <see cref="Configuration"/> and cannot be added to the current source.</exception>
        public void Add(
            string sectionName,
            ConfigurationSection configurationSection)
        {
            if (!CompositeConfigurationHandler.CheckAddSection(sectionName, configurationSection))
            {
                DoAdd(sectionName, configurationSection);
            }
        }

        /// <summary>
        /// When implemented in a derived class, adds a <see cref="ConfigurationSection"/> to the configuration and saves the configuration source.
        /// </summary>
        /// <remarks>
        /// If a configuration section with the specified name already exists it should be replaced.
        /// </remarks>
        /// <param name="sectionName">The name by which the <paramref name="configurationSection"/> should be added.</param>
        /// <param name="configurationSection">The configuration section to add.</param>
        public abstract void DoAdd(
            string sectionName,
            ConfigurationSection configurationSection);


        /// <summary>
        /// Removes a <see cref="ConfigurationSection"/> from the configuration and saves the configuration source.
        /// </summary>
        /// <param name="sectionName">The name of the section to remove.</param>
        public void Remove(string sectionName)
        {
            if (!CompositeConfigurationHandler.CheckRemoveSection(sectionName))
            {
                DoRemove(sectionName);
            }
        }

        /// <summary>
        /// When implemented in a derived class, removes a <see cref="ConfigurationSection"/> from the configuration and saves the configuration source.
        /// </summary>
        /// <param name="sectionName">The name of the section to remove.</param>
        public abstract void DoRemove(string sectionName);

        #endregion


        /// <summary>
        /// This method supports the Enterprise Library infrastructure and is not intended to be used directly from your code.
        /// </summary>
        /// <param name="configSource">The name of the updated configuration source.</param>
        /// <devdoc>
        /// Only needs to deal with concurrency to get the current sections and to update the watchers.
        /// 
        /// Rationale:
        /// - Sections' are only added or updated.
        /// - For this notification, all sections in the configuration file must be updated, and sections in external 
        /// files must be refreshed only if the config source changed.
        /// - why not check after the original set of sections is retrieved?
        /// -- Sections might have been added to the listener set after the original set is retrieved, but...
        /// -- If they were added after the original set was retrieved, then they are up to date.
        /// --- For this to happen, they couldn't have been read before the o.s., otherwise they would be a listener for them.
        /// --- So, the retrieved information must be fresh (checked this with a test). 
        /// -- What about other changes?
        /// --- Erased sections: only tested in the configuration file watching thread, because the meta configuration 
        /// is kept in the configuration file.
        /// ---- Even if the external file an external is deleted because of the deletion, and this change is processed
        /// before the configuration file change, the refresh for the external section will refresh all the sections for the file and 
        /// notify a change, without need for checking the change. The change would later be picked up by the configuration file watcher 
        /// which will notify again. This shouldn't be a problem.
        /// --- External sections with changed sources. If they existed before, they must have been in the configuration file and there 
        /// was an entry in the bookeeping data structures.
        /// - Concurrent updates for sections values should be handled by the system.config fx
        /// </devdoc>
        private void ConfigSourceChanged(string configSource)
        {
            lock (this.refreshLock)
            {
                IDictionary<string, string> localSectionsToRefresh = new Dictionary<string, string>();
                IDictionary<string, string> externalSectionsToRefresh = new Dictionary<string, string>();

                IDictionary<string, string> sectionsWithChangedConfigSource;
                ICollection<string> sectionsToNotify = new List<string>();

                AddKnownSectionsToRefresh(localSectionsToRefresh, externalSectionsToRefresh);

                RefreshAndValidateSections(
                    localSectionsToRefresh,
                    externalSectionsToRefresh,
                    out sectionsToNotify,
                    out sectionsWithChangedConfigSource);

                UpdateWatchersForSections(sectionsWithChangedConfigSource);

                NotifyUpdatedSections(sectionsToNotify);
            }
        }

        private void AddKnownSectionsToRefresh(
            IDictionary<string, string> localSectionsToRefresh,
            IDictionary<string, string> externalSectionsToRefresh)
        {
            lock (this.watchersLock)
            {
                if (this.configFileWatcher != null)
                {
                    AddSectionsToUpdate(this.configFileWatcher, localSectionsToRefresh);
                }
                foreach (ConfigurationSourceWatcher watcher in this.watchedConfigSourceMapping.Values)
                {
                    if (watcher != this.configFileWatcher)
                    {
                        AddSectionsToUpdate(watcher, externalSectionsToRefresh);
                    }
                }
            }
        }

        private void ExternalConfigSourceChanged(string configSource)
        {
            lock (this.refreshLock)
            {
                List<string> sectionsToNotify = new List<string>();

                lock (this.watchersLock)
                {
                    ConfigurationSourceWatcher watcher;
                    if (this.watchedConfigSourceMapping.TryGetValue(configSource, out watcher))
                    {
                        sectionsToNotify.AddRange(watcher.WatchedSections);
                    }
                }

                RefreshExternalSections(sectionsToNotify);

                NotifyUpdatedSections(sectionsToNotify);
            }
        }

        private static void AddSectionsToUpdate(
            ConfigurationSourceWatcher watcher,
            IDictionary<string, string> sectionsToUpdate)
        {
            foreach (string section in watcher.WatchedSections)
            {
                sectionsToUpdate.Add(section, watcher.ConfigSource);
            }
        }

        private ConfigurationSourceWatcher CreateWatcherForConfigSource(string configSource)
        {
            ConfigurationSourceWatcher watcher;

            if (string.Empty == configSource)
            {
                watcher = new ConfigurationFileSourceWatcher(configurationFilepath,
                                                             configSource,
                                                             refresh,
                                                             this.refreshInterval,
                                                             OnConfigurationChanged);
                configFileWatcher = watcher;
            }
            else
            {
                watcher = new ConfigurationFileSourceWatcher(configurationFilepath,
                                                             configSource,
                                                             refresh && !NullConfigSource.Equals(configSource),
                                                             this.refreshInterval,
                                                             OnExternalConfigurationChanged);
            }

            watchedConfigSourceMapping.Add(configSource, watcher);

            return watcher;
        }


        /// <summary>
        /// Releases the resources used by the change watchers.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreach (IDisposable watcher in watchedConfigSourceMapping.Values)
                {
                    watcher.Dispose();
                }

                if (CompositeConfigurationHandler != null)
                {
                    CompositeConfigurationHandler.Dispose();
                }
                if (HierarchicalConfigurationHandler != null)
                {
                    HierarchicalConfigurationHandler.Dispose();
                }

                this.eventHandlers.Dispose();
            }
        }

        /// <summary>
        /// Releases the resources used by the change watchers.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private bool IsWatchingConfigSource(string configSource)
        {
            return watchedConfigSourceMapping.ContainsKey(configSource);
        }

        private bool IsWatchingSection(string sectionName)
        {
            return watchedSectionMapping.ContainsKey(sectionName);
        }

        private void LinkWatcherForSection(ConfigurationSourceWatcher watcher, string sectionName)
        {
            watchedSectionMapping.Add(sectionName, watcher);
            watcher.WatchedSections.Add(sectionName);
        }

        /// <summary/>
        protected void NotifyUpdatedSections(IEnumerable<string> sectionsToNotify)
        {
            this.OnSourceChanged(new ConfigurationSourceChangedEventArgs(this, sectionsToNotify));

            foreach (string sectionName in sectionsToNotify)
            {
                this.DoNotifyUpdatedSection(sectionName);
            }
        }

        private void DoNotifyUpdatedSection(string sectionName)
        {
            Delegate[] invocationList;

            lock (eventHandlersLock)
            {
                ConfigurationChangedEventHandler callbacks = (ConfigurationChangedEventHandler)eventHandlers[sectionName];
                if (callbacks == null)
                {
                    return;
                }
                invocationList = callbacks.GetInvocationList();
            }

            ConfigurationChangedEventArgs eventData = new ConfigurationChangedEventArgs(sectionName);
            try
            {
                foreach (ConfigurationChangedEventHandler callback in invocationList)
                {
                    if (callback != null)
                    {
                        callback(this, eventData);
                    }
                }
            }
            catch // (Exception e)
            {
                //EventLog.WriteEntry(GetEventSourceName(), Resources.ExceptionEventRaisingFailed + GetType().FullName + " :" + e.Message);
            }
        }

        /// <summary>
        /// Retrieves the specified <see cref="ConfigurationSection"/> from the configuration file.
        /// </summary>
        /// <param name="sectionName">The section name.</param>
        /// <returns>The section, or <see langword="null"/> if it doesn't exist.</returns>
        protected abstract ConfigurationSection DoGetSection(string sectionName);

        /// <summary>
        /// Raises the <see cref="SourceChanged"/> event.
        /// </summary>
        /// <param name="configurationSourceChangedEventArgs">The argument for the raised event.</param>
        protected virtual void OnSourceChanged(ConfigurationSourceChangedEventArgs configurationSourceChangedEventArgs)
        {
            var handler = this.SourceChanged;
            if (handler != null)
            {
                handler(this, configurationSourceChangedEventArgs);
            }
        }

        private void OnConfigurationChanged(object sender, ConfigurationChangedEventArgs args)
        {
            ConfigSourceChanged(args.SectionName);
        }

        private void OnExternalConfigurationChanged(object sender, ConfigurationChangedEventArgs args)
        {
            ExternalConfigSourceChanged(args.SectionName);
        }

        /// <summary>
        /// Refreshes the configuration sections from the main configuration file and determines which sections have suffered notifications
        /// and should be notified to registered handlers.
        /// </summary>
        /// <param name="localSectionsToRefresh">A dictionary with the configuration sections residing in the main configuration file that must be refreshed.</param>
        /// <param name="externalSectionsToRefresh">A dictionary with the configuration sections residing in external files that must be refreshed.</param>
        /// <param name="sectionsToNotify">A new collection with the names of the sections that suffered changes and should be notified.</param>
        /// <param name="sectionsWithChangedConfigSource">A new dictionary with the names and file names of the sections that have changed their location.</param>
        protected abstract void RefreshAndValidateSections(IDictionary<string, string> localSectionsToRefresh,
                                                           IDictionary<string, string> externalSectionsToRefresh,
                                                           out ICollection<string> sectionsToNotify,
                                                           out IDictionary<string, string> sectionsWithChangedConfigSource);

        /// <summary>
        /// Refreshes the configuration sections from an external configuration file.
        /// </summary>
        /// <param name="sectionsToRefresh">A collection with the names of the sections that suffered changes and should be refreshed.</param>
        protected abstract void RefreshExternalSections(IEnumerable<string> sectionsToRefresh);

        private void RemoveConfigSourceWatcher(ConfigurationSourceWatcher watcher)
        {
            watchedConfigSourceMapping.Remove(watcher.ConfigSource);
            (watcher as IDisposable).Dispose();
        }

        private void SetConfigurationWatchers(string sectionName, ConfigurationSection configurationSection)
        {
            if (configurationSection != null)
            {
                lock (this.watchersLock)
                {
                    if (!IsWatchingSection(sectionName))
                    {
                        SetWatcherForSection(sectionName, configurationSection.SectionInformation.ConfigSource);
                    }
                }
            }
        }

        private void SetWatcherForSection(string sectionName, string configSource)
        {
            ConfigurationSourceWatcher currentConfigSourceWatcher;
            watchedConfigSourceMapping.TryGetValue(configSource, out currentConfigSourceWatcher);

            if (currentConfigSourceWatcher == null)
            {
                currentConfigSourceWatcher = CreateWatcherForConfigSource(configSource);
            }
            else
            {
                currentConfigSourceWatcher.StopWatching();
            }
            LinkWatcherForSection(currentConfigSourceWatcher, sectionName);
            currentConfigSourceWatcher.StartWatching();

            // must watch the app.config if not watching already
            if ((string.Empty != configSource) && (!IsWatchingConfigSource(string.Empty)))
            {
                CreateWatcherForConfigSource(string.Empty).StartWatching();
            }
        }

        private void UnlinkWatcherForSection(ConfigurationSourceWatcher watcher, string sectionName)
        {
            watchedSectionMapping.Remove(sectionName);
            watcher.WatchedSections.Remove(sectionName);
            if (watcher.WatchedSections.Count == 0 && configFileWatcher != watcher)
            {
                RemoveConfigSourceWatcher(watcher);
            }
        }

        private void UpdateWatcherForSection(string sectionName, string configSource)
        {
            ConfigurationSourceWatcher currentSectionWatcher;
            this.watchedSectionMapping.TryGetValue(sectionName, out currentSectionWatcher);

            if (currentSectionWatcher == null || currentSectionWatcher.ConfigSource != configSource)
            {
                if (currentSectionWatcher != null)
                {
                    UnlinkWatcherForSection(currentSectionWatcher, sectionName);
                }

                if (configSource != null)
                {
                    SetWatcherForSection(sectionName, configSource);
                }
            }
        }

        private void UpdateWatchersForSections(IEnumerable<KeyValuePair<string, string>> sectionsChangingSource)
        {
            lock (this.watchersLock)
            {
                foreach (KeyValuePair<string, string> sectionSourcePair in sectionsChangingSource)
                {
                    UpdateWatcherForSection(sectionSourcePair.Key, sectionSourcePair.Value);
                }
            }
        }

        /// <summary>
        /// Gets the path of the configuration file for the configuration source.
        /// </summary>
        protected string ConfigurationFilePath
        {
            get { return this.configurationFilepath; }
        }


        /// <summary>
        /// Validates the parameters required to save a configuration section.
        /// </summary>
        protected static void ValidateArgumentsAndFileExists(
            string fileName,
            string section,
            ConfigurationSection configurationSection)
        {
            if (string.IsNullOrEmpty(fileName)) throw new ArgumentException(Resources.ExceptionStringNullOrEmpty, "fileName");
            if (string.IsNullOrEmpty(section)) throw new ArgumentException(Resources.ExceptionStringNullOrEmpty, "section");
            if (null == configurationSection) throw new ArgumentNullException("configurationSection");

            if (!File.Exists(fileName))
            {
                throw new FileNotFoundException(
                    string.Format(CultureInfo.CurrentCulture, Resources.ExceptionConfigurationFileNotFound, section),
                    fileName);
            }
        }

        #region test support members

        void IConfigurationSourceTest.ConfigSourceChanged(string configSource)
        {
            this.ConfigSourceChanged(configSource);
        }

        void IConfigurationSourceTest.ExternalConfigSourceChanged(string configSource)
        {
            this.ExternalConfigSourceChanged(configSource);
        }

        IDictionary<string, ConfigurationSourceWatcher> IConfigurationSourceTest.ConfigSourceWatcherMappings
        {
            get { return watchedConfigSourceMapping; }
        }

        ICollection<string> IConfigurationSourceTest.WatchedConfigSources
        {
            get { return watchedConfigSourceMapping.Keys; }
        }

        ICollection<string> IConfigurationSourceTest.WatchedSections
        {
            get { return watchedSectionMapping.Keys; }
        }

        #endregion
    }
}
