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
using Transformalize.Libs.EnterpriseLibrary.Common.Configuration;
using Transformalize.Libs.EnterpriseLibrary.Common.Utility;

namespace Transformalize.Libs.EnterpriseLibrary.Validation
{
    /// <summary>
    /// Factory for creating <see cref="Validator"/> objects for types.
    /// </summary>
    /// <seealso cref="Validation"/>
    /// <seealso cref="Validator"/>
    public static class ValidationFactory
    {
        private readonly static AttributeValidatorFactory defaultAttributeValidatorFactory = new AttributeValidatorFactory();
        private readonly static ValidationAttributeValidatorFactory defaultValidationAttributeValidatorFactory = new ValidationAttributeValidatorFactory();
        private readonly static IConfigurationSource EmptyValidationConfigurationSource = new DictionaryConfigurationSource();
        private static volatile ConfigurationValidatorFactory defaultConfigurationValidatorFactory = BuildDefaultConfigurationValidatorFactory();
        private static volatile Lazy<ValidatorFactory> defaultCompositeValidatorFactory = BuildDefaultCompositeValidatorFactory();

        /// <summary>
        /// Resets the cached validators.
        /// </summary>
        public static void ResetCaches()
        {
            defaultAttributeValidatorFactory.ResetCache();
            defaultValidationAttributeValidatorFactory.ResetCache();
            defaultConfigurationValidatorFactory.ResetCache();
            if (defaultCompositeValidatorFactory.IsValueCreated)
            {
                defaultCompositeValidatorFactory.Value.ResetCache();
            }
        }

        /// <summary>
        /// Returns a validator that represents the validation criteria specified for type <typeparamref name="T"/>
        /// through configuration and attributes on type <typeparamref name="T"/> and its ancestors for the default rule set.
        /// </summary>
        /// <typeparam name="T">The type to get the validator for.</typeparam>
        /// <returns>The validator.</returns>
        public static Validator<T> CreateValidator<T>()
        {
            return DefaultCompositeValidatorFactory.CreateValidator<T>();
        }

        /// <summary>
        /// Returns a validator that represents the validation criteria specified for type <typeparamref name="T"/>
        /// through configuration and attributes on type <typeparamref name="T"/> and its ancestors for the supplied rule set.
        /// </summary>
        /// <typeparam name="T">The type to get the validator for.</typeparam>
        /// <param name="ruleset">The name of the required rule set.</param>
        /// <returns>The validator.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="ruleset"/> is <see langword="null"/>.</exception>
        public static Validator<T> CreateValidator<T>(string ruleset)
        {
            return DefaultCompositeValidatorFactory.CreateValidator<T>(ruleset);
        }

        /// <summary>
        /// Returns a validator that represents the validation criteria specified for type <typeparamref name="T"/>
        /// through configuration and attributes on type <typeparamref name="T"/> and its ancestors for the default rule set,
        /// retrieving configuration information from the supplied <see cref="IConfigurationSource"/>.
        /// </summary>
        /// <typeparam name="T">The type to get the validator for.</typeparam>
        /// <param name="configurationSource">The configuration source to retrieve configuration information from.</param>
        /// <returns>The validator.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="configurationSource"/> is <see langword="null"/>.</exception>
        public static Validator<T> CreateValidator<T>(IConfigurationSource configurationSource)
        {
            return CreateValidator<T>(configurationSource, ValidationSpecificationSource.All);
        }

        /// <summary>
        /// Returns a validator that represents the validation criteria specified for type <typeparamref name="T"/>
        /// through configuration and attributes on type <typeparamref name="T"/> and its ancestors for the supplied rule set,
        /// retrieving configuration information from the supplied <see cref="IConfigurationSource"/>.
        /// </summary>
        /// <typeparam name="T">The type to get the validator for.</typeparam>
        /// <param name="ruleset">The name of the required rule set.</param>
        /// <param name="configurationSource">The configuration source to retrieve configuration information from.</param>
        /// <returns>The validator.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="ruleset"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="configurationSource"/> is <see langword="null"/>.</exception>
        public static Validator<T> CreateValidator<T>(string ruleset, IConfigurationSource configurationSource)
        {
            return CreateValidator<T>(ruleset, configurationSource, ValidationSpecificationSource.All);
        }

