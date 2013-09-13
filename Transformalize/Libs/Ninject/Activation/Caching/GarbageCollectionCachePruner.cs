#region License

// 
// Author: Nate Kohari <nate@enkari.com>
// Copyright (c) 2007-2010, Enkari, Ltd.
// 
// Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
// See the file LICENSE.txt for details.
// 

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
            TimeSpan interval = Settings.CachePruningInterval;
            return interval == TimeSpan.MaxValue ? -1 : (int) interval.TotalMilliseconds;
        }
    }
}