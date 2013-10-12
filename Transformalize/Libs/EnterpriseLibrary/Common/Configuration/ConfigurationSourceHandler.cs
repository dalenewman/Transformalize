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
using System.Linq;

namespace Transformalize.Libs.EnterpriseLibrary.Common.Configuration
{
    /// <summary>
    /// Implements common behavior for classes that add extended functionality to <see cref="IConfigurationSource"/> implementations.<br/>
    /// This class can create subordinate sources based on the <see cref="ConfigurationSourceSection"/> configuration and propagates change events 
    /// From these sources to the main source.
    /// </summary>
    /// <seealso cref="IConfigurationSource"/>
    /// <seealso cref="CompositeConfigurationSourceHandler"/>
    /// <seealso cref="HierarchicalConfigurationSourceHandler"/>
    public abstract class ConfigurationSourceHandler : IDisposable
    {
        private object LockObject = new object();

        IConfigurationSource configurationSource;
        EventHandler<ConfigurationSourceChangedEventArgs> configurationSourceChangedHandler;

        Dictionary<string, SubordinateSource> subordinateSourcesByName = new Dictionary<string, SubordinateSource>();
        Dictionary<string, SectionInSubordinateSource> sectionMappings = new Dictionary<string, SectionInSubordinateSource>();

        private bool initialized = false;

        /// <summary>
        /// Creates a new instance of <see cref="ConfigurationSourceHandler"/> passing the <see cref="IConfigurationSource"/> implementation
        /// That contains the <see cref="ConfigurationSourceSection"/> configuration.
        /// </summary>
        /// <param name="configurationSource">The <see cref="IConfigurationSource"/> implementation that should be extended.</param>
        protected ConfigurationSourceHandler(IConfigurationSource configurationSource)
        {
            this.configurationSource = configurationSource;
        }

        /// <summary>
        /// Checks whether a call to <see cref="IConfigurationSource.GetSection(string)"/> should be extended.<br/>
        /// If the call should be extended performs the extended behavior and returns the modified <see cref="ConfigurationSection"/> intance.<br/>
        /// If the call should not be extended returns <paramref name="configurationSection"/>.
        /// </summary>
        /// <param name="sectionName">The name of the section that was retrieved from configuration.</param>
        /// <param name="configurationSection">The section that was retrieved from configuration.</param>
        /// <returns>The resulting <see cref="ConfigurationSection"/> instance.</returns>
        /// <seealso cref="IConfigurationSource.GetSection(string)"/>
        public ConfigurationSection CheckGetSection(string sectionName, ConfigurationSection configurationSection)
        {
            //design time managers occasionally call with sectionName == "".
            //this should be fixed in designtime managers
            if (string.IsNullOrEmpty(sectionName)) //   throw new ArgumentException(Resources.ExceptionStringNullOrEmpty, "sectionName");
            {
                return configurationSection;
            }

            //if we are already loading we should return.
            if (RecursionLock.InsideHandlerOperation)
            {
                return configurationSection;
            }

            //this is a section we depend on internally
            if (sectionName == ConfigurationSourceSection.SectionName)
            {
                return configurationSection;
            }

            lock (LockObject)
            {
                using (new RecursionLock())
                {
                    EnsureInitialized();

                    return DoCheckGetSection(sectionName, configurationSection);
                }
            }
        }

        /// <summary>
        /// When overridden in a derived class, checks whether a call to <see cref="IConfigurationSource.GetSection(string)"/> should be extended.<br/>
        /// If the call should be extended performs the extended behavior and returns the modified <see cref="ConfigurationSection"/> intance.<br/>
        /// If the call should not be extended returns <paramref name="configurationSection"/>.
        /// </summary>
        /// <param name="sectionName">The name of the section that was retrieved from configuration.</param>
        /// <param name="configurationSection">The section that was retrieved from configuration.</param>
        /// <returns>The <see cref="ConfigurationSection"/> instance passed as <paramref name="configurationSection"/>.</returns>
        /// <seealso cref="IConfigurationSource.GetSection(string)"/>
        protected virtual ConfigurationSection DoCheckGetSection(string sectionName, ConfigurationSection configurationSection)
        {
            return configurationSection;
        }

