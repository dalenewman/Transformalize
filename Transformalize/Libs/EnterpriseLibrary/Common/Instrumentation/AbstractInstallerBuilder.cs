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
using System.Security;

namespace Transformalize.Libs.EnterpriseLibrary.Common.Instrumentation
{
    /// <summary>
    /// Base class for the reflection-based installers. These reflection-based installers 
    /// search through assemblies looking for the appropriate kinds of installable resources and
    /// arrange for them to be registered with the appropriate installer
    /// </summary>
    [SecurityCritical]
    public abstract class AbstractInstallerBuilder
    {
        IList<Type> instrumentedTypes;

        /// <summary>
        /// Fills the given installer with other, more specific kinds of installers that have been
        /// filled with the appropriate kinds of installable resources
        /// </summary>
        /// <param name="installer">Outer installer to be filled with nested installers for specific resources</param>
        public void Fill(Installer installer)
        {
            ICollection<Installer> installers = CreateInstallers(InstrumentedTypes);

            foreach (Installer childInstaller in installers)
            {
                installer.Installers.Add(childInstaller);
            }
        }

        /// <summary>
        /// Gets or sets a list of all instrumentented types found in a given assembly. Types are instrumented if they are
        /// attributed with <see cref="HasInstallableResourcesAttribute"></see>	 and another attribute specifying 
        /// another, more specific resource type.
        /// </summary>
        protected IList<Type> InstrumentedTypes
        {
            get { return instrumentedTypes; }
            set { instrumentedTypes = value; }
        }

        /// <summary>
        /// Initializes object by giving it access to an array of all available types and a specification of
        /// the more specific resource type that will be installed.
        /// </summary>
        /// <param name="availableTypes">Array of available types through which installer should look</param>
        /// <param name="instrumentationAttributeType">Attribute specifying the more specific resource type to search for</param>
        protected AbstractInstallerBuilder(Type[] availableTypes, Type instrumentationAttributeType)
        {
            this.instrumentedTypes
                = FindInstrumentedTypes(availableTypes, instrumentationAttributeType);
        }

        /// <summary>
        /// Helper method to determine if the given type is annotated with the required attribute.
        /// </summary>
        /// <param name="instrumentedType">Type in question</param>
        /// <param name="attributeType">More specific attribute used to match resource being installed</param>
        /// <returns>True if the attributes on the given <paramref name="instrumentedType"></paramref> matches <paramref name="attributeType"></paramref></returns>
        protected bool ConfirmAttributeExists(Type instrumentedType, Type attributeType)
        {
            if (instrumentedType == null) throw new ArgumentNullException("instrumentedType");

            object[] attributesFound = instrumentedType.GetCustomAttributes(attributeType, false);
            return attributesFound.Length == 0 ? false : true;
        }

        /// <summary>
        /// Helper method to determine if the attributes for a given type match the attributes used to 
        /// specify a specific kind of installable resource. The type should be attributed with <see cref="HasInstallableResourcesAttribute"></see>
        /// and the attribute passed to this method call.
        /// </summary>
        /// <param name="instrumentedType">Type in question</param>
        /// <param name="instrumentedAttributeType">More specific attribute used to match resource being installed</param>
        /// <returns><b>true</b> if the type specifies intallable resources.</returns>
        protected bool IsInstrumented(Type instrumentedType, Type instrumentedAttributeType)
        {
            if (instrumentedType == null) return false;

            Type attributeType = typeof(HasInstallableResourcesAttribute);
            if (ConfirmAttributeExists(instrumentedType, typeof(HasInstallableResourcesAttribute)) &&
               (ConfirmAttributeExists(instrumentedType, instrumentedAttributeType)))
                return true;
            else
                return false;
        }

        private Type[] FindInstrumentedTypes(Type[] reflectableTypes, Type instrumentedAttributeType)
        {
            List<Type> instrumentedTypes = new List<Type>();
            foreach (Type type in reflectableTypes)
            {
                if (IsInstrumented(type, instrumentedAttributeType))
                {
                    instrumentedTypes.Add(type);
                }
            }

            return instrumentedTypes.ToArray();
        }

        /// <summary>
        /// Creates one or more installers after iterating over the <paramref name="instrumentedTypes"></paramref>.
        /// The number of iterators returned depends on the specific needs of the particular installable type.
        /// </summary>
        /// <returns>Collection of installers created through iterating over included types</returns>
        protected abstract ICollection<Installer> CreateInstallers(ICollection<Type> instrumentedTypes);
    }
}
