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
using System.Diagnostics;

namespace Transformalize.Libs.EnterpriseLibrary.Common.Instrumentation
{
    /// <summary>
    /// Factory for <see cref="EnterpriseLibraryPerformanceCounter"></see>s. Individual <see cref="PerformanceCounter"></see>
    /// instances are cached to prevent the same instance from being created multiple times.
    /// </summary>
    public class EnterpriseLibraryPerformanceCounterFactory
    {
        Dictionary<string, PerformanceCounter> counterCache = new Dictionary<string, PerformanceCounter>();
        object lockObject = new object();

        /// <summary>
        /// Creates an <see cref="EnterpriseLibraryPerformanceCounter"></see> initialized with individual <see cref="PerformanceCounter"></see>
        /// instances. Instances are named according to <paramref name="instanceNames"></paramref> passed to this method.
        /// </summary>
        /// <param name="categoryName">Performance counter category name, as defined during installation.</param>
        /// <param name="counterName">Performance counter name, as defined during installation.</param>
        /// <param name="instanceNames">Param array of instance names for which individual counters should be created.</param>
        /// <returns>The new counter instance.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase", Justification = "Not a roundtrip")]
        public EnterpriseLibraryPerformanceCounter CreateCounter(string categoryName, string counterName, string[] instanceNames)
        {
            if (instanceNames == null) throw new ArgumentNullException("instanceNames");

            string combinedCounterNameRoot = categoryName.ToLowerInvariant() + counterName.ToLowerInvariant();

            PerformanceCounter[] counters = new PerformanceCounter[instanceNames.Length];
            for (int i = 0; i < instanceNames.Length; i++)
            {
                string combinedCounterName = combinedCounterNameRoot + instanceNames[i].ToLowerInvariant();

                lock (lockObject)
                {
                    if (counterCache.ContainsKey(combinedCounterName) == false)
                    {
                        PerformanceCounter newCounter = new PerformanceCounter(categoryName, counterName, instanceNames[i], false);
                        counterCache.Add(combinedCounterName, newCounter);
                    }

                    counters[i] = counterCache[combinedCounterName];
                }
            }

            return new EnterpriseLibraryPerformanceCounter(counters);
        }

        /// <summary>
        /// This method supports the Enterprise Library infrastructure and is not intended to be used directly from your code.
        /// </summary>
        public void ClearCachedCounters()
        {
            counterCache.Clear();
        }
    }
}
