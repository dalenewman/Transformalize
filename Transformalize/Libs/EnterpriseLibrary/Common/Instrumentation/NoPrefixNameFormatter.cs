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

namespace Transformalize.Libs.EnterpriseLibrary.Common.Instrumentation
{
    /// <summary>
    /// Provides a pluggable way to format the name given to a particular instance of a performance counter.
    /// This class does no formatting, returning the provided name suffix as the counter name.
    /// </summary>
    public class NoPrefixNameFormatter : IPerformanceCounterNameFormatter
    {
        /// <summary>
        /// Returns the given <paramref name="nameSuffix"></paramref> as the created name.
        /// </summary>
        /// <param name="nameSuffix">Performance counter name, as defined during installation of the counter</param>
        /// <returns>Formatted instance name in form of "<paramref name="nameSuffix"/>"</returns>
        public string CreateName(string nameSuffix)
        {
            return new PerformanceCounterInstanceName("", nameSuffix).ToString();
        }
    }
}
