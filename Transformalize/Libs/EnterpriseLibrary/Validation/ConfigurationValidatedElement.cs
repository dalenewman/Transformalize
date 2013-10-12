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
using Transformalize.Libs.EnterpriseLibrary.Validation.Configuration;
using Transformalize.Libs.EnterpriseLibrary.Validation.Validators;

namespace Transformalize.Libs.EnterpriseLibrary.Validation
{
    /// <summary>
    /// Represents the description of how validation must be performed on a language element as defined by configuration.
    /// </summary>
    /// <remarks>
    /// This class is a flyweight, so instances should not be kept for later use.
    /// </remarks>
    public class ConfigurationValidatedElement : IValidatedElement
    {
        private ValidatedMemberReference validatedMemberReference;
        private MemberInfo memberInfo;
        private Type targetType;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationValidatedElement"/> class.
        /// </summary>
        public ConfigurationValidatedElement()
        { }

        #region erase when posible

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationValidatedElement"/> class for a field.
        /// </summary>
        /// <param name="validatedFieldReference">The field reference configuration object.</param>
        /// <param name="fieldInfo">The field.</param>
        public ConfigurationValidatedElement(ValidatedFieldReference validatedFieldReference, FieldInfo fieldInfo)
        {
            UpdateFlyweight(validatedFieldReference, fieldInfo);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationValidatedElement"/> class for a method.
        /// </summary>
        /// <param name="validatedMethodReference">The method reference configuration object.</param>
        /// <param name="methodInfo">The method.</param>
        public ConfigurationValidatedElement(ValidatedMethodReference validatedMethodReference, MethodInfo methodInfo)
        {
            UpdateFlyweight(validatedMethodReference, methodInfo);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationValidatedElement"/> class for a property.
        /// </summary>
        /// <param name="validatedPropertyReference">The property reference configuration object.</param>
        /// <param name="propertyInfo">The property.</param>
        public ConfigurationValidatedElement(ValidatedPropertyReference validatedPropertyReference, PropertyInfo propertyInfo)
        {
            UpdateFlyweight(validatedPropertyReference, propertyInfo);
        }

        #endregion

        /// <summary>
        /// Updates the flyweight for a field.
        /// </summary>
        /// <param name="validatedFieldReference">The field reference configuration object.</param>
        /// <param name="fieldInfo">The field.</param>
        public void UpdateFlyweight(ValidatedFieldReference validatedFieldReference, FieldInfo fieldInfo)
        {
            if (fieldInfo == null) throw new ArgumentNullException("fieldInfo");

            this.UpdateFlyweight(validatedFieldReference, fieldInfo, fieldInfo.FieldType);
        }

        /// <summary>
        /// Updates the flyweight for a method.
        /// </summary>
        /// <param name="validatedMethodReference">The method reference configuration object.</param>
        /// <param name="methodInfo">The method.</param>
        public void UpdateFlyweight(ValidatedMethodReference validatedMethodReference, MethodInfo methodInfo)
        {
            if (methodInfo == null) throw new ArgumentNullException("methodInfo");

            UpdateFlyweight(validatedMethodReference, methodInfo, methodInfo.ReturnType);
        }

        /// <summary>
        /// Updates the flyweight for a property.
        /// </summary>
        /// <param name="validatedPropertyReference">The property reference configuration object.</param>
        /// <param name="propertyInfo">The property.</param>
        public void UpdateFlyweight(ValidatedPropertyReference validatedPropertyReference, PropertyInfo propertyInfo)
        {
            if (propertyInfo == null) throw new ArgumentNullException("propertyInfo");

            UpdateFlyweight(validatedPropertyReference, propertyInfo, propertyInfo.PropertyType);
        }

        private void UpdateFlyweight(ValidatedMemberReference validatedMemberReference, MemberInfo memberInfo, Type targetType)
        {
            this.validatedMemberReference = validatedMemberReference;
            this.memberInfo = memberInfo;
            this.targetType = targetType;
        }

        IEnumerable<IValidatorDescriptor> IValidatedElement.GetValidatorDescriptors()
        {
            if (this.validatedMemberReference == null)
            {
                yield break;
            }

            foreach (IValidatorDescriptor validatorData in this.validatedMemberReference.Validators)
            {
                yield return validatorData;
            }
        }

        CompositionType IValidatedElement.CompositionType
        {
            get { return CompositionType.And; }
        }

        string IValidatedElement.CompositionMessageTemplate
        {
            get { return null; }
        }

        string IValidatedElement.CompositionTag
        {
            get { return null; }
        }

        bool IValidatedElement.IgnoreNulls
        {
            get { return false; }
        }

        string IValidatedElement.IgnoreNullsMessageTemplate
        {
            get { return null; }
        }

        string IValidatedElement.IgnoreNullsTag
        {
            get { return null; }
        }

        MemberInfo IValidatedElement.MemberInfo
        {
            get { return this.memberInfo; }
        }

        Type IValidatedElement.TargetType
        {
            get { return this.targetType; }
        }
    }
}