        /// <summary>
        /// Returns a validator that represents the validation criteria specified for type <paramref name="targetType"/>
        /// through configuration and attributes on type <paramref name="targetType"/> and its ancestors for the default rule set.
        /// </summary>
        /// <param name="targetType">The type to get the validator for.</param>
        /// <returns>The validator.</returns>
        public static Validator CreateValidator(Type targetType)
        {
            return DefaultCompositeValidatorFactory.CreateValidator(targetType);
        }

        /// <summary>
        /// Returns a validator that represents the validation criteria specified for type <paramref name="targetType"/>
        /// through configuration and attributes on type <paramref name="targetType"/> and its ancestors for the supplied rule set.
        /// </summary>
        /// <param name="targetType">The type to get the validator for.</param>
        /// <param name="ruleset">The name of the required rule set.</param>
        /// <returns>The validator.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="ruleset"/> is <see langword="null"/>.</exception>
        public static Validator CreateValidator(Type targetType, string ruleset)
        {
            return DefaultCompositeValidatorFactory.CreateValidator(targetType, ruleset);
        }

        /// <summary>
        /// Returns a validator that represents the validation criteria specified for type <paramref name="targetType"/>
        /// through configuration and attributes on type <paramref name="targetType"/> and its ancestors for the supplied rule set,
        /// retrieving configuration information from the supplied <see cref="IConfigurationSource"/>.
        /// </summary>
        /// <param name="targetType">The type to get the validator for.</param>
        /// <param name="ruleset">The name of the required rule set.</param>
        /// <param name="configurationSource">The configuration source to retrieve configuration information from.</param>
        /// <returns>The validator.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="ruleset"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="configurationSource"/> is <see langword="null"/>.</exception>
        public static Validator CreateValidator(Type targetType, string ruleset, IConfigurationSource configurationSource)
        {
            return CreateValidator(targetType, ruleset, configurationSource, ValidationSpecificationSource.All);
        }

        /// <summary>
        /// Returns a validator that represents the validation criteria specified for type <typeparamref name="T"/>
        /// through configuration and attributes on type <typeparamref name="T"/> and its ancestors for the default rule set.
        /// </summary>
        /// <typeparam name="T">The type to get the validator for.</typeparam>
        /// <param name="source">The source of validation information.</param>
        /// <returns>The validator.</returns>
        public static Validator<T> CreateValidator<T>(ValidationSpecificationSource source)
        {
            return GetValidatorFactory(source).CreateValidator<T>();
        }

        /// <summary>
        /// Returns a validator that represents the validation criteria specified for type <typeparamref name="T"/>
        /// through configuration and attributes on type <typeparamref name="T"/> and its ancestors for the supplied rule set.
        /// </summary>
        /// <typeparam name="T">The type to get the validator for.</typeparam>
        /// <param name="ruleset">The name of the required rule set.</param>
        /// <param name="source">The source of validation information.</param>
        /// <returns>The validator.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="ruleset"/> is <see langword="null"/>.</exception>
        public static Validator<T> CreateValidator<T>(string ruleset, ValidationSpecificationSource source)
        {
            return GetValidatorFactory(source).CreateValidator<T>(ruleset);
        }

        /// <summary>
        /// Returns a validator that represents the validation criteria specified for type <typeparamref name="T"/>
        /// through configuration and attributes on type <typeparamref name="T"/> and its ancestors for the default rule set,
        /// retrieving configuration information from the supplied <see cref="IConfigurationSource"/>.
        /// </summary>
        /// <typeparam name="T">The type to get the validator for.</typeparam>
        /// <param name="configurationSource">The configuration source to retrieve configuration information from.</param>
        /// <param name="source">The source of validation information.</param>
        /// <returns>The validator.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="configurationSource"/> is <see langword="null"/>.</exception>
        public static Validator<T> CreateValidator<T>(IConfigurationSource configurationSource, ValidationSpecificationSource source)
        {
            return GetValidatorFactory(configurationSource, source).CreateValidator<T>();
        }

        /// <summary>
        /// Returns a validator that represents the validation criteria specified for type <typeparamref name="T"/>
        /// through configuration and attributes on type <typeparamref name="T"/> and its ancestors for the supplied rule set
        /// retrieving configuration information from the supplied <see cref="IConfigurationSource"/>.
        /// </summary>
        /// <typeparam name="T">The type to get the validator for.</typeparam>
        /// <param name="ruleset">The name of the required rule set.</param>
        /// <param name="configurationSource">The configuration source to retrieve configuration information from.</param>
        /// <param name="source">The source of validation information.</param>
        /// <returns>The validator.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="ruleset"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="configurationSource"/> is <see langword="null"/>.</exception>
        public static Validator<T> CreateValidator<T>(string ruleset, IConfigurationSource configurationSource, ValidationSpecificationSource source)
        {
            return GetValidatorFactory(configurationSource, source).CreateValidator<T>(ruleset);
        }

