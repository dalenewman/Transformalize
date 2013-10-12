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
    /// Attribute class used to indicate that the property is a reference to provider. <br/>
    /// Reference properties will show an editable dropdown that allows the referred element to be selected.<br/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple=false)]
    public sealed class ReferenceAttribute : Attribute
    {
        private readonly string scopeTypeName;
        private readonly string targetTypeName;
        private Type cachedType;
        private Type cachedScopeType;
        private bool scopeTypeCached = false;


        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceAttribute"/> class.
        /// </summary>
        /// <param name="targetTypeName">The configuration type name of the provider that used as a reference.</param>
        public ReferenceAttribute(string targetTypeName)
        {
            if (string.IsNullOrEmpty(targetTypeName)) throw new ArgumentException(Resources.ExceptionStringNullOrEmpty, "targetTypeName");
            this.targetTypeName = targetTypeName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceAttribute"/> class.
        /// </summary>
        /// <param name="scopeTypeName">The name of a configuration type that contains the references.</param>
        /// <param name="targetTypeName">The configuration type name of the provider that used as a reference.</param>
        public ReferenceAttribute(string scopeTypeName, string targetTypeName)
        {
            if (string.IsNullOrEmpty(scopeTypeName)) throw new ArgumentException(Resources.ExceptionStringNullOrEmpty, "scopeTypeName");
            if (string.IsNullOrEmpty(targetTypeName)) throw new ArgumentException(Resources.ExceptionStringNullOrEmpty, "targetTypeName");

            this.scopeTypeName = scopeTypeName;
            this.targetTypeName = targetTypeName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceAttribute"/> class.
        /// </summary>
        /// <param name="targetType">The configuration type of the provider that used as a reference.</param>
        public ReferenceAttribute(Type targetType)
        {
            if (targetType == null) throw new ArgumentNullException("targetType");

            this.targetTypeName = targetType.AssemblyQualifiedName;
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceAttribute"/> class.
        /// </summary>
        /// <param name="scopeType">The configuration type that contains the references.</param>
        /// <param name="targetType">The configuration type of the provider that used as a reference.</param>
        public ReferenceAttribute(Type scopeType, Type targetType)
        {
            if (targetType == null) throw new ArgumentNullException("targetType");
            if (scopeType == null) throw new ArgumentNullException("scopeType");

            this.scopeTypeName = scopeType.AssemblyQualifiedName;
            this.targetTypeName = targetType.AssemblyQualifiedName;
        }

        /// <summary>
        /// Gets the configuration type that contains the references.
        /// </summary>
        public Type ScopeType
        {
            get
            {
                if (!scopeTypeCached)
                {
                    cachedScopeType = string.IsNullOrEmpty(scopeTypeName) ? null : Type.GetType(scopeTypeName);
                    scopeTypeCached = true;
                }

                return cachedScopeType;
            }
        }

        /// <summary>
        /// Gets or sets a boolean indicating whether only providers can be used that are contained in the current Element View Model.
        /// </summary>
        /// <value>
        /// <see langword="true"/> if only providers can be used that are contained in the current Element View Model. Otherwise <see langword="false"/>.
        /// </value>
        public bool ScopeIsDeclaringElement
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the configuration type of the provider that used as a reference.
        /// </summary>
        public Type TargetType
        {
            get
            {
                if (cachedType == null)
                {
                    cachedType = Type.GetType(targetTypeName);
                }
                
                return cachedType;
            }
        }
    }
}
