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
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Security;

namespace Transformalize.Libs.EnterpriseLibrary.Common.Instrumentation
{
    /// <summary>
    /// Add event log source definitions for classes that have been attributed
    /// with HasInstallableResourceAttribute and EventLogDefinition attributes to EventLogInstallers.
    /// One installer is created for each unique performance counter category that is found.
    /// </summary>
    [SecurityCritical]
    public class PerformanceCounterInstallerBuilder : AbstractInstallerBuilder
    {
        /// <summary>
        /// Initializes this object with a list of <see cref="Type"></see>s that may potentially be attributed appropriately.
        /// </summary>
        /// <param name="availableTypes">Array of types to inspect check for performance counter definitions needing installation</param>
        public PerformanceCounterInstallerBuilder(Type[] availableTypes)
            : base(availableTypes, typeof(PerformanceCountersDefinitionAttribute)) { }

        void CollectPerformanceCounters(Type instrumentedType,
                                        PerformanceCounterInstaller installer)
        {
            foreach (FieldInfo field in instrumentedType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                object[] attributes = field.GetCustomAttributes(typeof(PerformanceCounterAttribute), false);
                if (attributes.Length == 1)
                {
                    PerformanceCounterAttribute attribute = (PerformanceCounterAttribute)attributes[0];

                    CounterCreationData counter = GetExistingCounter(installer, attribute.CounterName);
                    if (counter == null)
                    {
                        installer.Counters.Add(
                            new CounterCreationData(attribute.CounterName, GetCounterHelp(attribute.CounterHelp, instrumentedType.Assembly), attribute.CounterType));
                        if (attribute.HasBaseCounter())
                        {
                            installer.Counters.Add(
                                new CounterCreationData(attribute.BaseCounterName, GetCounterHelp(attribute.BaseCounterHelp, instrumentedType.Assembly), attribute.BaseCounterType));
                        }
                    }
                    else
                    {
                        if (counter.CounterType != attribute.CounterType || !counter.CounterHelp.Equals(GetCounterHelp(attribute.CounterHelp, instrumentedType.Assembly)))
                        {
                            throw new InvalidOperationException(
                                string.Format(
                                    CultureInfo.CurrentCulture,
                                    Resources.ExceptionPerformanceCounterRedefined,
                                    counter.CounterName,
                                    installer.CategoryName,
                                    instrumentedType.FullName));
                        }

                        // ignore new definition if equal
                    }
                }
            }
        }

        /// <summary>
        /// Creates <see cref="PerformanceCounterInstaller"></see> instances for each separate performance counter definition needing installation.
        /// </summary>
        /// <param name="instrumentedTypes">Collection of <see cref="Type"></see>s that represent types defining
        /// performance counter definitions to be installed.</param>
        /// <returns>Collection of installers containing performance counter definitions to be installed.</returns>
        [SecurityCritical]
        protected override ICollection<Installer> CreateInstallers(ICollection<Type> instrumentedTypes)
        {
            List<Installer> installers = new List<Installer>();

            foreach (Type instrumentedType in instrumentedTypes)
            {
                PerformanceCounterInstaller installer = GetOrCreateInstaller(instrumentedType, installers);
                CollectPerformanceCounters(instrumentedType, installer);
            }

            return installers;
        }

        /// <summary>
        /// Gets the category help for a performance counter.
        /// </summary>
        /// <param name="attribute">The performance counter attribute.</param>
        /// <param name="originalAssembly">The assembly where the help is defined.</param>
        /// <returns>The help for the performance counter.</returns>
        public static string GetCategoryHelp(PerformanceCountersDefinitionAttribute attribute,
                                             Assembly originalAssembly)
        {
            if (attribute == null) throw new ArgumentNullException("attribute");

            return GetResourceString(attribute.CategoryHelp, originalAssembly);
        }

        /// <summary>
        /// Gets the counter help for a performance counter.
        /// </summary>
        /// <param name="resourceName">The resource name.</param>
        /// <param name="originalAssembly">The assembly where the help is defined.</param>
        /// <returns>The help for the performance counter.</returns>
        public static string GetCounterHelp(string resourceName,
                                            Assembly originalAssembly)
        {
            return GetResourceString(resourceName, originalAssembly);
        }

        static CounterCreationData GetExistingCounter(PerformanceCounterInstaller installer,
                                               string counterName)
        {
            foreach (CounterCreationData counter in installer.Counters)
            {
                if (counter.CounterName.Equals(counterName, StringComparison.CurrentCulture))
                    return counter;
            }
            return null;
        }

        static PerformanceCounterInstaller GetExistingInstaller(string categoryName, IEnumerable<Installer> installers)
        {
            foreach (PerformanceCounterInstaller installer in installers)
            {
                if (installer.CategoryName.Equals(categoryName, StringComparison.CurrentCulture))
                    return installer;
            }
            return null;
        }

        PerformanceCounterInstaller GetOrCreateInstaller(Type instrumentedType,
                                                         ICollection<Installer> installers)
        {
            PerformanceCountersDefinitionAttribute attribute
                = (PerformanceCountersDefinitionAttribute)instrumentedType.GetCustomAttributes(typeof(PerformanceCountersDefinitionAttribute), false)[0];

            PerformanceCounterInstaller installer = GetExistingInstaller(attribute.CategoryName, installers);
            if (installer == null)
            {
                installer = new PerformanceCounterInstaller();
                PopulateCounterCategoryData(attribute, instrumentedType.Assembly, installer);
                installers.Add(installer);
            }

            return installer;
        }

        static string GetResourceString(string name,
                                        Assembly originalAssembly)
        {
            string translatedHelpString = null;

            string[] resourceNames = originalAssembly.GetManifestResourceNames();
            for (int i = 0; i < resourceNames.Length; i++)
            {
                try
                {
                    int lastDotResourcesString = resourceNames[i].LastIndexOf(".resources", StringComparison.OrdinalIgnoreCase);
                    string resourceName = resourceNames[i].Remove(lastDotResourcesString);
                    ResourceManager manager = new ResourceManager(resourceName, originalAssembly);
                    translatedHelpString = manager.GetString(name);
                }
                catch (Exception)
                {
                    // There is nothing we can do if this doesn't find the resource string. The only harm is that
                    // the help text that is registered will  not be as helpful as could otherwise be. Not worth 
                    // failing the entire installation process over.
                }

                if (!string.IsNullOrEmpty(translatedHelpString))
                    return translatedHelpString;
            }

            return "";
        }

        void PopulateCounterCategoryData(PerformanceCountersDefinitionAttribute attribute,
                                         Assembly originalAssembly,
                                         PerformanceCounterInstaller installer)
        {
            installer.CategoryName = attribute.CategoryName;
            installer.CategoryHelp = GetCategoryHelp(attribute, originalAssembly);

            installer.CategoryType = attribute.CategoryType;
        }
    }
}
