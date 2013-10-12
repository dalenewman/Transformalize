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

using System.Collections.Generic;
using Transformalize.Libs.EnterpriseLibrary.Common.Configuration;
using Transformalize.Libs.EnterpriseLibrary.Validation.Configuration;

namespace Transformalize.Libs.EnterpriseLibrary.Validation.Validators
{
    /// <summary>
    /// Aggregates a set of <see cref="Validator"/> instances, performing validation by allowing each validator to perform its own.
    /// </summary>
    /// <remarks>
    /// Validation fails if any of the composed validators fails.
    /// </remarks>
    [ConfigurationElementType(typeof(AndCompositeValidatorData))]
    public class AndCompositeValidator : Validator
    {
        private IEnumerable<Validator> validators;

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="AndCompositeValidator"/> class.</para>
        /// </summary>
        /// <param name="validators">The validators to be composed by the created instance.</param>
        public AndCompositeValidator(params Validator[] validators)
            : base(null, null)
        {
            this.validators = validators;
        }

        /// <summary>
        /// Implements the validation logic for the receiver, invoking validation on the composed validators.
        /// </summary>
        /// <param name="objectToValidate">The object to validate.</param>
        /// <param name="currentTarget">The object on the behalf of which the validation is performed.</param>
        /// <param name="key">The key that identifies the source of <paramref name="objectToValidate"/>.</param>
        /// <param name="validationResults">The validation results to which the outcome of the validation should be stored.</param>
        /// <remarks>
        /// All the composed validators' results will be logged.
        /// </remarks>
        public override void DoValidate(object objectToValidate,
            object currentTarget,
            string key,
            ValidationResults validationResults)
        {
            foreach (Validator validator in this.validators)
            {
                validator.DoValidate(objectToValidate, currentTarget, key, validationResults);
            }
        }

        /// <summary>
        /// Gets the message template to use when logging results no message is supplied.
        /// </summary>
        /// <remarks>
        /// This validator does not log messages of its own.
        /// </remarks>
        protected override string DefaultMessageTemplate
        {
            get { return null; }
        }

        /// <summary>
        /// The children of this validator that are run with tthe results anded together.
        /// </summary>
        public IEnumerable<Validator> Validators
        {
            get
            {
                return this.validators;
            }
        }
    }
}
