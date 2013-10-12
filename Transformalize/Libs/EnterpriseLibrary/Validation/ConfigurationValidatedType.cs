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
    /// Represents the description of how validation must be performed on a type as defined by configuration.
    /// </summary>
    public class ConfigurationValidatedType : IValidatedType
    {
        private ValidationRulesetData ruleData;
        private Type targetType;
        private static readonly MethodInfo[] NoSelfValidationMethods = new MethodInfo[0];

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationValidatedType"/> class for a ruleset on a 
        /// target type.
        /// </summary>
        /// <param name="ruleData">The validation rules corresponding to a ruleset.</param>
        /// <param name="targetType">The target type.</param>
        public ConfigurationValidatedType(ValidationRulesetData ruleData, Type targetType)
        {
            this.ruleData = ruleData;
            this.targetType = targetType;
        }

        #region IValidatedType Members

        IEnumerable<IValidatedElement> IValidatedType.GetValidatedProperties()
        {
            ConfigurationValidatedElement flyweight = new ConfigurationValidatedElement();

            foreach (ValidatedPropertyReference validatedMemberReference in this.ruleData.Properties)
            {
                if (validatedMemberReference.Validators.Count == 0)
                    continue;

                PropertyInfo propertyInfo
                    = ValidationReflectionHelper.GetProperty(this.targetType, validatedMemberReference.Name, false);
                if (propertyInfo == null)
                    continue;

                flyweight.UpdateFlyweight(validatedMemberReference, propertyInfo);
                yield return flyweight;
            }
        }

        IEnumerable<IValidatedElement> IValidatedType.GetValidatedFields()
        {
            ConfigurationValidatedElement flyweight = new ConfigurationValidatedElement();

            foreach (ValidatedFieldReference validatedMemberReference in this.ruleData.Fields)
            {
                if (validatedMemberReference.Validators.Count == 0)
                    continue;

                FieldInfo fieldInfo
                    = ValidationReflectionHelper.GetField(this.targetType, validatedMemberReference.Name, false);
                if (fieldInfo == null)
                    continue;

                flyweight.UpdateFlyweight(validatedMemberReference, fieldInfo);
                yield return flyweight;
            }
        }

        IEnumerable<IValidatedElement> IValidatedType.GetValidatedMethods()
        {
            ConfigurationValidatedElement flyweight = new ConfigurationValidatedElement();

            foreach (ValidatedMethodReference validatedMemberReference in this.ruleData.Methods)
            {
                if (validatedMemberReference.Validators.Count == 0)
                    continue;

                MethodInfo methodInfo
                    = ValidationReflectionHelper.GetMethod(this.targetType, validatedMemberReference.Name, false);
                if (methodInfo == null)
                    continue;

                flyweight.UpdateFlyweight(validatedMemberReference, methodInfo);
                yield return flyweight;
            }
        }

        IEnumerable<MethodInfo> IValidatedType.GetSelfValidationMethods()
        {
            // self validation is not supported through configuration
            return NoSelfValidationMethods;
        }

        #endregion

        #region IValidatedElement Members

        IEnumerable<IValidatorDescriptor> IValidatedElement.GetValidatorDescriptors()
        {
            foreach (IValidatorDescriptor validatorData in this.ruleData.Validators)
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
            get { return this.targetType; }
        }

        Type IValidatedElement.TargetType
        {
            get { return this.targetType; }
        }

        #endregion
    }
}
