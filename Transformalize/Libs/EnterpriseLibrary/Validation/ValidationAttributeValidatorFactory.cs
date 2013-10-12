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
using Transformalize.Libs.EnterpriseLibrary.Validation.Validators;

namespace Transformalize.Libs.EnterpriseLibrary.Validation
{
    ///<summary>
    /// A <see cref="Validator"/> factory producing validators from rules specified in a configuration file.
    ///</summary>
    /// <seealso cref="ValidatorFactory"/>
    public class ValidationAttributeValidatorFactory : ValidatorFactory
    {
        private static readonly Validator EmptyValidator = new AndCompositeValidator();

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationAttributeValidatorFactory"/> class.
        /// </summary>
        public ValidationAttributeValidatorFactory()
        { }

        /// <summary>
        /// Creates the validator for the specified target and ruleset.
        /// </summary>
        /// <param name="targetType">The <see cref="Type"/>to validate.</param>
        /// <param name="ruleset">The ruleset to use when validating.</param>
        /// <param name="mainValidatorFactory">Factory to use when building nested validators.</param>
        /// <returns>A <see cref="Validator"/>.</returns>
        protected internal override Validator InnerCreateValidator(Type targetType, string ruleset, ValidatorFactory mainValidatorFactory)
        {
            if (string.IsNullOrEmpty(ruleset))
            {
                ValidationAttributeValidatorBuilder builder =
                    new ValidationAttributeValidatorBuilder(MemberAccessValidatorBuilderFactory.Default, mainValidatorFactory);

                return builder.CreateValidator(targetType);
            }
            else
            {
                return EmptyValidator;
            }
        }
    }
}