        /// <summary>
        /// Returns a validator that represents the validation criteria specified for type <paramref name="targetType"/>
        /// through configuration and attributes on type <paramref name="targetType"/> and its ancestors for the default rule set.
        /// </summary>
        /// <param name="targetType">The type to get the validator for.</param>
        /// <param name="source">The source of validation information.</param>
        /// <returns>The validator.</returns>
        public static Validator CreateValidator(Type targetType, ValidationSpecificationSource source)
        {
            return GetValidatorFactory(source).CreateValidator(targetType);
        }

        /// <summary>
        /// Returns a validator that represents the validation criteria specified for type <paramref name="targetType"/>
        /// through configuration and attributes on type <paramref name="targetType"/> and its ancestors for the supplied rule set.
        /// </summary>
        /// <param name="targetType">The type to get the validator for.</param>
        /// <param name="ruleset">The name of the required rule set.</param>
        /// <param name="source">The source of validation information.</param>
        /// <returns>The validator.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="ruleset"/> is <see langword="null"/>.</exception>
        public static Validator CreateValidator(Type targetType, string ruleset, ValidationSpecificationSource source)
        {
            return GetValidatorFactory(source).CreateValidator(targetType, ruleset);
        }

        /// <summary>
        /// Returns a validator that represents the validation criteria specified for type <paramref name="targetType"/>
        /// through configuration and attributes on type <paramref name="targetType"/> and its ancestors for the supplied rule set,
        /// retrieving configuration information from the supplied <see cref="IConfigurationSource"/>.
        /// </summary>
        /// <param name="targetType">The type to get the validator for.</param>
        /// <param name="ruleset">The name of the required rule set.</param>
        /// <param name="configurationSource">The configuration source to retrieve configuration information from.</param>
        /// <param name="source">The source of validation information.</param>
        /// <returns>The validator.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="ruleset"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="configurationSource"/> is <see langword="null"/>.</exception>
        public static Validator CreateValidator(Type targetType, string ruleset, IConfigurationSource configurationSource, ValidationSpecificationSource source)
        {
            return GetValidatorFactory(configurationSource, source).CreateValidator(targetType, ruleset);
        }

        /// <summary>
        /// Returns a validator that represents the validation criteria specified for type <typeparamref name="T"/>
        /// through attributes on type <typeparamref name="T"/> and its ancestors for the default rule set.
        /// </summary>
        /// <typeparam name="T">The type to get the validator for.</typeparam>
        /// <returns>The validator.</returns>
        public static Validator<T> CreateValidatorFromAttributes<T>()
        {
            return DefaultAttributeValidatorFactory.CreateValidator<T>();
        }

        /// <summary>
        /// Returns a validator that represents the validation criteria specified for type <typeparamref name="T"/>
        /// through attributes on type <typeparamref name="T"/> and its ancestors for the supplied rule set.
        /// </summary>
        /// <typeparam name="T">The type to get the validator for.</typeparam>
        /// <param name="ruleset">The name of the required rule set.</param>
        /// <returns>The validator.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="ruleset"/> is <see langword="null"/>.</exception>
        public static Validator<T> CreateValidatorFromAttributes<T>(string ruleset)
        {
            return DefaultAttributeValidatorFactory.CreateValidator<T>(ruleset);
        }

        /// <summary>
        /// Returns a validator that represents the validation criteria specified for type <paramref name="targetType"/>
        /// through attributes on type <paramref name="targetType"/> and its ancestors for the supplied rule set.
        /// </summary>
        /// <param name="targetType">The type to get the validator for.</param>
        /// <param name="ruleset">The name of the required rule set.</param>
        /// <returns>The validator.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="ruleset"/> is <see langword="null"/>.</exception>
        public static Validator CreateValidatorFromAttributes(Type targetType, string ruleset)
        {
            return DefaultAttributeValidatorFactory.CreateValidator(targetType, ruleset);
        }

