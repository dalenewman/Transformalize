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
using Transformalize.Libs.EnterpriseLibrary.Common.Configuration.Design.Validation;
using Transformalize.Libs.EnterpriseLibrary.Validation.Validators;

namespace Transformalize.Libs.EnterpriseLibrary.Validation
{
    /// <summary>
    /// Builder of validators from attributes inheriting from <see cref="ValidationAttribute"/>.
    /// </summary>
    /// <seealso cref="ValidationAttributeValidator"/>
    public class ValidationAttributeValidatorBuilder : ValidatorBuilderBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationAttributeValidatorBuilder"/> class.
        /// </summary>
        /// <param name="memberAccessValidatorFactory"></param>
        /// <param name="validatorFactory">Factory to use when building nested validators.</param>
        public ValidationAttributeValidatorBuilder(
            MemberAccessValidatorBuilderFactory memberAccessValidatorFactory,
            ValidatorFactory validatorFactory)
            : base(memberAccessValidatorFactory, validatorFactory)
        { }

        /// <summary>
        /// Creates a validator for the supplied type.
        /// </summary>
        /// <param name="type">The type for which the validator should be created.</param>
        /// <returns>A validator.</returns>
        public Validator CreateValidator(Type type)
        {
            return CreateValidator(new ValidationAttributeValidatedType(type));
        }

        internal Validator CreateValidatorForProperty(PropertyInfo propertyInfo)
        {
            return CreateValidatorForValidatedElement(
                new ValidationAttributeValidatedElement(propertyInfo),
                this.GetCompositeValidatorBuilderForProperty);
        }
    }
}
