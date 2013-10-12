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

namespace Transformalize.Libs.EnterpriseLibrary.Validation.Validators
{
    /// <summary>
    /// Describes a <see cref="ValueValidator"/>.
    /// </summary>
    /// <seealso cref="ValueValidator"/>
    [AttributeUsage(AttributeTargets.Property
        | AttributeTargets.Field
        | AttributeTargets.Method
        | AttributeTargets.Parameter,
        AllowMultiple = true,
        Inherited = false)]
    public abstract class ValueValidatorAttribute : ValidatorAttribute
    {
        private bool negated;

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="ValueValidatorAttribute"/> class.</para>
        /// </summary>
        protected ValueValidatorAttribute()
        { }

        /// <summary>
        /// Gets or sets the flag indicating if the validator to create should be negated.
        /// </summary>
        public bool Negated
        {
            get { return negated; }
            set { negated = value; }
        }

        /// <summary>
        /// Determines whether the specified value of the object is valid.
        /// </summary>
        /// <param name="value">The value of the specified validation object on which the 
        /// <see cref="System.ComponentModel.DataAnnotations.ValidationAttribute "/> is declared.</param>
        /// <returns><see langword="true"/> if the specified value is valid; otherwise, <see langword="false"/>.</returns>
        public override bool IsValid(object value)
        {
            bool hasRuleset = !string.IsNullOrEmpty(this.Ruleset);
            return hasRuleset || TestIsValid(value);
        }

        /// <summary>
        /// Applies formatting to an error message based on the data field where the error occurred. 
        /// </summary>
        /// <param name="name">The name of the data field where the error occurred.</param>
        /// <returns>An instance of the formatted error message.</returns>
        public override string FormatErrorMessage(string name)
        {
            return this.CreateValidator(null, null, null, null).GetMessage(null, name);
        }

        private bool TestIsValid(object value)
        {
            var validator = DoCreateValidator(null, null, null, null);
            var result = validator.Validate(value);
            return result.IsValid;
        }
    }
}
