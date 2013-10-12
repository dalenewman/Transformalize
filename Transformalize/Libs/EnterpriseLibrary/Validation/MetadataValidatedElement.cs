//===============================================================================
// Microsoft patterns & practices Enterprise Library
// Validation Application Block
//===============================================================================
// Copyright © Microsoft Corporation.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===============================================================================

using System;
using System.Collections.Generic;
using System.Reflection;
using Transformalize.Libs.EnterpriseLibrary.Validation.Validators;

namespace Transformalize.Libs.EnterpriseLibrary.Validation
{
    /// <summary>
    /// Represents the description of how validation must be performed on a language element as defined by attributes.
    /// </summary>
    /// <remarks>
    /// This class is a flyweight, so instances should not be kept for later use.
    /// </remarks>
    public class MetadataValidatedElement : IValidatedElement
    {
        private MemberInfo memberInfo;
        private IgnoreNullsAttribute ignoreNullsAttribute;
        private ValidatorCompositionAttribute validatorCompositionAttribute;
        private Type targetType;
        private string ruleset;

        /// <summary>
        /// Initializes a new instance of the <see cref="MetadataValidatedElement"/> class for a ruleset..
        /// </summary>
        /// <param name="ruleset">The ruleset.</param>
        public MetadataValidatedElement(string ruleset)
        {
            this.ruleset = ruleset;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MetadataValidatedElement"/> class for a field and a ruleset.
        /// </summary>
        /// <param name="fieldInfo">The field.</param>
        /// <param name="ruleset">The ruleset.</param>
        public MetadataValidatedElement(FieldInfo fieldInfo, string ruleset)
            : this(ruleset)
        {
            this.UpdateFlyweight(fieldInfo);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MetadataValidatedElement"/> class for a method and a ruleset.
        /// </summary>
        /// <param name="methodInfo">The method.</param>
        /// <param name="ruleset">The ruleset.</param>
        public MetadataValidatedElement(MethodInfo methodInfo, string ruleset)
            : this(ruleset)
        {
            this.UpdateFlyweight(methodInfo);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MetadataValidatedElement"/> class for a property and a ruleset.
        /// </summary>
        /// <param name="propertyInfo">The property.</param>
        /// <param name="ruleset">The ruleset.</param>
        public MetadataValidatedElement(PropertyInfo propertyInfo, string ruleset)
            : this(ruleset)
        {
            this.UpdateFlyweight(propertyInfo);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MetadataValidatedElement"/> class for a type and a ruleset.
        /// </summary>
        /// <param name="type">The property.</param>
        /// <param name="ruleset">The ruleset.</param>
        public MetadataValidatedElement(Type type, string ruleset)
            : this(ruleset)
        {
            this.UpdateFlyweight(type);
        }

        /// <summary>
        /// Updates the flyweight for a field.
        /// </summary>
        /// <param name="fieldInfo">The field.</param>
        public void UpdateFlyweight(FieldInfo fieldInfo)
        {
            if (fieldInfo == null) throw new ArgumentNullException("fieldInfo");

            this.UpdateFlyweight(fieldInfo, fieldInfo.FieldType);
        }

        /// <summary>
        /// Updates the flyweight for a method.
        /// </summary>
        /// <param name="methodInfo">The method.</param>
        public void UpdateFlyweight(MethodInfo methodInfo)
        {
            if (methodInfo == null) throw new ArgumentNullException("methodInfo");

            this.UpdateFlyweight(methodInfo, methodInfo.ReturnType);
        }

        /// <summary>
        /// Updates the flyweight for a property.
        /// </summary>
        /// <param name="propertyInfo">The property.</param>
        public void UpdateFlyweight(PropertyInfo propertyInfo)
        {
            if (propertyInfo == null) throw new ArgumentNullException("propertyInfo");

            this.UpdateFlyweight(propertyInfo, propertyInfo.PropertyType);
        }

        /// <summary>
        /// Updates the flyweight for a type.
        /// </summary>
        /// <param name="type">The type.</param>
        public void UpdateFlyweight(Type type)
        {
            this.UpdateFlyweight(type, type);
        }

        private void UpdateFlyweight(MemberInfo memberInfo, Type targetType)
        {
            this.memberInfo = memberInfo;
            this.targetType = targetType;
            this.ignoreNullsAttribute
                = ValidationReflectionHelper.ExtractValidationAttribute<IgnoreNullsAttribute>(memberInfo, this.ruleset);
            this.validatorCompositionAttribute
                = ValidationReflectionHelper.ExtractValidationAttribute<ValidatorCompositionAttribute>(memberInfo, this.ruleset);
        }

        /// <summary>
        /// Gets the ruleset.
        /// </summary>
        protected string Ruleset
        {
            get { return this.ruleset; }
        }

        /// <summary>
        /// Gets the target type.
        /// </summary>
        protected Type TargetType
        {
            get { return this.targetType; }
        }

        IEnumerable<IValidatorDescriptor> IValidatedElement.GetValidatorDescriptors()
        {
            if (this.memberInfo == null)
                yield break;

            foreach (ValidatorAttribute attribute in
                ValidationReflectionHelper.GetCustomAttributes(this.memberInfo, typeof(ValidatorAttribute), false))
            {
                if (this.ruleset.Equals(attribute.Ruleset))
                    yield return attribute;
            }
        }

        CompositionType IValidatedElement.CompositionType
        {
            get
            {
                return this.validatorCompositionAttribute != null
                    ? this.validatorCompositionAttribute.CompositionType
                    : CompositionType.And;
            }
        }

        string IValidatedElement.CompositionMessageTemplate
        {
            get
            {
                return this.validatorCompositionAttribute != null
                    ? this.validatorCompositionAttribute.GetMessageTemplate()
                    : null;
            }
        }

        string IValidatedElement.CompositionTag
        {
            get
            {
                return this.validatorCompositionAttribute != null
                    ? this.validatorCompositionAttribute.Tag
                    : null;
            }
        }

        bool IValidatedElement.IgnoreNulls
        {
            get
            {
                return this.ignoreNullsAttribute != null;
            }
        }

        string IValidatedElement.IgnoreNullsMessageTemplate
        {
            get
            {
                return this.ignoreNullsAttribute != null
                    ? this.ignoreNullsAttribute.GetMessageTemplate()
                    : null;
            }
        }

        string IValidatedElement.IgnoreNullsTag
        {
            get
            {
                return this.ignoreNullsAttribute != null
                    ? this.ignoreNullsAttribute.Tag
                    : null;
            }
        }

        MemberInfo IValidatedElement.MemberInfo
        {
            get
            {
                return this.memberInfo;
            }
        }

        Type IValidatedElement.TargetType
        {
            get
            {
                return this.targetType;
            }
        }
    }
}
