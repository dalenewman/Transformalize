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
using System.Configuration.Install;
using System.Diagnostics;
using System.Security;

namespace Transformalize.Libs.EnterpriseLibrary.Common.Instrumentation
{
    /// <summary>
    /// Add event log source definitions for classes that have been attributed
    /// with HasInstallableResourceAttribute and EventLogDefinition attributes to EventLogInstallers.
    /// One installer is created for each unique event log source that is found.
    /// </summary>
    [SecurityCritical]
    public class EventLogInstallerBuilder : AbstractInstallerBuilder
    {
        /// <summary>
        /// Initializes this object with a list of types that may potentially be attributed appropriately.
        /// </summary>
        /// <param name="potentialTypes">Array of types to inspect check for event log sources needing installation</param>
        public EventLogInstallerBuilder(Type[] potentialTypes)
            : base(potentialTypes, typeof(EventLogDefinitionAttribute))
        {
        }

        /// <summary>
        /// Creates <see cref="EventLogInstaller"></see> instances for each separate event log source needing installation.
        /// </summary>
        /// <param name="instrumentedTypes">Collection of <see cref="Type"></see>s that represent types defining
        /// event log sources to be installed.</param>
        /// <returns>Collection of installers containing event log sources to be installed.</returns>
        [SecurityCritical]
        protected override ICollection<Installer> CreateInstallers(ICollection<Type> instrumentedTypes)
        {
            IList<Installer> installers = new List<Installer>();

            foreach (Type instrumentedType in instrumentedTypes)
            {
                EventLogDefinitionAttribute attribute
                    = (EventLogDefinitionAttribute)instrumentedType.GetCustomAttributes(typeof(EventLogDefinitionAttribute), false)[0];

                EventLogInstaller installer = new EventLogInstaller();
                installer.Log = attribute.LogName;
                installer.Source = attribute.SourceName;
                installer.CategoryCount = attribute.CategoryCount;
                if (attribute.CategoryResourceFile != null) installer.CategoryResourceFile = attribute.CategoryResourceFile;
                if (attribute.MessageResourceFile != null) installer.MessageResourceFile = attribute.MessageResourceFile;
                if (attribute.ParameterResourceFile != null) installer.ParameterResourceFile = attribute.ParameterResourceFile;

                installers.Add(installer);
            }

            return installers;
        }
    }
}
