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
using System.Globalization;
using Transformalize.Libs.EnterpriseLibrary.Common.Configuration;
using Transformalize.Libs.EnterpriseLibrary.Validation.Configuration;

namespace Transformalize.Libs.EnterpriseLibrary.Validation.Validators
{
	/// <summary>
	/// Validates a string by checking it represents a value for a given enum type.
	/// </summary>
	[ConfigurationElementType(typeof(EnumConversionValidatorData))]
	public class EnumConversionValidator : ValueValidator<string>
	{
		private Type enumType;

		/// <summary>
		/// <para>Initializes a new instance of the <see cref="EnumConversionValidator"/>.</para>
		/// </summary>
		/// <param name="enumType">The enum type to check if the string can be converted.</param>
		public EnumConversionValidator(Type enumType)
			: this(enumType, false)
		{ }

		/// <summary>
		/// <para>Initializes a new instance of the <see cref="EnumConversionValidator"/>.</para>
		/// </summary>
		/// <param name="enumType">The enum type to check if the string can be converted.</param>
		/// <param name="negated">True if the validator must negate the result of the validation.</param>
		public EnumConversionValidator(Type enumType, bool negated)
			: this(enumType, null, negated)
		{ }

		/// <summary>
		/// <para>Initializes a new instance of the <see cref="EnumConversionValidator"/>.</para>
		/// </summary>
		/// <param name="enumType">The enum type to check if the string can be converted.</param>
		/// <param name="messageTemplate">The message template to use when logging results.</param>
		public EnumConversionValidator(Type enumType, string messageTemplate)
			: this(enumType, messageTemplate, false)
		{ }

		/// <summary>
		/// <para>Initializes a new instance of the <see cref="EnumConversionValidator"/>.</para>
		/// </summary>
		/// <param name="enumType">The enum type to check if the string can be converted.</param>
		/// <param name="messageTemplate">The message template to use when logging results.</param>
		/// <param name="negated">True if the validator must negate the result of the validation.</param>
		public EnumConversionValidator(Type enumType, string messageTemplate, bool negated)
			: base(messageTemplate, null, negated)
		{
			ValidatorArgumentsValidatorHelper.ValidateEnumConversionValidator(enumType);

			this.enumType = enumType;
		}

		/// <summary>
		/// Implements the validation logic for the receiver.
		/// </summary>
		/// <param name="objectToValidate">The object to validate.</param>
		/// <param name="currentTarget">The object on the behalf of which the validation is performed.</param>
		/// <param name="key">The key that identifies the source of <paramref name="objectToValidate"/>.</param>
		/// <param name="validationResults">The validation results to which the outcome of the validation should be stored.</param>
		protected override void DoValidate(string objectToValidate, object currentTarget, string key, ValidationResults validationResults)
		{
			bool logError = false;
			bool isObjectToValidateNull = objectToValidate == null;

			if (!isObjectToValidateNull)
			{
				logError = !Enum.IsDefined(enumType, objectToValidate);
			}

			if (isObjectToValidateNull || (logError != Negated))
			{
				this.LogValidationResult(validationResults,
					GetMessage(objectToValidate, key),
					currentTarget,
					key);
			}
		}

		/// <summary>
		/// Gets the message representing a failed validation.
		/// </summary>
		/// <param name="objectToValidate">The object for which validation was performed.</param>
		/// <param name="key">The key representing the value being validated for <paramref name="objectToValidate"/>.</param>
		/// <returns>The message representing the validation failure.</returns>
		protected internal override string GetMessage(object objectToValidate, string key)
		{
			return string.Format(
				CultureInfo.CurrentCulture,
				this.MessageTemplate,
				objectToValidate,
				key,
				this.Tag,
				this.enumType.Name);
		}

		/// <summary>
		/// Gets the Default Message Template when the validator is non negated.
		/// </summary>
		protected override string DefaultNonNegatedMessageTemplate
		{
			get { return Resources.EnumConversionNonNegatedDefaultMessageTemplate; }
		}

		/// <summary>
		/// Gets the Default Message Template when the validator is negated.
		/// </summary>
		protected override string DefaultNegatedMessageTemplate
		{
			get { return Resources.EnumConversionNegatedDefaultMessageTemplate; }
		}

        /// <summary>
        /// Enum type to convert to.
        /// </summary>
        public Type EnumType
		{
			get { return enumType; }
		}
	}
}
