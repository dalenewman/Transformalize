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
using Transformalize.Libs.EnterpriseLibrary.Validation.Validators;

namespace Transformalize.Libs.EnterpriseLibrary.Validation
{
    /// <summary>
    /// Helps building validators composed by other validators.
    /// </summary>
    public class CompositeValidatorBuilder
    {
        private IValidatedElement validatedElement;
        private List<Validator> valueValidators;
        private Validator builtValidator;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositeValidatorBuilder"/> class.
        /// </summary>
        /// <param name="validatedElement">The element for which a composite validator will be created.</param>
        public CompositeValidatorBuilder(IValidatedElement validatedElement)
        {
            this.validatedElement = validatedElement;
            this.valueValidators = new List<Validator>();
        }

        /// <summary>
        /// Returns the validator created by the builder.
        /// </summary>
        public Validator GetValidator()
        {
            this.builtValidator = this.DoGetValidator();

            return this.builtValidator;
        }

        /// <summary>
        /// Creates the composite validator built by the builder.
        /// </summary>
        protected virtual Validator DoGetValidator()
        {
            // create the appropriate validator
            Validator validator;

            if (this.valueValidators.Count == 1)
            {
                validator = this.valueValidators[0];
            }
            else
            {

                if (CompositionType.And == this.validatedElement.CompositionType)
                {
                    validator = new AndCompositeValidator(this.valueValidators.ToArray());
                }
                else
                {
                    validator = new OrCompositeValidator(this.valueValidators.ToArray());
                    validator.MessageTemplate = this.validatedElement.CompositionMessageTemplate;
                    validator.Tag = this.validatedElement.CompositionTag;
                }
            }

            // add support for ignoring nulls
            Validator valueValidator;
            if (this.validatedElement.IgnoreNulls)
            {
                valueValidator = new NullIgnoringValidatorWrapper(validator);
            }
            else
            {
                valueValidator = validator;
            }

            return valueValidator;
        }

        /// <summary>
        /// Adds a value validator to the composite validator.
        /// </summary>
        /// <param name="valueValidator">The validator to add.</param>
        public void AddValueValidator(Validator valueValidator)
        {
            this.valueValidators.Add(valueValidator);
        }

        #region test only properties

        /// <summary>
        /// This member supports the Enterprise Library infrastructure and is not intended to be used directly from your code.
        /// </summary>
        public bool IgnoreNulls
        {
            get { return this.validatedElement.IgnoreNulls; }
        }

        /// <summary>
        /// This member supports the Enterprise Library infrastructure and is not intended to be used directly from your code.
        /// </summary>
        public CompositionType CompositionType
        {
            get { return this.validatedElement.CompositionType; }
        }

        /// <summary>
        /// This member supports the Enterprise Library infrastructure and is not intended to be used directly from your code.
        /// </summary>
        public Validator BuiltValidator
        {
            get { return this.builtValidator; }
        }

        /// <summary>
        /// This member supports the Enterprise Library infrastructure and is not intended to be used directly from your code.
        /// </summary>
        public IList<Validator> ValueValidators
        {
            get { return this.valueValidators; }
        }

        #endregion
    }
}
