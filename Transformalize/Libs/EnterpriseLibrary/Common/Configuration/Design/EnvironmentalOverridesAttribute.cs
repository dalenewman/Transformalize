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

namespace Transformalize.Libs.EnterpriseLibrary.Common.Configuration.Design
{

    /// <summary>
    /// Attribute class used to indicate whether a property can be overwritten per environment.<br/>
    /// The default behavior is that any property can be overwritten.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class, AllowMultiple = false)]
    public sealed class EnvironmentalOverridesAttribute : Attribute
    {
        private readonly bool canOverride;

        /// <summary>
        /// Initializes a new instance of the <see cref="EnvironmentalOverridesAttribute"/> class.
        /// </summary>
        /// <param name="canOverride"><see langword="true"/> to specify the property can be overwritten per environment. Otherwise <see langword="false"/>.</param>
        public EnvironmentalOverridesAttribute(bool canOverride)
        {
            this.canOverride = canOverride;
        }

        /// <summary>
        /// <see langword="true"/> if the property can be overwritten per environment. Otherwise <see langword="false"/>.
        /// </summary>
        public bool CanOverride
        {
            get { return canOverride; }
        }

        /// <summary>
        /// Specifies a custom property type for the overrides property.<br/>
        /// </summary>
        public Type CustomOverridesPropertyType
        {
            get;
            set;
        }

        /// <summary>
        /// Specifies a <see cref="TypeConverter"/> that should be used to serialize the overriden value to the delta configuration file. <br/>
        /// This can be used to overwrite a property that doesnt implement <see cref="IConvertible"/>.  <br/>
        /// </summary>
        public Type StorageConverterType
        {
            get;
            set;
        }
    }
}
