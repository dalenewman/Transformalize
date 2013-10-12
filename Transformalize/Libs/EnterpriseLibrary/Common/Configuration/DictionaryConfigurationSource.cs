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

namespace Transformalize.Libs.EnterpriseLibrary.Common.Configuration
{
    /// <summary>
    /// Represents a configuration source that is backed by a dictionary of named objects.
    /// </summary>
    public class DictionaryConfigurationSource : IConfigurationSource
    {
        /// <summary>
        /// This field supports the Enterprise Library infrastructure and is not intended to be used directly from your code.
        /// </summary>
        protected internal Dictionary<string, ConfigurationSection> sections;
        /// <summary>
        /// This field supports the Enterprise Library infrastructure and is not intended to be used directly from your code.
        /// </summary>
        protected internal EventHandlerList eventHandlers = new EventHandlerList();

        /// <summary>
        /// Raised when anything in the source changes.
        /// </summary>
        /// <remarks>
        /// <see cref="DictionaryConfigurationSource"/> does not report any
        /// configuration change events.
        /// </remarks>
        public event EventHandler<ConfigurationSourceChangedEventArgs> SourceChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="DictionaryConfigurationSource"/> class.
        /// </summary>
        public DictionaryConfigurationSource()
        {
            this.sections = new Dictionary<string, ConfigurationSection>();
        }

        /// <summary>
        /// Retrieves the specified <see cref="ConfigurationSection"/>.
        /// </summary>
        /// <param name="sectionName">The name of the section to be retrieved.</param>
        /// <returns>The specified <see cref="ConfigurationSection"/>, or <see langword="null"/> (<b>Nothing</b> in Visual Basic)
        /// if a section by that name is not found.</returns>
        public ConfigurationSection GetSection(string sectionName)
        {
            if (sections.ContainsKey(sectionName))
            {
                return sections[sectionName];
            }
            return null;
        }

        /// <summary>
        /// Adds a <see cref="ConfigurationSection"/> to the configuration source.
        /// </summary>
        /// <remarks>
        /// If a configuration section with the specified name already exists it will be replaced.
        /// </remarks>
        /// <param name="name">The name by which the <paramref name="section"/> should be added.</param>
        /// <param name="section">The configuration section to add.</param>
        public void Add(string name, ConfigurationSection section)
        {
            sections.Add(name, section);
        }

        /// <summary>
        /// Removes a <see cref="ConfigurationSection"/> from the configuration source.
        /// </summary>
        /// <param name="sectionName">The name of the section to remove.</param>
        public void Remove(string sectionName)
        {
            sections.Remove(sectionName);
        }

        /// <summary>
        /// Determines if a section name exists in the source.
        /// </summary>
        /// <param name="name">The section name to find.</param>
        /// <returns><b>true</b> if the section exists; otherwise, <b>false</b>.</returns>
        public bool Contains(string name)
        {
            return sections.ContainsKey(name);
        }

        /// <summary>
        /// Adds a handler to be called when changes to the section named <paramref name="sectionName"/> are detected.
        /// </summary>
        /// <param name="sectionName">The name of the section to watch for.</param>
        /// <param name="handler">The handler for the change event to add.</param>
        public void AddSectionChangeHandler(string sectionName, ConfigurationChangedEventHandler handler)
        {
            eventHandlers.AddHandler(sectionName, handler);
        }

        /// <summary>
        /// Removes a handler to be called when changes to section <code>sectionName</code> are detected.
        /// </summary>
        /// <param name="sectionName">The name of the watched section.</param>
        /// <param name="handler">The handler for the change event to remove.</param>
        public void RemoveSectionChangeHandler(string sectionName, ConfigurationChangedEventHandler handler)
        {
            eventHandlers.RemoveHandler(sectionName, handler);
        }

        /// <summary>
        /// Raises the <see cref="SourceChanged"/> event.
        /// </summary>
        /// <param name="args">Event arguments</param>
        protected void OnSourceChangedEvent(ConfigurationSourceChangedEventArgs args)
        {
            var handler = this.SourceChanged;
            if (handler != null)
            {
                handler(this, args);
            }
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
                this.eventHandlers.Dispose();
            }
        }
    }
}
