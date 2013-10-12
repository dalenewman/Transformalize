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

namespace Transformalize.Libs.EnterpriseLibrary.Validation
{
    /// <summary>
    /// Facade for validation services.
    /// </summary>
    public static class Validation
    {
        /// <summary>
        /// Validates <paramref name="target"/> using validation criteria specified for type <typeparamref name="T"/>
        /// through configuration and attributes on type <typeparamref name="T"/> and its ancestors for the default ruleset.
        /// </summary>
        /// <typeparam name="T">The type of object to validate.</typeparam>
        /// <param name="target">The instance of <typeparamref name="T"/> to validate.</param>
        /// <returns>A collection of with the results of the individual validations.</returns>
        public static ValidationResults Validate<T>(T target)
        {
            return Validate<T>(target, ValidationSpecificationSource.All);
        }

        /// <summary>
        /// Validates <paramref name="target"/> using validation criteria specified for type <typeparamref name="T"/>
        /// through configuration and attributes on type <typeparamref name="T"/> and its ancestors for the supplied ruleset.
        /// </summary>
        /// <typeparam name="T">The type of object to validate.</typeparam>
        /// <param name="target">The instance of <typeparamref name="T"/> to validate.</param>
        /// <param name="rulesets">The rulesets to use when validating.</param>
        /// <returns>A collection of with the results of the individual validations.</returns>
        /// <exception cref="ArgumentNullException">when the <paramref name="rulesets"/> is <see langword="null"/>.</exception>
        public static ValidationResults Validate<T>(T target, params string[] rulesets)
        {
            return Validate<T>(target, ValidationSpecificationSource.All, rulesets);
        }

        /// <summary>
        /// Validates <paramref name="target"/> using validation criteria specified for type <typeparamref name="T"/>
        /// through configuration and attributes on type <typeparamref name="T"/> and its ancestors for the default ruleset.
        /// </summary>
        /// <typeparam name="T">The type of object to validate.</typeparam>
        /// <param name="target">The instance of <typeparamref name="T"/> to validate.</param>
        /// <param name="source">The source of validation information.</param>
        /// <returns>A collection of with the results of the individual validations.</returns>
        public static ValidationResults Validate<T>(T target, ValidationSpecificationSource source)
        {
            Type targetType = target != null ? target.GetType() : typeof(T);

            Validator validator = ValidationFactory.CreateValidator(targetType, source);

            return validator.Validate(target);
        }

        /// <summary>
        /// Validates <paramref name="target"/> using validation criteria specified for type <typeparamref name="T"/>
        /// through configuration and attributes on type <typeparamref name="T"/> and its ancestors for the supplied ruleset.
        /// </summary>
        /// <typeparam name="T">The type of object to validate.</typeparam>
        /// <param name="target">The instance of <typeparamref name="T"/> to validate.</param>
        /// <param name="source">The source of validation information.</param>
        /// <param name="rulesets">The rulesets to use when validating.</param>
        /// <returns>A collection of with the results of the individual validations.</returns>
        /// <exception cref="ArgumentNullException">when the <paramref name="rulesets"/> is <see langword="null"/>.</exception>
        public static ValidationResults Validate<T>(T target, ValidationSpecificationSource source, params string[] rulesets)
        {
            if (rulesets == null)
            {
                throw new ArgumentNullException("rulesets");
            }

            Type targetType = target != null ? target.GetType() : typeof(T);
            ValidationResults resultsReturned = new ValidationResults();
            foreach (string ruleset in rulesets)
            {
                Validator validator = ValidationFactory.CreateValidator(targetType, ruleset, source);
                foreach (ValidationResult validationResult in validator.Validate(target))
                {
                    resultsReturned.AddResult(validationResult);
                }
            }
            return resultsReturned;
        }

        /// <summary>
        /// Validates <paramref name="target"/> using validation criteria specified for type <typeparamref name="T"/>
        /// through attributes on type <typeparamref name="T"/> and its ancestors for the default ruleset.
        /// </summary>
        /// <typeparam name="T">The type of object to validate.</typeparam>
        /// <param name="target">The instance of <typeparamref name="T"/> to validate.</param>
        /// <returns>A collection of with the results of the individual validations.</returns>
        public static ValidationResults ValidateFromAttributes<T>(T target)
        {
            Validator<T> validator = ValidationFactory.CreateValidatorFromAttributes<T>();

            return validator.Validate(target);
        }

        /// <summary>
        /// Validates <paramref name="target"/> using validation criteria specified for type <typeparamref name="T"/>
        /// through attributes on type <typeparamref name="T"/> and its ancestors for the supplied ruleset.
        /// </summary>
        /// <typeparam name="T">The type of object to validate.</typeparam>
        /// <param name="target">The instance of <typeparamref name="T"/> to validate.</param>
        /// <param name="rulesets">The rulesets to use when validating.</param>
        /// <returns>A collection of with the results of the individual validations.</returns>
        /// <exception cref="ArgumentNullException">when the <paramref name="rulesets"/> is <see langword="null"/>.</exception>
        public static ValidationResults ValidateFromAttributes<T>(T target, params string[] rulesets)
        {
            if (rulesets == null)
            {
                throw new ArgumentNullException("rulesets");
            }

            ValidationResults resultsReturned = new ValidationResults();
            foreach (string ruleset in rulesets)
            {
                Validator<T> validator = ValidationFactory.CreateValidatorFromAttributes<T>(ruleset);

                ValidationResults results = validator.Validate(target);

                foreach (ValidationResult validationResult in results)
                {
                    resultsReturned.AddResult(validationResult);
                }
            }
            return resultsReturned;
        }

        /// <summary>
        /// Validates <paramref name="target"/> using validation criteria specified for type <typeparamref name="T"/>
        /// through configuration for the default ruleset.
        /// </summary>
        /// <typeparam name="T">The type of object to validate.</typeparam>
        /// <param name="target">The instance of <typeparamref name="T"/> to validate.</param>
        /// <returns>A collection of with the results of the individual validations.</returns>
        public static ValidationResults ValidateFromConfiguration<T>(T target)
        {
            Validator<T> validator = ValidationFactory.CreateValidatorFromConfiguration<T>();

            return validator.Validate(target);
        }

        /// <summary>
        /// Validates <paramref name="target"/> using validation criteria specified for type <typeparamref name="T"/>
        /// through configuration for the supplied ruleset.
        /// </summary>
        /// <typeparam name="T">The type of object to validate.</typeparam>
        /// <param name="target">The instance of <typeparamref name="T"/> to validate.</param>
        /// <param name="rulesets">The rulesets to use when validating.</param>
        /// <returns>A collection of with the results of the individual validations.</returns>
        /// <exception cref="ArgumentNullException">when the <paramref name="rulesets"/> is <see langword="null"/>.</exception>
        public static ValidationResults ValidateFromConfiguration<T>(T target, params string[] rulesets)
        {
            if (rulesets == null)
            {
                throw new ArgumentNullException("rulesets");
            }

            ValidationResults resultsReturned = new ValidationResults();
            foreach (string ruleset in rulesets)
            {
                Validator<T> validator = ValidationFactory.CreateValidatorFromConfiguration<T>(ruleset);

                ValidationResults results = validator.Validate(target);

                foreach (ValidationResult validationResult in results)
                {
                    resultsReturned.AddResult(validationResult);
                }
            }
            return resultsReturned;
        }
    }
}
