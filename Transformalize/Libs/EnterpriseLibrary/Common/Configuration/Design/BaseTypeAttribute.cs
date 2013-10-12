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
    /// Indicates the base class or interface that must be assignable from the type specified in the property that this attribute decorates.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class BaseTypeAttribute : Attribute
    {
        private readonly Type configurationType;
        private readonly Type baseType;
        private readonly TypeSelectorIncludes typeSelectorIncludes;

        /// <summary>
        /// Initializes a new instance of the  <see cref="BaseTypeAttribute"/> class with the specified <see cref="Type"/> object.
        /// </summary>
        /// <param name="baseType">
        /// The <see cref="Type"/> to filter selections.
        /// </param>
        public BaseTypeAttribute(Type baseType)
            : this(baseType, TypeSelectorIncludes.None)
        {
        }

        /// <summary>
        /// Initializes a new instance of the  <see cref="BaseTypeAttribute"/> class with the specified base <see cref="Type"/> object and configuration <see cref="Type"/>.
        /// </summary>
        /// <param name="baseType">The base <see cref="Type"/> to filter.</param>
        /// <param name="configurationType">The configuration object <see cref="Type"/>.</param>
        public BaseTypeAttribute(Type baseType, Type configurationType)
            : this(baseType, TypeSelectorIncludes.None, configurationType)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseTypeAttribute"/> class with the specified <see cref="Type"/> object and <see cref="TypeSelectorIncludes"/>.
        /// </summary>
        /// <param name="baseType">
        /// The <see cref="Type"/> to filter selections.
        /// </param>
        /// <param name="typeSelectorIncludes">
        /// One of the <see cref="TypeSelectorIncludes"/> values.
        /// </param>
        public BaseTypeAttribute(Type baseType, TypeSelectorIncludes typeSelectorIncludes)
            : this(baseType, typeSelectorIncludes, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the  <see cref="BaseTypeAttribute"/> class with the specified base <see cref="Type"/> object and configuration <see cref="Type"/>.
        /// </summary>
        /// <param name="typeSelectorIncludes">
        /// One of the <see cref="typeSelectorIncludes"/> values.
        /// </param>
        /// <param name="baseType">The base <see cref="Type"/> to filter.</param>
        /// <param name="configurationType">The configuration object <see cref="Type"/>.</param>
        public BaseTypeAttribute(Type baseType, TypeSelectorIncludes typeSelectorIncludes, Type configurationType)
            : base()
        {
            if (null == baseType) throw new ArgumentNullException("baseType");
            this.configurationType = configurationType;
            this.baseType = baseType;
            this.typeSelectorIncludes = typeSelectorIncludes;
        }

        /// <summary>
        /// Gets the includes for the type selector.
        /// </summary>
        /// <value>
        /// The includes for the type selector.
        /// </value>
        public TypeSelectorIncludes TypeSelectorIncludes
        {
            get { return typeSelectorIncludes; }
        }

        /// <summary>
        /// Gets the <see cref="Type"/> to filter selections.
        /// </summary>
        /// <value>
        /// The <see cref="Type"/> to filter selections.
        /// </value>
        public Type BaseType
        {
            get { return baseType; }
        }

        /// <summary>
        /// Gets the configuration object <see cref="Type"/>.
        /// </summary>
        /// <value>
        /// The configuration object <see cref="Type"/>.
        /// </value>
        public Type ConfigurationType
        {
            get { return configurationType; }
        }
    }
}
