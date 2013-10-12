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
using System.Reflection;
using Transformalize.Libs.EnterpriseLibrary.Common.Configuration;
using Transformalize.Libs.EnterpriseLibrary.Validation.Configuration;
using Transformalize.Libs.EnterpriseLibrary.Validation.Validators;

namespace Transformalize.Libs.EnterpriseLibrary.Validation
{
    /// <summary>
    /// 
    /// </summary>
    public class ConfigurationValidatorBuilder : ValidatorBuilderBase
    {
        private static readonly Validator EmptyValidator = new AndCompositeValidator();

        private readonly ValidationSettings validationSettings;

        ///<summary>
        ///</summary>
        ///<param name="configurationSource"></param>
        ///<param name="memberAccessValidatorBuilderFactory"></param>
        ///<param name="validatorFactory"></param>
        ///<returns></returns>
        public static ConfigurationValidatorBuilder FromConfiguration(
            IConfigurationSource configurationSource,
            MemberAccessValidatorBuilderFactory memberAccessValidatorBuilderFactory,
            ValidatorFactory validatorFactory)
        {
            var settings = ValidationSettings.TryGet(configurationSource);

            return
                new ConfigurationValidatorBuilder(
                    settings,
                    memberAccessValidatorBuilderFactory,
                    validatorFactory);
        }

        ///<summary>
        ///</summary>
        ///<param name="validationSettings"></param>
        public ConfigurationValidatorBuilder(ValidationSettings validationSettings)
            : this(
                validationSettings,
                MemberAccessValidatorBuilderFactory.Default,
                ValidationFactory.DefaultCompositeValidatorFactory)
        { }

        ///<summary>
        ///</summary>
        ///<param name="validationSettings"></param>
        ///<param name="memberAccessValidatorFactory"></param>
        ///<param name="validatorFactory"></param>
        public ConfigurationValidatorBuilder(
            ValidationSettings validationSettings,
            MemberAccessValidatorBuilderFactory memberAccessValidatorFactory,
            ValidatorFactory validatorFactory)
            : base(memberAccessValidatorFactory, validatorFactory)
        {
            this.validationSettings = validationSettings;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="ruleset"></param>
        /// <returns></returns>
        public Validator CreateValidator(Type type, string ruleset)
        {

            ValidatedTypeReference typeReference = GetTypeReference(type);
            if (null == typeReference)
                return EmptyValidator;

            return CreateValidator(type, typeReference, ruleset);
        }

        private ValidatedTypeReference GetTypeReference(Type type)
        {
            if (null == validationSettings)
                return null;

            ValidatedTypeReference typeReference = validationSettings.Types.Get(type.FullName);
            return typeReference;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="typeReference"></param>
        /// <param name="ruleset"></param>
        /// <returns></returns>
        public Validator CreateValidator(Type type, ValidatedTypeReference typeReference, string ruleset)
        {
            if (typeReference == null) throw new ArgumentNullException("typeReference");

            if (string.IsNullOrEmpty(ruleset))
            {
                ruleset = typeReference.DefaultRuleset;
            }

            ValidationRulesetData ruleData = typeReference.Rulesets.Get(ruleset);

            if (null == ruleData)
                return EmptyValidator;

            return this.CreateValidator(new ConfigurationValidatedType(ruleData, type));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="propertyReference"></param>
        /// <returns></returns>
        public Validator CreateValidatorForProperty(Type type, ValidatedPropertyReference propertyReference)
        {
            if (propertyReference == null) throw new ArgumentNullException("propertyReference");

            if (propertyReference.Validators.Count == 0)
                return null;

            PropertyInfo propertyInfo = ValidationReflectionHelper.GetProperty(type, propertyReference.Name, false);
            if (propertyInfo == null)
                return null;

            ConfigurationValidatedElement validatedElement = new ConfigurationValidatedElement(propertyReference, propertyInfo);

            return CreateValidatorForValidatedElement(validatedElement, this.GetCompositeValidatorBuilderForProperty);
        }

        #region test only methods

        /// <summary>
        /// This member supports the Enterprise Library infrastructure and is not intended to be used directly from your code.
        /// </summary>
        public Validator CreateValidatorForRule(Type type, ValidationRulesetData ruleData)
        {
            return CreateValidator(new ConfigurationValidatedType(ruleData, type));
        }

        /// <summary>
        /// This member supports the Enterprise Library infrastructure and is not intended to be used directly from your code.
        /// </summary>
        public Validator CreateValidatorForType(Type type, ValidationRulesetData ruleData)
        {
            if (ruleData == null) throw new ArgumentNullException("ruleData");

            if (ruleData.Validators.Count == 0)
                return null;

            ConfigurationValidatedType validatedElement = new ConfigurationValidatedType(ruleData, type);

            return CreateValidatorForValidatedElement(validatedElement, this.GetCompositeValidatorBuilderForType);
        }

        /// <summary>
        /// This member supports the Enterprise Library infrastructure and is not intended to be used directly from your code.
        /// </summary>
        public Validator CreateValidatorForField(Type type, ValidatedFieldReference fieldReference)
        {
            if (fieldReference == null) throw new ArgumentNullException("fieldReference");

            if (fieldReference.Validators.Count == 0)
                return null;

            FieldInfo fieldInfo = ValidationReflectionHelper.GetField(type, fieldReference.Name, false);
            if (fieldInfo == null)
                return null;

            ConfigurationValidatedElement validatedElement = new ConfigurationValidatedElement(fieldReference, fieldInfo);

            return CreateValidatorForValidatedElement(validatedElement, this.GetCompositeValidatorBuilderForField);
        }

        /// <summary>
        /// This member supports the Enterprise Library infrastructure and is not intended to be used directly from your code.
        /// </summary>
        public Validator CreateValidatorForMethod(Type type, ValidatedMethodReference methodReference)
        {
            if (methodReference == null) throw new ArgumentNullException("methodReference");

            if (methodReference.Validators.Count == 0)
                return null;

            MethodInfo methodInfo = ValidationReflectionHelper.GetMethod(type, methodReference.Name, false);
            if (methodInfo == null)
                return null;

            ConfigurationValidatedElement validatedElement = new ConfigurationValidatedElement(methodReference, methodInfo);

            return CreateValidatorForValidatedElement(validatedElement, this.GetCompositeValidatorBuilderForMethod);
        }

        #endregion
    }
}