        /// <summary>
        /// Returns a validator that represents the validation criteria specified for type <typeparamref name="T"/>
        /// through configuration for the default rule set.
        /// </summary>
        /// <typeparam name="T">The type to get the validator for.</typeparam>
        /// <returns>The validator.</returns>
        public static Validator<T> CreateValidatorFromConfiguration<T>()
        {
            return DefaultConfigurationValidatorFactory.CreateValidator<T>();
        }

        /// <summary>
        /// Returns a validator that represents the validation criteria specified for type <typeparamref name="T"/>
        /// through configuration for the default rule set,
        /// retrieving configuration information from the supplied <see cref="IConfigurationSource"/>.
        /// </summary>
        /// <typeparam name="T">The type to get the validator for.</typeparam>
        /// <param name="configurationSource">The configuration source to retrieve configuration information from.</param>
        /// <returns>The validator.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="configurationSource"/> is <see langword="null"/>.</exception>
        public static Validator<T> CreateValidatorFromConfiguration<T>(IConfigurationSource configurationSource)
        {
            var validationFactory = ConfigurationValidatorFactory.FromConfigurationSource(configurationSource);

            return validationFactory.CreateValidator<T>();
        }

        /// <summary>
        /// Returns a validator that represents the validation criteria specified for type <typeparamref name="T"/>
        /// through configuration for the supplied rule set.
        /// </summary>
        /// <typeparam name="T">The type to get the validator for.</typeparam>
        /// <param name="ruleset">The name of the required rule set.</param>
        /// <returns>The validator.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="ruleset"/> is <see langword="null"/>.</exception>
        public static Validator<T> CreateValidatorFromConfiguration<T>(string ruleset)
        {
            return DefaultConfigurationValidatorFactory.CreateValidator<T>(ruleset);
        }

        /// <summary>
        /// Returns a validator that represents the validation criteria specified for type <typeparamref name="T"/>
        /// through configuration for the supplied rule set,
        /// retrieving configuration information from the supplied <see cref="IConfigurationSource"/>.
        /// </summary>
        /// <typeparam name="T">The type to get the validator for.</typeparam>
        /// <param name="ruleset">The name of the required rule set.</param>
        /// <param name="configurationSource">The configuration source to retrieve configuration information from.</param>
        /// <returns>The validator.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="ruleset"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="configurationSource"/> is <see langword="null"/>.</exception>
        public static Validator<T> CreateValidatorFromConfiguration<T>(string ruleset, IConfigurationSource configurationSource)
        {
            var validationFactory = ConfigurationValidatorFactory.FromConfigurationSource(configurationSource);
            return validationFactory.CreateValidator<T>(ruleset);
        }

        /// <summary>
        /// Returns a validator that represents the validation criteria specified for type <paramref name="targetType"/>
        /// through configuration for the supplied rule set, retrieving configuration information from
        /// the default configuration source.
        /// </summary>
        /// <param name="targetType">The type to get the validator for.</param>
        /// <param name="ruleset">The name of the validation ruleset.</param>
        /// <returns>The validator.</returns>
        public static Validator CreateValidatorFromConfiguration(Type targetType, string ruleset)
        {
            return DefaultConfigurationValidatorFactory.CreateValidator(targetType, ruleset);
        }

        /// <summary>
        /// Returns a validator that represents the validation criteria specified for type <paramref name="targetType"/>
        /// through configuration for the supplied rule set,
        /// retrieving configuration information from the supplied <see cref="IConfigurationSource"/>.
        /// </summary>
        /// <param name="targetType">The type to get the validator for.</param>
        /// <param name="ruleset">The name of the required rule set.</param>
        /// <param name="configurationSource">The configuration source to retrieve configuration information from.</param>
        /// <returns>The validator.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="ruleset"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="configurationSource"/> is <see langword="null"/>.</exception>
        public static Validator CreateValidatorFromConfiguration(Type targetType, string ruleset, IConfigurationSource configurationSource)
        {
            var validationFactory = ConfigurationValidatorFactory.FromConfigurationSource(configurationSource);

            return validationFactory.CreateValidator(targetType, ruleset);
        }

        /// <summary>
        /// Sets the default configuration validator factory for the static <see cref="ValidationFactory"/>.
        /// </summary>
        /// <param name="factory">The configuration validation factory.</param>
        public static void SetDefaultConfigurationValidatorFactory(ConfigurationValidatorFactory factory)
        {
            Guard.ArgumentNotNull(factory, "factory");

            defaultConfigurationValidatorFactory = factory;
            defaultCompositeValidatorFactory = BuildDefaultCompositeValidatorFactory();
        }

