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
using System.Collections.ObjectModel;
using System.Linq;

namespace Transformalize.Libs.EnterpriseLibrary.Common.Configuration
{
    /// <summary>
    /// Event arguments describing which sections have changed in a configuration source.
    /// </summary>
    public class ConfigurationSourceChangedEventArgs : EventArgs
    {
        private readonly IConfigurationSource configurationSource;
        private readonly ReadOnlyCollection<string> changedSectionNames;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationSourceChangedEventArgs"/> class.
        /// </summary>
        /// <param name="configurationSource">Configuration source that changed.</param>
        /// <param name="changedSectionNames">Sequence of the section names in <paramref name="configurationSource"/>
        /// that have changed.</param>
        public ConfigurationSourceChangedEventArgs(IConfigurationSource configurationSource,
            IEnumerable<string> changedSectionNames)
        {
            this.configurationSource = configurationSource;
            this.changedSectionNames = new ReadOnlyCollection<string>(changedSectionNames.ToArray());
        }

        /// <summary>
        /// The configuration source that has changed.
        /// </summary>
        public IConfigurationSource ConfigurationSource
        {
            get { return configurationSource; }
        }

        /// <summary>
        /// The set of section names that have changed.
        /// </summary>
        public ReadOnlyCollection<string> ChangedSectionNames
        {
            get { return changedSectionNames; }
        }
    }
}