        /// <summary>
        /// Checks whether a call to <see cref="IConfigurationSource.Add(string, ConfigurationSection)"/> should be extended.<br/>
        /// If the call should be extended performs the extended behavior.
        /// </summary>
        /// <param name="sectionName">The name of the section that should be stored in configuration.</param>
        /// <param name="configurationSection">The section that should be stored in configuration.</param>
        /// <returns><see langword="true"/> if the call to <see cref="IConfigurationSource.Add(string, ConfigurationSection)"/> was handled by the extension.</returns>
        /// <seealso cref="IConfigurationSource.Add(string, ConfigurationSection)"/>
        public bool CheckAddSection(string sectionName, ConfigurationSection configurationSection)
        {
            //if we are adding, we should return.
            if (RecursionLock.InsideHandlerOperation)
            {
                return false;
            }

            //this is a section we depend on internally
            if (sectionName == ConfigurationSourceSection.SectionName)
            {
                return false;
            }

            lock (LockObject)
            {
                using (new RecursionLock())
                {
                    EnsureInitialized();

                    return DoCheckAddSection(sectionName, configurationSection);
                }
            }
        }

        /// <summary>
        /// When overridden in a derived class, checks whether a call to <see cref="IConfigurationSource.Add(string, ConfigurationSection)"/> should be extended.<br/>
        /// If the call should be extended performs the extended behavior.
        /// </summary>
        /// <param name="sectionName">The name of the section that should be stored in configuration.</param>
        /// <param name="configurationSection">The section that should be stored in configuration.</param>
        /// <returns><see langword="false"/></returns>
        /// <seealso cref="IConfigurationSource.Add(string, ConfigurationSection)"/>
        protected virtual bool DoCheckAddSection(string sectionName, ConfigurationSection configurationSection)
        {
            return false;
        }

        /// <summary>
        /// Checks whether a call to <see cref="IConfigurationSource.Remove(string)"/> should be extended.<br/>
        /// If the call should be extended performs the extended behavior.
        /// </summary>
        /// <param name="sectionName">The name of the section that should be removed from configuration.</param>
        /// <returns><see langword="true"/> if the call to <see cref="IConfigurationSource.Remove(string)"/> was handled by the extension.</returns>
        /// <seealso cref="IConfigurationSource.Remove(string)"/>
        public bool CheckRemoveSection(string sectionName)
        {
            //if we are adding, we should return.
            if (RecursionLock.InsideHandlerOperation)
            {
                return false;
            }

            //this is a section we depend on internally
            if (sectionName == ConfigurationSourceSection.SectionName)
            {
                return false;
            }

            lock (LockObject)
            {
                using (new RecursionLock())
                {
                    EnsureInitialized();

                    return DoCheckRemoveSection(sectionName);
                }
            }
        }

        /// <summary>
        /// When overridden in a derived class, checks whether a call to <see cref="IConfigurationSource.Remove(string)"/> should be extended.<br/>
        /// If the call should be extended performs the extended behavior.
        /// </summary>
        /// <param name="sectionName">The name of the section that should be removed from configuration.</param>
        /// <returns><see langword="false"/></returns>
        /// <seealso cref="IConfigurationSource.Remove(string)"/>
        protected virtual bool DoCheckRemoveSection(string sectionName)
        {
            return false;
        }

        private void EnsureInitialized()
        {
            if (!initialized)
            {
                Initialize();

                initialized = true;
            }
        }

        /// <summary>
        /// Performs intialization logic for this <see cref="ConfigurationSourceHandler"/>.
        /// </summary>
        protected virtual void Initialize()
        {
            configurationSourceChangedHandler = new EventHandler<ConfigurationSourceChangedEventArgs>(configurationSource_ConfigurationSourceChanged);
            configurationSource.SourceChanged += configurationSourceChangedHandler;
        }

        private void configurationSource_ConfigurationSourceChanged(object sender, ConfigurationSourceChangedEventArgs args)
        {
            if (args.ChangedSectionNames.Contains(ConfigurationSourceSection.SectionName))
            {
                Refresh();
            }
        }

        /// <summary>
        /// Performs re-intialization logic for this <see cref="ConfigurationSourceHandler"/>.
        /// </summary>
        protected void Refresh()
        {
            lock (LockObject)
            {
                DoRefresh();
                DoConfigurationSourceChanged(sectionMappings.Keys);
            }
        }

