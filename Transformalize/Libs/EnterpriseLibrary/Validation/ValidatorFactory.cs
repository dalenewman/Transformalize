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
using Transformalize.Libs.EnterpriseLibrary.Validation.Validators;

namespace Transformalize.Libs.EnterpriseLibrary.Validation
{
    ///<summary>
    /// Abstract validator factory for creating validators for a specific type.
    ///</summary>
    /// <seealso cref="AttributeValidatorFactory"/>
    /// <seealso cref="ConfigurationValidatorFactory"/>    
    /// <seealso cref="ValidationAttributeValidatorFactory"/>
    /// <seealso cref="CompositeValidatorFactory"/>
    public abstract class ValidatorFactory
    {
        private readonly object validatorCacheLock = new object();
        private readonly IDictionary<ValidatorCacheKey, Validator> validatorCache
            = new Dictionary<ValidatorCacheKey, Validator>();

        ///<summary>
        /// Initializes a ValidatorFactory factory.
        ///</summary>
        protected ValidatorFactory()
        {
        }

        /// <summary>
        /// Wraps a <see cref="Validator"/> with a <see cref="GenericValidatorWrapper{T}"/> for instrumentation purposes.
        /// </summary>
        /// <param name="validator">The validator to wrap</param>
        /// <param name="type">Validator's target type</param>
        /// <returns>A new, wrapped validator</returns>
        protected virtual Validator WrapAndInstrumentValidator(Validator validator, Type type)
        {
            var validatorWrapperType = typeof(GenericValidatorWrapper<>).MakeGenericType(type);
            var validatorWrapper = (Validator)Activator.CreateInstance(
                validatorWrapperType,
                new object[] { validator });

            return validatorWrapper;
        }

        /// <summary>
        /// Wraps a <see cref="Validator{T}"/> with a <see cref="GenericValidatorWrapper{T}"/> for instrumentation purposes.
        /// </summary>
        /// <typeparam name="T">Validator's target type.</typeparam>
        /// <param name="validator">Validator to wrap</param>
        /// <param name="unusedType">Not used in this implementation of WrapAndInstrumentValidator</param>
        /// <returns>A new, wrapped validator.</returns>
        protected virtual Validator<T> WrapAndInstrumentValidator<T>(Validator validator, Type unusedType)
        {
            var validatorWrapper = new GenericValidatorWrapper<T>(validator);
            return validatorWrapper;
        }

        /// <summary>
        /// Returns a validator representing the validation criteria specified for type <typeparamref name="T"/>
        /// through configuration and attributes on type <typeparamref name="T"/> and its ancestors for the default ruleset.
        /// </summary>
        /// <typeparam name="T">The type to get the validator for.</typeparam>
        /// <returns>The validator.</returns>
        public Validator<T> CreateValidator<T>()
        {
            return CreateValidator<T>(string.Empty);
        }

        private Validator FindOrCreateValidator(ValidatorCacheKey cacheKey, Func<Validator, Type, Validator> wrapAndInstrument)
        {
            Validator wrapperValidator = null;

            lock (validatorCacheLock)
            {
                Validator cachedValidator;
                if (validatorCache.TryGetValue(cacheKey, out cachedValidator))
                {
                    return cachedValidator;
                }

                Validator validator = InnerCreateValidator(cacheKey.SourceType, cacheKey.Ruleset, this);
                wrapperValidator = wrapAndInstrument(validator, cacheKey.SourceType);

                validatorCache[cacheKey] = wrapperValidator;
            }

            return wrapperValidator;
        }

        /// <summary>
        /// Returns a validator representing the validation criteria specified for type <typeparamref name="T"/>
        /// through configuration and attributes on type <typeparamref name="T"/> and its ancestors for the supplied ruleset.
        /// </summary>
        /// <typeparam name="T">The type to get the validator for.</typeparam>
        /// <param name="ruleset">The name of the required ruleset.</param>
        /// <returns>The validator.</returns>
        /// <exception cref="ArgumentNullException">when the <paramref name="ruleset"/> is <see langword="null"/>.</exception>
        public virtual Validator<T> CreateValidator<T>(string ruleset)
        {
            if (ruleset == null) throw new ArgumentNullException("ruleset");

            return (Validator<T>)FindOrCreateValidator(
                new ValidatorCacheKey(typeof(T), ruleset, true),
                WrapAndInstrumentValidator<T>);
        }

        /// <summary>
        /// Returns a validator representing the validation criteria specified for type <paramref name="targetType"/>
        /// through configuration and aatributes on type <paramref name="targetType"/> and its ancestors for the default ruleset.
        /// </summary>
        /// <param name="targetType">The type to get the validator for.</param>
        /// <returns>The validator.</returns>
        public Validator CreateValidator(Type targetType)
        {
            return CreateValidator(targetType, string.Empty);
        }

        /// <summary>
        /// Returns a validator representing the validation criteria specified for type <paramref name="targetType"/>
        /// through configuration and attributes on type <paramref name="targetType"/> and its ancestors for the supplied ruleset.
        /// </summary>
        /// <param name="targetType">The type to get the validator for.</param>
        /// <param name="ruleset">The name of the required ruleset.</param>
        /// <returns>The validator.</returns>
        /// <exception cref="ArgumentNullException">when the <paramref name="ruleset"/> is <see langword="null"/>.</exception>
        public virtual Validator CreateValidator(Type targetType, string ruleset)
        {
            if (targetType == null) throw new ArgumentNullException("targetType");
            if (ruleset == null) throw new ArgumentNullException("ruleset");

            return FindOrCreateValidator(
                new ValidatorCacheKey(targetType, ruleset, false),
                WrapAndInstrumentValidator);
        }

        /// <summary>
        /// Creates the validator for the specified target and ruleset.
        /// </summary>
        /// <param name="targetType">The <see cref="Type"/>to validate.</param>
        /// <param name="ruleset">The ruleset to use when validating</param>
        /// <param name="mainValidatorFactory">Factory to use when building nested validators.</param>
        /// <returns>A <see cref="Validator"/></returns>
        protected internal abstract Validator InnerCreateValidator(Type targetType, string ruleset, ValidatorFactory mainValidatorFactory);

        ///<summary>
        /// Clears the internal validator cache.
        ///</summary>
        public virtual void ResetCache()
        {
            lock (validatorCacheLock)
            {
                validatorCache.Clear();
            }
        }

    }
}
