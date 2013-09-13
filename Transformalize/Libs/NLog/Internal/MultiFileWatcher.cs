#region License

// /*
// Transformalize - Replicate, Transform, and Denormalize Your Data...
// Copyright (C) 2013 Dale Newman
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
// */

#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Transformalize.Libs.NLog.Common;

#if !NET_CF && !SILVERLIGHT

namespace Transformalize.Libs.NLog.Internal
{
    /// <summary>
    ///     Watches multiple files at the same time and raises an event whenever
    ///     a single change is detected in any of those files.
    /// </summary>
    internal class MultiFileWatcher : IDisposable
    {
        private readonly List<FileSystemWatcher> watchers = new List<FileSystemWatcher>();

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            StopWatching();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Occurs when a change is detected in one of the monitored files.
        /// </summary>
        public event EventHandler OnChange;

        /// <summary>
        ///     Stops the watching.
        /// </summary>
        public void StopWatching()
        {
            lock (this)
            {
                foreach (var watcher in watchers)
                {
                    InternalLogger.Info("Stopping file watching for path '{0}' filter '{1}'", watcher.Path, watcher.Filter);
                    watcher.EnableRaisingEvents = false;
                    watcher.Dispose();
                }

                watchers.Clear();
            }
        }

        /// <summary>
        ///     Watches the specified files for changes.
        /// </summary>
        /// <param name="fileNames">The file names.</param>
        public void Watch(IEnumerable<string> fileNames)
        {
            if (fileNames == null)
            {
                return;
            }

            foreach (var s in fileNames)
            {
                Watch(s);
            }
        }

        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Watcher is released in Dispose()")]
        internal void Watch(string fileName)
        {
            var watcher = new FileSystemWatcher
                              {
                                  Path = Path.GetDirectoryName(fileName),
                                  Filter = Path.GetFileName(fileName),
                                  NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.CreationTime | NotifyFilters.Size | NotifyFilters.Security | NotifyFilters.Attributes
                              };

            watcher.Created += OnWatcherChanged;
            watcher.Changed += OnWatcherChanged;
            watcher.Deleted += OnWatcherChanged;
            watcher.EnableRaisingEvents = true;
            InternalLogger.Info("Watching path '{0}' filter '{1}' for changes.", watcher.Path, watcher.Filter);

            lock (this)
            {
                watchers.Add(watcher);
            }
        }

        private void OnWatcherChanged(object source, FileSystemEventArgs e)
        {
            lock (this)
            {
                if (OnChange != null)
                {
                    OnChange(source, e);
                }
            }
        }
    }
}

#endif