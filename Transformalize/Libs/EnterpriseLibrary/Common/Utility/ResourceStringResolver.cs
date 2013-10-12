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

namespace Transformalize.Libs.EnterpriseLibrary.Common.Utility
{
    /// <summary>
    /// Resolves strings by retrieving them from assembly resources, falling back to a specified
    /// value.
    /// </summary>
    /// <remarks>
    /// If both the resource type and the resource name are available, a resource lookup will be 
    /// performed; otherwise, the default value will be returned.
    /// </remarks>
    public sealed class ResourceStringResolver : IStringResolver
    {
        /// <summary>
        /// Initializes a new instance of <see cref="ResourceStringResolver"/>
        /// for a resource type, a resource name and a fallback value.
        /// </summary>
        /// <param name="resourceType">The type that identifies the resources file.</param>
        /// <param name="resourceName">The name of the resource.</param>
        /// <param name="fallbackValue">The fallback value, to use when any of the resource
        /// identifiers is not available.</param>
        public ResourceStringResolver(Type resourceType, string resourceName, string fallbackValue)
        {
            this.resourceType = resourceType;
            this.resourceName = resourceName;
            this.fallbackValue = fallbackValue;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="ResourceStringResolver"/>
        /// for a resource type name, a resource name and a fallback value.
        /// </summary>
        /// <param name="resourceTypeName">The name of the type that identifies the resources file.</param>
        /// <param name="resourceName">The name of the resource.</param>
        /// <param name="fallbackValue">The fallback value, to use when any of the resource
        /// identifiers is not available.</param>
        public ResourceStringResolver(string resourceTypeName, string resourceName, string fallbackValue)
            : this(LoadType(resourceTypeName), resourceName, fallbackValue)
        { }

        private static Type LoadType(string resourceTypeName)
        {
            return Type.GetType(resourceTypeName, false);
        }

        private readonly Type resourceType;
        private readonly string resourceName;
        private readonly string fallbackValue;

        string IStringResolver.GetString()
        {
            string value;

            if (!(this.resourceType == null || string.IsNullOrEmpty(this.resourceName)))
            {
                value
                    = ResourceStringLoader.LoadString(
                        this.resourceType.FullName,
                        this.resourceName,
                        this.resourceType.Assembly);
            }
            else
            {
                value = this.fallbackValue;
            }

            return value;
        }
    }
}
