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
using System.Text.RegularExpressions;

namespace Transformalize.Libs.EnterpriseLibrary.Common.Instrumentation
{
    /// <summary>
	/// Provides the friendly name of the application domain as the prefix in formatting a 
	/// particular instance of a performance counter.
	/// </summary>
    public class AppDomainNameFormatter : IPerformanceCounterNameFormatter
    {
        private string applicationInstanceName;
        private const string InvalidCharacters = @"\()/#*";

        /// <summary>
        /// Creates an instance of the <see cref="AppDomainNameFormatter"/>
        /// </summary>
        public AppDomainNameFormatter()
        {
        }
        
        /// <summary>
        /// Creates an instance of the <see cref="AppDomainNameFormatter"/> with an Application Instance Name
        /// </summary>
        /// <param name="applicationInstanceName"></param>
        public AppDomainNameFormatter(string applicationInstanceName)
        {
            this.applicationInstanceName = applicationInstanceName;
        }

        /// <summary>
		/// Creates the formatted instance name for a performance counter, providing the Application
		/// Domain friendly name for the prefix for the instance.
		/// </summary>
		/// <param name="nameSuffix">Performance counter name, as defined during installation of the counter</param>
		/// <returns>Formatted instance name in form of "appDomainFriendlyName - nameSuffix"</returns>
		public string CreateName(string nameSuffix)
        {
            string replacePattern = "[\\\\()#/\\*]*";
            string appDomainFriendlyName = string.IsNullOrEmpty(this.applicationInstanceName) ? AppDomain.CurrentDomain.FriendlyName : this.applicationInstanceName;

            Regex filter = new Regex(replacePattern);
            appDomainFriendlyName = filter.Replace(appDomainFriendlyName, string.Empty);

			PerformanceCounterInstanceName instanceName = new PerformanceCounterInstanceName(appDomainFriendlyName, nameSuffix);
			return instanceName.ToString();        	
        }
    }
}
