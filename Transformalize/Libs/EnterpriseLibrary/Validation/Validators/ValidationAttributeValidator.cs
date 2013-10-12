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
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Globalization;

namespace Transformalize.Libs.EnterpriseLibrary.Validation.Validators
{
    /// <summary>
    /// Validator wrapping a collection of <see cref="ValidationAttribute"/>.
    /// </summary>
    public class ValidationAttributeValidator : Validator
    {
        private IEnumerable<ValidationAttribute> validationAttributes;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="validationAttributes"></param>
        public ValidationAttributeValidator(params ValidationAttribute[] validationAttributes)
            : this((IEnumerable<ValidationAttribute>)validationAttributes)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="ValidationAttributeValidator"/> with a 
        /// <see cref="ValidationAttribute"/>.
        /// </summary>
        /// <param name="validationAttributes">The validation attributes to wrap.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly", Justification = "Properly instantiated. False positive caused by using a lambda expression.")]
        public ValidationAttributeValidator(IEnumerable<ValidationAttribute> validationAttributes)
            : base(null, null)
        {
            if (validationAttributes == null) throw new ArgumentNullException("validationAttributes");

            this.validationAttributes =
                validationAttributes.Select(
                    va =>
                    {
                        if (va == null)
                        {
                            throw new ArgumentException(Resources.ExceptionContainsNullElements, "validationAttributes");
                        }
                        return va;
                    }).ToList();
        }

        /// <summary>
        /// Validates by forwarding the validation request to the wrapped <see cref="ValidationAttribute"/>.
        /// </summary>
        /// <param name="objectToValidate">The object to validate.</param>
        /// <param name="currentTarget">The object on the behalf of which the validation is performed.</param>
        /// <param name="key">The key that identifies the source of <paramref name="objectToValidate"/>.</param>
        /// <param name="validationResults">The validation results to which the outcome of the validation should be stored.</param>
        public override void DoValidate(object objectToValidate, object currentTarget, string key, ValidationResults validationResults)
        {
            foreach (var validationAttribute in this.validationAttributes)
            {
                try
                {
                    if (!validationAttribute.IsValid(objectToValidate))
                    {
                        this.LogValidationResult(validationResults, validationAttribute.FormatErrorMessage(key), currentTarget, key);
                    }
                }
                catch (Exception e)
                {
                    string message =
                        string.Format(
                            CultureInfo.CurrentCulture,
                            Resources.ValidationAttributeFailed,
                            validationAttribute.GetType().Name,
                            e.Message);
                    LogValidationResult(validationResults, message, currentTarget, key);
                }
            }
        }

        /// <summary>
        /// Gets the message template to use when logging results no message is supplied.
        /// </summary>
        /// <remarks>
        /// Not used by this validator.
        /// </remarks>
        protected override string DefaultMessageTemplate
        {
            get { return null; }
        }
    }
}
