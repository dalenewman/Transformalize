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
	/// Base generic validator for member access validation.
	/// </summary>
	/// <typeparam name="T">The type for which validation on a member is to be performed.</typeparam>
	public abstract class MemberAccessValidator<T> : Validator<T>
	{
		private ValueAccessValidator valueAccessValidator;

		/// <summary>
		/// <para>Initializes a new instance of the <see cref="MemberAccessValidator{T}"/> class.</para>
		/// </summary>
		/// <param name="valueAccess">The <see cref="ValueAccess"/> to use when accessing the
		/// value of the validated member.</param>
		/// <param name="valueValidator">The <see cref="Validator"/> to validate the value of the
		/// validated member.</param>
		protected MemberAccessValidator(ValueAccess valueAccess, Validator valueValidator)
			: base(null, null)
		{
			this.valueAccessValidator = new ValueAccessValidator(valueAccess, valueValidator);
		    Validator = valueValidator;
		}

		/// <summary>
		/// Validates extracting a value from <paramref name="objectToValidate"/> and applying a validator
		/// to it.
		/// </summary>
		/// <param name="objectToValidate">The object to validate.</param>
		/// <param name="currentTarget">The object on the behalf of which the validation is performed.</param>
		/// <param name="key">The key that identifies the source of <paramref name="objectToValidate"/>.</param>
		/// <param name="validationResults">The validation results to which the outcome of the validation should be stored.</param>
		protected override void DoValidate(T objectToValidate, object currentTarget, string key, 
			ValidationResults validationResults)
		{
			this.valueAccessValidator.DoValidate(objectToValidate, currentTarget, key, validationResults);
		}

		/// <summary>
		/// Gets the message template to use when logging results no message is supplied.
		/// </summary>
		protected override string DefaultMessageTemplate
		{
			get { return null; }
		}

        /// <summary>
        /// The actual validator that gets run against this field.
        /// </summary>
        public Validator Validator
        {
            get; private set;
        }

        /// <summary>
        /// Key used to access the value.
        /// </summary>
        public string Key
        {
            get { return valueAccessValidator.Key; }
        }
	}
}
