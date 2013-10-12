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

namespace Transformalize.Libs.EnterpriseLibrary.Common.Configuration.Design
{
    /// <summary>
    /// Attribute class used to associate a <see cref="System.Configuration.ConfigurationElementCollection"/> class with an <see cref="IMergeableConfigurationElementCollection"/> implementation.
    /// </summary>
    /// <seealso cref="IMergeableConfigurationElementCollection"/>
    [AttributeUsage(AttributeTargets.Class)]
    public class MergeableConfigurationCollectionTypeAttribute : Attribute
    {
        private readonly Type mergeableConfigurationCollectionType;

        /// <summary>
        /// Creates a new instance of the <see cref="MergeableConfigurationCollectionTypeAttribute"/> class.
        /// </summary>
        /// <param name="mergeableConfigurationCollectionType">The type of <see cref="IMergeableConfigurationElementCollection"/> that should be associated with the target <see cref="System.Configuration.ConfigurationElementCollection"/> class.</param>
        public MergeableConfigurationCollectionTypeAttribute(Type mergeableConfigurationCollectionType)
        {
            this.mergeableConfigurationCollectionType = mergeableConfigurationCollectionType;
        }

        /// <summary>
        /// Gets the type of <see cref="IMergeableConfigurationElementCollection"/> that should be associated with the target <see cref="System.Configuration.ConfigurationElementCollection"/> class.
        /// </summary>
        /// <value>
        /// The type of <see cref="IMergeableConfigurationElementCollection"/> that should be associated with the target <see cref="System.Configuration.ConfigurationElementCollection"/> class.
        /// </value>
        public Type MergeableConfigurationCollectionType
        {
            get { return mergeableConfigurationCollectionType; }
        }
    }
}
