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
    internal class ConfigurationChangeWatcherPauser : IDisposable
    {
        private readonly IConfigurationChangeWatcher watcher;

        public ConfigurationChangeWatcherPauser(IConfigurationChangeWatcher watcher)
        {
            this.watcher = watcher;
            if(watcher != null) watcher.StopWatching();
        }

        public void Dispose()
        {
            if(watcher != null) watcher.StartWatching();
        }
    }
}