        /// <summary>
        /// Sets the default configuration validator factory for the static <see cref="ValidationFactory"/> by using the specified configuration source.
        /// </summary>
        /// <param name="factoryConfigurationSource">The configuration source for the validation factory.</param>
        public static void SetDefaultConfigurationValidatorFactory(IConfigurationSource factoryConfigurationSource)
        {
            Guard.ArgumentNotNull(factoryConfigurationSource, "factoryConfigurationSource");

            SetDefaultConfigurationValidatorFactory(new ConfigurationValidatorFactory(factoryConfigurationSource));
        }

        /// <summary>
        /// Resets the factories.
        /// </summary>
        /// <remarks>
        /// Used for tests.
        /// </remarks>
        public static void Reset()
        {
            SetDefaultConfigurationValidatorFactory(BuildDefaultConfigurationValidatorFactory());
        }

        /// <summary>
        /// Gets the <see cref="ValidatorFactory"/> to use by default.
        /// </summary>
        public static ValidatorFactory DefaultCompositeValidatorFactory
        {
            get { return defaultCompositeValidatorFactory.Value; }
        }

        /// <summary>
        /// Gets the <see cref="ValidatorFactory"/> to use by default for configuration.
        /// </summary>
        public static ConfigurationValidatorFactory DefaultConfigurationValidatorFactory
        {
            get { return defaultConfigurationValidatorFactory; }
        }

        private static CompositeValidatorFactory CreateCompositeValidatorFactory(IEnumerable<ValidatorFactory> validatorFactories)
        {
            return new CompositeValidatorFactory(validatorFactories.ToArray());
        }

        private static ValidatorFactory GetValidatorFactory(ValidationSpecificationSource source)
        {
            if (source == ValidationSpecificationSource.All || source == ValidationSpecificationSource.Both)
            {
                return DefaultCompositeValidatorFactory;
            }

            List<ValidatorFactory> factories = new List<ValidatorFactory>();
            if (source.IsSet(ValidationSpecificationSource.Attributes))
            {
                factories.Add(DefaultAttributeValidatorFactory);
            }
            if (source.IsSet(ValidationSpecificationSource.Configuration))
            {
                factories.Add(DefaultConfigurationValidatorFactory);
            }
            if (source.IsSet(ValidationSpecificationSource.DataAnnotations))
            {
                factories.Add(DefaultValidationAttributeValidatorFactory);
            }

            if (factories.Count == 1)
            {
                return factories[0];
            }

            return CreateCompositeValidatorFactory(factories);
        }

        private static ValidatorFactory GetValidatorFactory(IConfigurationSource configurationSource, ValidationSpecificationSource source)
        {
            List<ValidatorFactory> factories = new List<ValidatorFactory>();
            if (source.IsSet(ValidationSpecificationSource.Attributes))
            {
                factories.Add(DefaultAttributeValidatorFactory);
            }
            if (source.IsSet(ValidationSpecificationSource.Configuration))
            {
                factories.Add(new ConfigurationValidatorFactory(configurationSource));
            }
            if (source.IsSet(ValidationSpecificationSource.DataAnnotations))
            {
                factories.Add(DefaultValidationAttributeValidatorFactory);
            }

            if (factories.Count == 1)
            {
                return factories[0];
            }

            return CreateCompositeValidatorFactory(factories);
        }

        private static AttributeValidatorFactory DefaultAttributeValidatorFactory
        {
            get { return defaultAttributeValidatorFactory; }
        }

        private static ValidationAttributeValidatorFactory DefaultValidationAttributeValidatorFactory
        {
            get { return defaultValidationAttributeValidatorFactory; }
        }

        private static Lazy<ValidatorFactory> BuildDefaultCompositeValidatorFactory()
        {
            return new Lazy<ValidatorFactory>(
                () => new CompositeValidatorFactory(DefaultAttributeValidatorFactory, DefaultConfigurationValidatorFactory, DefaultValidationAttributeValidatorFactory),
                System.Threading.LazyThreadSafetyMode.PublicationOnly);
        }

        private static ConfigurationValidatorFactory BuildDefaultConfigurationValidatorFactory()
        {
            return new ConfigurationValidatorFactory(EmptyValidationConfigurationSource);
        }
    }
}