        /// <summary>
        /// Perform required refresh actions as needed when source changes.
        /// </summary>
        /// <returns>Sequence of changed sections</returns>
        protected virtual void DoRefresh()
        {
            RefreshSubordinateSources();
            RefreshExistingSectionMappings();
        }

        private void RefreshSubordinateSources()
        {
            var removedSources = SourceNamesToRemove();
            ClearRemovedSubordinateSources(removedSources);
            ClearRemovedSubordinateSections(removedSources);
            RefreshExistingSubordinateSources();
        }

        private IEnumerable<string> SourceNamesToRemove()
        {
            var configSourceSection =
                (ConfigurationSourceSection)configurationSource.GetSection(ConfigurationSourceSection.SectionName);

            // If we don't have a configuration sources section, that means things were built up
            // manually for some reason, so don't muck with it.
            if (configSourceSection == null)
                return Enumerable.Empty<string>();

            var sourcesThatStay =
                from sourceName in subordinateSourcesByName.Keys
                join sourceElement in configSourceSection.Sources
                    on sourceName equals sourceElement.Name
                select sourceName;


            var sourcesToRemove = new HashSet<string>(subordinateSourcesByName.Keys);
            foreach (var source in sourcesThatStay)
            {
                sourcesToRemove.Remove(source);
            }

            return sourcesToRemove;
        }

        private void ClearRemovedSubordinateSources(IEnumerable<string> sourcesToRemove)
        {
            foreach (var sourceToRemove in sourcesToRemove)
            {
                var subordinate = subordinateSourcesByName[sourceToRemove];
                subordinate.Dispose();
                subordinateSourcesByName.Remove(sourceToRemove);
            }
        }

        private void ClearRemovedSubordinateSections(IEnumerable<string> removedSources)
        {
            var sectionNamesToRemove = sectionMappings.Join(
                removedSources,
                section => section.Key,
                removedName => removedName,
                (section, removedName) => removedName).ToList();

            foreach (var sectionNameToRemove in sectionNamesToRemove)
            {
                sectionMappings.Remove(sectionNameToRemove);
            }
        }

        private void RefreshExistingSubordinateSources()
        {
            foreach (var subordinateSource in subordinateSourcesByName.Values.ToArray())
            {
                subordinateSource.Refresh();
            }
        }


