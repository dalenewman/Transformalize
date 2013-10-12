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
using System.Linq;
using System.Reflection;
using Transformalize.Libs.EnterpriseLibrary.Common.Configuration;
using Transformalize.Libs.EnterpriseLibrary.Validation.Configuration;
using Transformalize.Libs.EnterpriseLibrary.Validation.Validators;

namespace Transformalize.Libs.EnterpriseLibrary.Validation
{
    /// <summary>
    /// Factory for creating <see cref="Validator"/> objects for properties.
    /// </summary>
    /// <seealso cref="Validator"/>
    public static class PropertyValidationFactory
    {
        private static IDictionary<PropertyValidatorCacheKey, Validator> attributeOnlyPropertyValidatorsCache
            = new Dictionary<PropertyValidatorCacheKey, Validator>();
        private static IDictionary<PropertyValidatorCacheKey, Validator> defaultConfigurationOnlyPropertyValidatorsCache
            = new Dictionary<PropertyValidatorCacheKey, Validator>();
        private static IDictionary<PropertyValidatorCacheKey, Validator> validationAttributeOnlyPropertyValidatorsCache
            = new Dictionary<PropertyValidatorCacheKey, Validator>();

        /// <summary>
        /// Resets the cached validators.
        /// </summary>
        public static void ResetCaches()
        {
            lock (attributeOnlyPropertyValidatorsCache)
            {
                attributeOnlyPropertyValidatorsCache.Clear();
            }
            lock (defaultConfigurationOnlyPropertyValidatorsCache)
            {
                defaultConfigurationOnlyPropertyValidatorsCache.Clear();
            }
            lock (validationAttributeOnlyPropertyValidatorsCache)
            {
                validationAttributeOnlyPropertyValidatorsCache.Clear();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="propertyInfo"></param>
        /// <param name="ruleset"></param>
        /// <param name="validationSpecificationSource"></param>
        /// <param name="memberValueAccessBuilder"></param>
        /// <returns></returns>
        public static Validator GetPropertyValidator(Type type,
            PropertyInfo propertyInfo,
            string ruleset,
            ValidationSpecificationSource validationSpecificationSource,
            MemberValueAccessBuilder memberValueAccessBuilder)
        {
            // TODO should pass along validator factory?
            return GetPropertyValidator(type,
                propertyInfo,
                ruleset,
                validationSpecificationSource,
                new MemberAccessValidatorBuilderFactory(memberValueAccessBuilder));
        }

        /// <summary>
        /// Returns a <see cref="Validator"/> for <paramref name="propertyInfo"/> as defined in the validation specification
        /// for <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The type for which the validation specification must be retrieved.</param>
        /// <param name="propertyInfo">The property for which the validator must be returned.</param>
        /// <param name="ruleset">The ruleset to use when retrieving validation information, or an empty string to use
        /// the default ruleset.</param>
        /// <param name="validationSpecificationSource">The <see cref="ValidationSpecificationSource"/> indicating
        /// where to retrieve validation specifications.</param>
        /// <param name="memberAccessValidatorBuilderFactory"></param>
        /// <returns>The appropriate validator, or null if there is no such validator specified.</returns>
        /// <exception cref="InvalidOperationException">when <paramref name="type"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvalidOperationException">when <paramref name="propertyInfo"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvalidOperationException">when <paramref name="propertyInfo"/> is not a readable property.</exception>
        /// <remarks>
        /// Both <paramref name="type"/> and <paramref name="propertyInfo"/> must be provided as <paramref name="type"/> might be different
        /// from the declaring type for <paramref name="propertyInfo"/>.
        /// </remarks>
        public static Validator GetPropertyValidator(Type type,
            PropertyInfo propertyInfo,
            string ruleset,
            ValidationSpecificationSource validationSpecificationSource,
            MemberAccessValidatorBuilderFactory memberAccessValidatorBuilderFactory)
        {
            if (null == type)
            {
                // invalid operation exception is used to match the platform's errors
                throw new InvalidOperationException(Resources.ExceptionTypeNotFound);
            }

            if (null == propertyInfo)
            {
                throw new InvalidOperationException(Resources.ExceptionPropertyNotFound);
            }
            if (!propertyInfo.CanRead)
            {
                throw new InvalidOperationException(Resources.ExceptionPropertyNotReadable);
            }

            var validators = new List<Validator>();
            if (validationSpecificationSource.IsSet(ValidationSpecificationSource.Attributes))
            {
                validators.Add(
                    GetPropertyValidatorFromAttributes(type, propertyInfo, ruleset, memberAccessValidatorBuilderFactory));
            }
            if (validationSpecificationSource.IsSet(ValidationSpecificationSource.Configuration))
            {
                validators.Add(
                    GetPropertyValidatorFromConfiguration(type, propertyInfo, ruleset, memberAccessValidatorBuilderFactory));
            }
            if (validationSpecificationSource.IsSet(ValidationSpecificationSource.DataAnnotations))
            {
                validators.Add(
                    GetPropertyValidatorFromValidationAttributes(type, propertyInfo, ruleset, memberAccessValidatorBuilderFactory));
            }

            var effectiveValidators = validators.Where(v => v != null).ToArray();
            if (effectiveValidators.Length == 1)
            {
                return effectiveValidators[0];
            }
            else if (effectiveValidators.Length > 1)
            {
                return new AndCompositeValidator(effectiveValidators);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="propertyInfo"></param>
        /// <param name="ruleset"></param>
        /// <param name="memberAccessValidatorBuilderFactory"></param>
        /// <returns></returns>
        public static Validator GetPropertyValidatorFromAttributes(
            Type type,
            PropertyInfo propertyInfo,
            string ruleset,
            MemberAccessValidatorBuilderFactory memberAccessValidatorBuilderFactory)
        {
            Validator validator = null;

            lock (attributeOnlyPropertyValidatorsCache)
            {
                var key = new PropertyValidatorCacheKey(type, propertyInfo.Name, ruleset);
                if (!attributeOnlyPropertyValidatorsCache.TryGetValue(key, out validator))
                {
                    validator =
                        new MetadataValidatorBuilder(memberAccessValidatorBuilderFactory, ValidationFactory.DefaultCompositeValidatorFactory)
                            .CreateValidatorForProperty(propertyInfo, ruleset);

                    attributeOnlyPropertyValidatorsCache[key] = validator;
                }
            }

            return validator;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="propertyInfo"></param>
        /// <param name="ruleset"></param>
        /// <param name="memberAccessValidatorBuilderFactory"></param>
        /// <returns></returns>
        public static Validator GetPropertyValidatorFromConfiguration(
            Type type,
            PropertyInfo propertyInfo,
            string ruleset,
            MemberAccessValidatorBuilderFactory memberAccessValidatorBuilderFactory)
        {
            Validator validator = null;

            lock (defaultConfigurationOnlyPropertyValidatorsCache)
            {
                PropertyValidatorCacheKey key = new PropertyValidatorCacheKey(type, propertyInfo.Name, ruleset);
                if (!defaultConfigurationOnlyPropertyValidatorsCache.TryGetValue(key, out validator))
                {
                    using (var configurationSource = ConfigurationSourceFactory.Create())
                    {
                        ConfigurationValidatorBuilder builder =
                            ConfigurationValidatorBuilder.FromConfiguration(
                                configurationSource,
                                memberAccessValidatorBuilderFactory,
                                ValidationFactory.DefaultCompositeValidatorFactory);

                        ValidatedPropertyReference propertyReference =
                            GetValidatedPropertyReference(type, ruleset, propertyInfo.Name, configurationSource);
                        if (null == propertyReference)
                            validator = null;
                        else
                            validator = builder.CreateValidatorForProperty(type, propertyReference);

                        defaultConfigurationOnlyPropertyValidatorsCache[key] = validator;
                    }
                }
            }

            return validator;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="propertyInfo"></param>
        /// <param name="ruleset"></param>
        /// <param name="memberAccessValidatorBuilderFactory"></param>
        /// <returns></returns>
        public static Validator GetPropertyValidatorFromValidationAttributes(
            Type type,
            PropertyInfo propertyInfo,
            string ruleset,
            MemberAccessValidatorBuilderFactory memberAccessValidatorBuilderFactory)
        {
            Validator validator = null;

            lock (validationAttributeOnlyPropertyValidatorsCache)
            {
                var key = new PropertyValidatorCacheKey(type, propertyInfo.Name, ruleset);
                if (!validationAttributeOnlyPropertyValidatorsCache.TryGetValue(key, out validator))
                {
                    validator =
                        string.IsNullOrEmpty(ruleset)
                            ? new ValidationAttributeValidatorBuilder(memberAccessValidatorBuilderFactory, ValidationFactory.DefaultCompositeValidatorFactory)
                                .CreateValidatorForProperty(propertyInfo)
                            : new AndCompositeValidator();

                    validationAttributeOnlyPropertyValidatorsCache[key] = validator;
                }
            }

            return validator;
        }

        private static ValidatedPropertyReference GetValidatedPropertyReference(Type type,
            string ruleset,
            string propertyName,
            IConfigurationSource configurationSource)
        {
            ValidationSettings validationSettings = configurationSource.GetSection(ValidationSettings.SectionName) as ValidationSettings;
            if (null != validationSettings)
            {
                ValidatedTypeReference typeReference = validationSettings.Types.Get(type.FullName);
                if (null != typeReference)
                {
                    ValidationRulesetData ruleData = string.IsNullOrEmpty(ruleset)
                        ? typeReference.Rulesets.Get(typeReference.DefaultRuleset)
                        : typeReference.Rulesets.Get(ruleset);
                    if (null != ruleData)
                    {
                        return ruleData.Properties.Get(propertyName);
                    }
                }
            }

            return null;
        }

        private struct PropertyValidatorCacheKey : IEquatable<PropertyValidatorCacheKey>
        {
            private Type sourceType;
            private string propertyName;
            private string ruleset;

            public PropertyValidatorCacheKey(Type sourceType, string propertyName, string ruleset)
            {
                this.sourceType = sourceType;
                this.propertyName = propertyName;
                this.ruleset = ruleset;
            }

            public override int GetHashCode()
            {
                return this.sourceType.GetHashCode()
                    ^ this.propertyName.GetHashCode()
                    ^ (this.ruleset != null ? this.ruleset.GetHashCode() : 0);
            }

            #region IEquatable<PropertyValidatorCacheKey> Members

            bool IEquatable<PropertyValidatorCacheKey>.Equals(PropertyValidatorCacheKey other)
            {
                return (this.sourceType == other.sourceType)
                    && (this.propertyName.Equals(other.propertyName))
                    && (this.ruleset == null ? other.ruleset == null : this.ruleset.Equals(other.ruleset));
            }

            #endregion
        }
    }
}
