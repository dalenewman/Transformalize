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

namespace Transformalize.Libs.EnterpriseLibrary.Validation.Validators
{
    /// <summary>
    /// Validator that succeeds on null values and delegates validation of non-null values to another validator.
    /// </summary>
    public class NullIgnoringValidatorWrapper : Validator
    {
        private readonly Validator wrappedValidator;

        /// <summary>
        /// Initializes a new instance of the <see cref="NullIgnoringValidatorWrapper"/> with a validator to wrap.
        /// </summary>
        /// <param name="wrappedValidator">The validator to wrap.</param>
        public NullIgnoringValidatorWrapper(Validator wrappedValidator)
            : base(string.Empty, string.Empty)
        {
            this.wrappedValidator = wrappedValidator;
        }

        /// <summary>
        /// Implements the validation logic for the receiver.
        /// </summary>
        /// <param name="objectToValidate">The object to validate.</param>
        /// <param name="currentTarget">The object on the behalf of which the validation is performed.</param>
        /// <param name="key">The key that identifies the source of <paramref name="objectToValidate"/>.</param>
        /// <param name="validationResults">The validation results to which the outcome of the validation should be stored.</param>
        public override void DoValidate(object objectToValidate, object currentTarget, string key, ValidationResults validationResults)
        {
            if (objectToValidate != null)
            {
                this.wrappedValidator.DoValidate(objectToValidate, currentTarget, key, validationResults);
            }
        }

        /// <summary>
        /// Gets the message template to use when logging results no message is supplied.
        /// </summary>
        /// <remarks>
        /// Not used for this validator.
        /// </remarks>
        protected override string DefaultMessageTemplate
        {
            get { return string.Empty; }
        }

        /// <summary>
        /// Test-only property. Gets the wrapped validator.
        /// </summary>
        public Validator WrappedValidator
        {
            get { return this.wrappedValidator; }
        }
    }
}
