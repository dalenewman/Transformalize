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
using System.ComponentModel;
using System.Resources;

namespace Transformalize.Libs.EnterpriseLibrary.Common.Configuration.Design
{
    /// <summary>
    /// A customized version of <see cref="DescriptionAttribute"/> that can
    /// load the string from assembly resources instead of just a hard-wired
    /// string.
    /// </summary>
    public class ResourceDescriptionAttribute : DescriptionAttribute
    {
        private bool resourceLoaded;

        /// <summary>
        /// Create a new instance of <see cref="ResourceDescriptionAttribute"/> where
        /// the type and name of the resource is set via properties.
        /// </summary>
        public ResourceDescriptionAttribute()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceDescriptionAttribute"/> class.
        /// </summary>
        /// <param name="resourceType">Type used to locate the assembly containing the resources.</param>
        /// <param name="resourceName">Name of the entry in the resource table.</param>
        public ResourceDescriptionAttribute(Type resourceType, string resourceName)
        {
            ResourceType = resourceType;
            ResourceName = resourceName;
        }

        /// <summary>
        /// A type contained in the assembly we want to get our display name from.
        /// </summary>
        public Type ResourceType { get; set; }

        /// <summary>
        /// Name of the string resource containing our display name.
        /// </summary>
        public string ResourceName { get; set; }

        /// <summary>
        /// Gets the description for a property, event, or public void method that takes no arguments stored in this attribute.
        /// </summary>
        /// <returns>
        /// The display name.
        /// </returns>
        public override string Description
        {
            get
            {
                EnsureDescriptionLoaded();
                return DescriptionValue;
            }
        }

        private void EnsureDescriptionLoaded()
        {
            if (resourceLoaded) return;

            var rm = new ResourceManager(ResourceType);

            try
            {
                DescriptionValue = rm.GetString(ResourceName);
            }
            catch (MissingManifestResourceException)
            {
                DescriptionValue = ResourceName;
            }
            DescriptionValue = DescriptionValue == null ? String.Empty : DescriptionValue;

            resourceLoaded = true;
        }

    }
}
