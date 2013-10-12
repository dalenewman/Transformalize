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

namespace Transformalize.Libs.EnterpriseLibrary.Common.Configuration.Storage
{
    /// <summary>
    /// <para>Provides a way to watch for changes to configuration in storage.</para>
    /// </summary>
    public interface IConfigurationChangeWatcher : IDisposable
    {
        /// <summary>
        /// Event raised when the underlying persistence mechanism for configuration notices that
        /// the persistent representation of configuration information has changed.
        /// </summary>
        event ConfigurationChangedEventHandler ConfigurationChanged;

        /// <summary>
        /// When implemented by a subclass, starts the object watching for configuration changes
        /// </summary>
        void StartWatching();

        /// <summary>
        /// When implemented by a subclass, stops the object from watching for configuration changes
        /// </summary>
        void StopWatching();

        /// <summary>
        /// When implemented by a subclass, returns the section name that is being watched.
        /// </summary>
        string SectionName { get; }
    }
}
