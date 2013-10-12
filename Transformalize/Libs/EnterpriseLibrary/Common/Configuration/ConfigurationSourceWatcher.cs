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

using System.Collections.Generic;
using Transformalize.Libs.EnterpriseLibrary.Common.Configuration.Storage;

namespace Transformalize.Libs.EnterpriseLibrary.Common.Configuration
{
    /// <summary>
    /// Reacts to changes on the medium on which a set of configuration sections are serialized.
    /// </summary>
    public abstract class ConfigurationSourceWatcher
    {
        private string configurationSource;
        private IList<string> watchedSections;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationSourceWatcher"/> class.
        /// </summary>
        /// <param name="configSource">The identification of the medium.</param>
        /// <param name="refresh"><b>true</b> if changes should be notified, <b>false</b> otherwise.</param>
        /// <param name="changed">The callback for changes notification.</param>
        protected ConfigurationSourceWatcher(string configSource, bool refresh, ConfigurationChangedEventHandler changed)
        {
            this.configurationSource = configSource;
            this.watchedSections = new List<string>();
        }

        /// <summary>
        /// Gets or sets the identification of the medium where the watched set of configuration sections is stored.
        /// </summary>
        public string ConfigSource
        {
            get { return configurationSource; }
            set { configurationSource = value; }
        }

        /// <summary>
        /// Gets or sets the collection of watched sections.
        /// </summary>
        public IList<string> WatchedSections
        {
            get { return watchedSections; }
            set { watchedSections = value; }
        }

        /// <summary>
        /// Starts watching for changes on the serialization medium.
        /// </summary>
        public void StartWatching()
        {
            if (this.Watcher != null)
            {
                this.Watcher.StartWatching();
            }
        }

        /// <summary>
        /// Stops watching for changes on the serialization medium.
        /// </summary>
        public void StopWatching()
        {
            if (this.Watcher != null)
            {
                this.Watcher.StopWatching();
            }
        }

        /// <summary>
        /// Gets the watcher over the serialization medium.
        /// </summary>
        public abstract ConfigurationChangeWatcher Watcher { get; }
    }
}
