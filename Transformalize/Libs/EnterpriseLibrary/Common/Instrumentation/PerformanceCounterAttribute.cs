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
using System.Diagnostics;

namespace Transformalize.Libs.EnterpriseLibrary.Common.Instrumentation
{
    /// <summary>
    /// Defines a <see cref="PerformanceCounter"></see>. Used by the reflection-based installers to 
    /// prepare a performance counter for installation.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class PerformanceCounterAttribute : Attribute
    {
        private string counterName;
        private string counterHelp;
        private PerformanceCounterType counterType;

        private string baseCounterName;
        private string baseCounterHelp;
        private PerformanceCounterType baseCounterType;

        /// <summary>
        /// Initializes this object with all data needed to install a <see cref="PerformanceCounter"></see>.
        /// </summary>
        /// <param name="counterName">Performance counter name.</param>
        /// <param name="counterHelp">Name of Help resource string. This is not the help text itself, 
        /// but is the resource name used to look up the internationalized help text at install-time.</param>
        /// <param name="counterType">Performance Counter type.</param>
        public PerformanceCounterAttribute(string counterName, string counterHelp, PerformanceCounterType counterType)
        {
            this.counterName = counterName;
            this.counterHelp = counterHelp;
            this.counterType = counterType;
        }

        /// <summary>
        /// Gets the <see cref="PerformanceCounter"></see> type.
        /// </summary>
        public PerformanceCounterType CounterType
        {
            get { return counterType; }
        }

        /// <summary>
        /// Get the name of Help resource string. This is not the help text itself, 
        /// but is the resource name used to look up the internationalized help text at install-time.
        /// </summary>
        public string CounterHelp
        {
            get { return counterHelp; }
        }

        /// <summary>
        /// Gets the <see cref="PerformanceCounter"></see> name.
        /// </summary>
        public string CounterName
        {
            get { return counterName; }
        }

        /// <summary>
        /// Gets and sets the base <see cref="PerformanceCounter"></see> type. This is an optional 
        /// property used when the counter being defined requires a base counter to operate, such as for 
        /// averages.
        /// </summary>
        public PerformanceCounterType BaseCounterType
        {
            get { return baseCounterType; }
            set { baseCounterType = value; }
        }

        /// <summary>
        /// Gets and sets the base <see cref="PerformanceCounter"></see> help resource name. 
        /// This is not the help text itself, 
        /// but is the resource name used to look up the internationalized help text at install-time.
        /// This is an optional 
        /// property used when the counter being defined requires a base counter to operate, such as for 
        /// averages.
        /// </summary>
        public string BaseCounterHelp
        {
            get { return baseCounterHelp; }
            set { baseCounterHelp = value; }
        }

        /// <summary>
        /// Gets and sets the base <see cref="PerformanceCounter"></see> name. This is an optional 
        /// property used when the counter being defined requires a base counter to operate, such as for 
        /// averages.
        /// </summary>
        public string BaseCounterName
        {
            get { return baseCounterName; }
            set { baseCounterName = value; }
        }

        /// <summary>
        /// Used to determine if the counter being installed has a base counter associated with it.
        /// </summary>
        /// <returns>True if counter being installed has a base counter associated with it.</returns>
        public bool HasBaseCounter() { return baseCounterName != null; }
    }
}