        private void RefreshExistingSectionMappings()
        {
            if (sectionMappings.Count > 0)
            {
                foreach (SectionInSubordinateSource sectionInSubordinateSource in sectionMappings.Values.ToArray())
                {
                    SubordinateSource subordinateSource = null;
                    if (!subordinateSourcesByName.TryGetValue(sectionInSubordinateSource.SubordinateSourceName, out subordinateSource))
                    {
                        sectionMappings.Remove(sectionInSubordinateSource.SectionName);
                    }
                    else
                    {
                        if (subordinateSource.Source != null)
                        {
                            sectionInSubordinateSource.Refresh(subordinateSource.Source);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Raises the <see cref="ConfigurationSourceHandler.ConfigurationSectionChanged"/> event.
        /// </summary>
        /// <param name="affectedSections">The names of the sections that are changed.</param>
        protected void DoConfigurationSourceChanged(IEnumerable<string> affectedSections)
        {
            EventHandler<ConfigurationSourceChangedEventArgs> handler = ConfigurationSourceChanged;
            if (handler != null)
            {
                handler(this, new ConfigurationSourceChangedEventArgs(configurationSource, affectedSections));
            }
        }

        /// <summary>
        /// Raises the <see cref="ConfigurationSourceHandler.ConfigurationSectionChanged"/> event.
        /// </summary>
        /// <param name="sectionName">The name of the section that was changed.</param>
        protected void DoConfigurationSectionChanged(string sectionName)
        {
            ConfigurationChangedEventHandler handler = ConfigurationSectionChanged;
            if (handler != null)
            {
                handler(this, new ConfigurationChangedEventArgs(sectionName));
            }
        }

        /// <summary>
        /// Indicate that a mapped section from one of the subordinate <see cref="IConfigurationSource"/>'s was changed.
        /// </summary>
        public event ConfigurationChangedEventHandler ConfigurationSectionChanged;

        /// <summary>
        /// Indicate a subordinate <see cref="IConfigurationSource"/>'s was changed.
        /// </summary>
        public event EventHandler<ConfigurationSourceChangedEventArgs> ConfigurationSourceChanged;

        /// <summary>
        /// Adds a subordinate <see cref="IConfigurationSource"/> to the <see cref="ConfigurationSourceHandler"/>.
        /// This <see cref="IConfigurationSource"/> will not be refreshed or disposed.
        /// </summary>
        /// <param name="sourceName">The name under which the <see cref="IConfigurationSource"/> will be added.</param>
        /// <param name="configurationSource">The <see cref="IConfigurationSource"/> that will be added.</param>
        protected void AddCustomSubordinateSource(string sourceName, IConfigurationSource configurationSource)
        {
            SubordinateSource sourceHolder = new SubordinateSource(this, sourceName, configurationSource, true);
            subordinateSourcesByName.Add(sourceName, sourceHolder);
        }

        /// <summary>
        /// Returns a subordinate <see cref="IConfigurationSource"/> with the specified name.<br/>
        /// Throws <see cref="ConfigurationSourceErrorsException"/> if the source was not found.
        /// </summary>
        /// <param name="sourceName">The name of the source that should be returned.</param>
        /// <returns>The <see cref="IConfigurationSource"/> instance.</returns>
        protected IConfigurationSource GetSubordinateSource(string sourceName)
        {
            SubordinateSource sourceHolder = null;
            if (subordinateSourcesByName.TryGetValue(sourceName, out sourceHolder))
            {
                return sourceHolder.Source;
            }

            return CreateSubordinateSource(sourceName, true);
        }

        private IConfigurationSource CreateSubordinateSource(string sourceName, bool throwWhenNotFound)
        {
            EnsureInitialized();

            ConfigurationSourceSection configurationSourcesSection = configurationSource.GetSection(ConfigurationSourceSection.SectionName) as ConfigurationSourceSection;
            if (configurationSourcesSection == null)
            {
                return null;
            }
            else
            {
                var configurationSourceElement = configurationSourcesSection.Sources
                                                    .Where(x => x.Name == sourceName)
                                                    .FirstOrDefault();

                if (configurationSourceElement == null)
                {
                    if (throwWhenNotFound)
                    {
                        string message = string.Format(CultureInfo.CurrentCulture,
                            Resources.ExceptionConfigurationSourceNotFound,
                            sourceName,
                            ConfigurationSourceSection.SectionName);

                        throw new ConfigurationSourceErrorsException(message);
                    }
                    else
                    {
                        return null;
                    }
                }
                IConfigurationSource source = configurationSourceElement.CreateSource();

                SubordinateSource sourceHolder = new SubordinateSource(this, sourceName, source);
                subordinateSourcesByName[sourceName] = sourceHolder;

                return source;

            }
        }

        /// <summary>
        /// Ensures <see cref="ConfigurationSourceHandler.ConfigurationSourceChanged"/> events are raised for 
        /// Changes in a subordinate section.
        /// </summary>
        /// <param name="sourceName">The name of the subordinate configuration source that contains the section.</param>
        /// <param name="sectionName">The name of the section events should be propagated for.</param>
        protected void EnsurePropagatingSectionChangeEvents(string sourceName, string sectionName)
        {
            if (!sectionMappings.ContainsKey(sectionName))
            {
                IConfigurationSource source = GetSubordinateSource(sourceName);

                sectionMappings.Add(sectionName, new SectionInSubordinateSource(this, sectionName, sourceName, source));
            }
        }

        /// <summary>
        /// Stops raising <see cref="ConfigurationSourceHandler.ConfigurationSourceChanged"/> events for 
        /// Changes in a subordinate section.
        /// </summary>
        /// <param name="sectionName">The name of the section events are propagated for.</param>
        protected void StopPropagatingSectionChangeEvents(string sectionName)
        {
            SectionInSubordinateSource sectionInSubordinate;
            if (sectionMappings.TryGetValue(sectionName, out sectionInSubordinate))
            {
                sectionMappings.Remove(sectionName);
                sectionInSubordinate.StopWatchingForSectionChangedEvent();
            }
        }

        /// <summary>
        /// Releases resources managed by this <see cref="ConfigurationSourceHandler"/> instance.
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
                if (configurationSource != null && configurationSourceChangedHandler != null)
                {
                    configurationSource.SourceChanged -= configurationSourceChangedHandler;
                }

                if (subordinateSourcesByName != null)
                {
                    foreach (SubordinateSource subOrdinateSource in subordinateSourcesByName.Values)
                    {
                        subOrdinateSource.Dispose();
                    }
                }
            }
        }

        private class SubordinateSource : IDisposable
        {
            string subordinateSourceName;
            IConfigurationSource subordinateConfigurationSource;
            ConfigurationSourceHandler compositeConfigurationSource;
            bool customSource;

            public SubordinateSource(ConfigurationSourceHandler compositeConfigurationSource, string subordinateSourceName, IConfigurationSource subordinateSource)
                : this(compositeConfigurationSource, subordinateSourceName, subordinateSource, false)
            {
            }

            public SubordinateSource(ConfigurationSourceHandler compositeConfigurationSource, string subordinateSourceName, IConfigurationSource subordinateSource, bool customSource)
            {
                this.subordinateSourceName = subordinateSourceName;
                this.subordinateConfigurationSource = subordinateSource;
                this.compositeConfigurationSource = compositeConfigurationSource;
                this.customSource = customSource;

                this.subordinateConfigurationSource.SourceChanged += new EventHandler<ConfigurationSourceChangedEventArgs>(compositeConfigurationSource_ConfigurationSourceChanged);
            }

            void compositeConfigurationSource_ConfigurationSourceChanged(object sender, ConfigurationSourceChangedEventArgs e)
            {
                compositeConfigurationSource.DoConfigurationSourceChanged(e.ChangedSectionNames);
            }

            public IConfigurationSource Source
            {
                get
                {
                    return subordinateConfigurationSource;
                }
            }

            public void Refresh()
            {
                if (customSource) return;
                if (subordinateConfigurationSource != null)
                {
                    subordinateConfigurationSource.SourceChanged -= compositeConfigurationSource_ConfigurationSourceChanged;
                    subordinateConfigurationSource.Dispose();
                }

                subordinateConfigurationSource = compositeConfigurationSource.CreateSubordinateSource(subordinateSourceName, false);
                if (subordinateConfigurationSource != null)
                {
                    this.subordinateConfigurationSource.SourceChanged += compositeConfigurationSource_ConfigurationSourceChanged;
                }
            }

            public void Dispose()
            {
                if (!customSource && subordinateConfigurationSource != null)
                {
                    subordinateConfigurationSource.Dispose();
                }
            }
        }

        private class SectionInSubordinateSource
        {
            string subordinateSourceName;
            string sectionName;
            IConfigurationSource subordinateSource;
            ConfigurationSourceHandler configurationSourceHandler;

            public SectionInSubordinateSource(ConfigurationSourceHandler configurationSourceHandler, string sectionName, string subordinateSourceName, IConfigurationSource subordinateSource)
            {
                this.sectionName = sectionName;
                this.subordinateSourceName = subordinateSourceName;
                this.configurationSourceHandler = configurationSourceHandler;
                Refresh(subordinateSource);
            }

            private void ChildConfigurationSectionChanged(object sender, ConfigurationChangedEventArgs args)
            {
                configurationSourceHandler.DoConfigurationSectionChanged(args.SectionName);
            }

            public string SectionName
            {
                get { return sectionName; }
            }

            public string SubordinateSourceName
            {
                get { return subordinateSourceName; }
            }

            public void Refresh(IConfigurationSource newSubordinateSource)
            {
                if (subordinateSource != null)
                {
                    subordinateSource.RemoveSectionChangeHandler(sectionName, ChildConfigurationSectionChanged);
                }

                subordinateSource = newSubordinateSource;
                subordinateSource.AddSectionChangeHandler(sectionName, ChildConfigurationSectionChanged);
            }

            public void StopWatchingForSectionChangedEvent()
            {
                subordinateSource.RemoveSectionChangeHandler(sectionName, ChildConfigurationSectionChanged);
            }

        }

        private class RecursionLock : IDisposable
        {
            [ThreadStatic]
            public static bool InsideHandlerOperation;

            public RecursionLock()
            {
                InsideHandlerOperation = true;
            }

            public void Dispose()
            {
                InsideHandlerOperation = false;
            }
        }
    }
}
