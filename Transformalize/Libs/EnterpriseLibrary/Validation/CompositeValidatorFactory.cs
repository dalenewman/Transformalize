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
using Transformalize.Libs.EnterpriseLibrary.Common.Utility;
using Transformalize.Libs.EnterpriseLibrary.Validation.Validators;

namespace Transformalize.Libs.EnterpriseLibrary.Validation
{
    ///<summary>
    /// An instance factory for creating validators based on other validtor factories.
    ///</summary>
    public class CompositeValidatorFactory : ValidatorFactory
    {
        private readonly IEnumerable<ValidatorFactory> validatorFactories;

        ///<summary>
        /// Initializes a composite validator factory from an array of <see cref="ValidatorFactory"/> instances.
        ///</summary>
        ///<param name="validatorFactories">One or more validator factories to compose from.</param>
        public CompositeValidatorFactory(IEnumerable<ValidatorFactory> validatorFactories)
        {
            this.validatorFactories = validatorFactories;
        }

        ///<summary>
        /// Initializes a composite validator factory from attribute and configuration validator factories
        ///</summary>
        ///<param name="attributeValidatorFactory">The <see cref="AttributeValidatorFactory"/> to composite.</param>
        ///<param name="configurationValidatorFactory">The <see cref="ConfigurationValidatorFactory"/> to composite.</param>
        public CompositeValidatorFactory(
            AttributeValidatorFactory attributeValidatorFactory,
            ConfigurationValidatorFactory configurationValidatorFactory)
            : this(new ValidatorFactory[] { attributeValidatorFactory, configurationValidatorFactory })
        {
        }

        ///<summary>
        /// Initializes a composite validator factory from attribute and configuration validator factories
        ///</summary>
        ///<param name="attributeValidatorFactory">The <see cref="AttributeValidatorFactory"/> to composite.</param>
        ///<param name="configurationValidatorFactory">The <see cref="ConfigurationValidatorFactory"/> to composite.</param>
        ///<param name="validationAttributeValidatorFactory">The <see cref="ValidationAttributeValidatorFactory"/> to composite.</param>
        public CompositeValidatorFactory(
            AttributeValidatorFactory attributeValidatorFactory,
            ConfigurationValidatorFactory configurationValidatorFactory,
            ValidationAttributeValidatorFactory validationAttributeValidatorFactory)
            : this(new ValidatorFactory[] { attributeValidatorFactory, configurationValidatorFactory, validationAttributeValidatorFactory })
        {
        }

        /// <summary>
        /// Creates the validator for the specified target and ruleset.
        /// </summary>
        /// <param name="targetType">The <see cref="Type"/>to validate.</param>
        /// <param name="ruleset">The ruleset to use when validating</param>
        /// <param name="mainValidatorFactory">Factory to use when building nested validators.</param>
        /// <returns>A <see cref="Validator"/></returns>
        protected internal override Validator InnerCreateValidator(Type targetType, string ruleset, ValidatorFactory mainValidatorFactory)
        {
            Validator validator =
                GetValidator(validatorFactories.Select(f => f.InnerCreateValidator(targetType, ruleset, mainValidatorFactory)));

            return validator;
        }

        ///<summary>
        /// Clears the internal validator cache.
        ///</summary>
        public override void ResetCache()
        {
            validatorFactories.ForEach(f => f.ResetCache());
            base.ResetCache();
        }

        private static Validator GetValidator(IEnumerable<Validator> validators)
        {
            var validValidators = validators.Where(CheckIfValidatorIsAppropiate);

            if (validValidators.Count() == 1)
            {
                return validValidators.First();
            }

            return new AndCompositeValidator(validValidators.ToArray());
        }

        private static bool CheckIfValidatorIsAppropiate(Validator validator)
        {
            if (IsComposite(validator))
            {
                return CompositeHasValidators(validator);
            }
            else
            {
                return true;
            }
        }

        private static bool IsComposite(Validator validator)
        {
            return validator is AndCompositeValidator || validator is OrCompositeValidator;
        }

        private static bool CompositeHasValidators(Validator validator)
        {
            AndCompositeValidator andValidator = validator as AndCompositeValidator;

            if (andValidator != null)
            {
                return ((Validator[])andValidator.Validators).Length > 0;
            }

            OrCompositeValidator orValidator = validator as OrCompositeValidator;

            if (orValidator != null)
            {
                return ((Validator[])orValidator.Validators).Length > 0;
            }

            return false;
        }
    }
}
