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

using System.Diagnostics;

namespace Transformalize.Libs.EnterpriseLibrary.Common.Instrumentation
{
    /// <summary>
    /// Constructs an instance name for a <see cref="PerformanceCounter"></see> following embedded
    /// formatting rules.
    /// </summary>
    public class PerformanceCounterInstanceName
    {
        const int MaxPrefixLength = 32;
        const int MaxSuffixLength = 32;

        readonly string prefix;
        readonly string suffix;

        /// <overloads>
        /// Initializes this object with information needed to construct a <see cref="PerformanceCounter"></see>\
        /// instance name.
        /// </overloads>
        /// <summary>
        /// Initializes this object with information needed to construct a <see cref="PerformanceCounter"></see>\
        /// instance name.
        /// </summary>
        /// <param name="prefix">Counter name prefix.</param>
        /// <param name="suffix">Counter name suffix.</param>
        public PerformanceCounterInstanceName(string prefix,
                                              string suffix)
            : this(prefix, suffix, MaxPrefixLength, MaxSuffixLength) {}

        /// <overloads>
        /// Initializes this object with information needed to construct a <see cref="PerformanceCounter"></see>\
        /// instance name.
        /// </overloads>
        /// <summary>
        /// Initializes this object with information needed to construct a <see cref="PerformanceCounter"></see>\
        /// instance name.
        /// </summary>
        /// <param name="prefix">Counter name prefix.</param>
        /// <param name="suffix">Counter name suffix.</param>
        /// <param name="maxPrefixLength">Max prefix length.</param>
        /// <param name="maxSuffixLength">Max suffix length.</param>
        public PerformanceCounterInstanceName(string prefix,
                                              string suffix,
                                              int maxPrefixLength,
                                              int maxSuffixLength)
        {
            this.prefix = NormalizeStringLength(prefix, maxPrefixLength);
            this.suffix = NormalizeStringLength(suffix, maxSuffixLength);
        }

        static string NormalizeStringLength(string namePart,
                                            int namePartMaxLength)
        {
            return (namePart.Length > namePartMaxLength) ? namePart.Substring(0, namePartMaxLength) : namePart;
        }

        /// <summary>
        /// Returns properly formatted counter name as a string.
        /// </summary>
        /// <returns>Formatted counter name.</returns>
        public override string ToString()
        {
            string namePrefix = "";
            if (prefix.Length > 0) namePrefix += (prefix + " - ");
            return namePrefix + suffix;
        }
    }
}
