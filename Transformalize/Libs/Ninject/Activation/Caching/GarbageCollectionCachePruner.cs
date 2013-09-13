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
using System.Threading;
using Transformalize.Libs.Ninject.Components;
using Transformalize.Libs.Ninject.Infrastructure;
using Transformalize.Libs.Ninject.Infrastructure.Language;

namespace Transformalize.Libs.Ninject.Activation.Caching
{
    /// <summary>
    ///     Uses a <see cref="Timer" /> and some <see cref="WeakReference" /> magic to poll
    ///     the garbage collector to see if it has run.
    /// </summary>
    public class GarbageCollectionCachePruner : NinjectComponent, ICachePruner
    {
        /// <summary>
        ///     The caches that are being pruned.
        /// </summary>
        private readonly List<IPruneable> caches = new List<IPruneable>();

        /// <summary>
        ///     indicator for if GC has been run.
        /// </summary>
        private readonly WeakReference indicator = new WeakReference(new object());

        private bool stop;

        /// <summary>
        ///     The timer used to trigger the cache pruning
        /// </summary>
        private Timer timer;

        /// <summary>
        ///     Starts pruning the specified pruneable based on the rules of the pruner.
        /// </summary>
        /// <param name="pruneable">The pruneable that will be pruned.</param>
        public void Start(IPruneable pruneable)
        {
            Ensure.ArgumentNotNull(pruneable, "pruneable");

            caches.Add(pruneable);
            if (timer == null)
            {
                timer = new Timer(PruneCacheIfGarbageCollectorHasRun, null, GetTimeoutInMilliseconds(), Timeout.Infinite);
            }
        }

        /// <summary>
        ///     Stops pruning.
        /// </summary>
        public void Stop()
        {
            lock (this)
            {
                stop = true;
            }

            using (var signal = new ManualResetEvent(false))
            {
#if !NETCF
                timer.Dispose(signal);
                signal.WaitOne();
#else
                this.timer.Dispose();
#endif

                timer = null;
                caches.Clear();
            }
        }

        /// <summary>
        ///     Releases resources held by the object.
        /// </summary>
        public override void Dispose(bool disposing)
        {
            if (disposing && !IsDisposed && timer != null)
            {
                Stop();
            }

            base.Dispose(disposing);
        }

        private void PruneCacheIfGarbageCollectorHasRun(object state)
        {
            lock (this)
            {
                if (stop)
                {
                    return;
                }

                try
                {
                    if (indicator.IsAlive)
                    {
                        return;
                    }

                    caches.Map(cache => cache.Prune());
                    indicator.Target = new object();
                }
                finally
                {
                    timer.Change(GetTimeoutInMilliseconds(), Timeout.Infinite);
                }
            }
        }

        private int GetTimeoutInMilliseconds()
        {
            var interval = Settings.CachePruningInterval;
            return interval == TimeSpan.MaxValue ? -1 : (int) interval.TotalMilliseconds;
        }
    }
}