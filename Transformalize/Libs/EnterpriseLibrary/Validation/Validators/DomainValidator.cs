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
	/// Validates an object by checking if it belongs to a set.
	/// </summary>
	[ConfigurationElementType(typeof(DomainValidatorData))]
	public class DomainValidator<T> : ValueValidator<T>
	{
		private IEnumerable<T> domain;

		/// <summary>
		/// <para>Initializes a new instance of the <see cref="DomainValidator{T}"/>.</para>
		/// </summary>
		/// <param name="domain">List of values to be used in the validation.</param>
		public DomainValidator(IEnumerable<T> domain)
			: this(domain, false)
		{ }

		/// <summary>
		/// <para>Initializes a new instance of the <see cref="DomainValidator{T}"/>.</para>
		/// </summary>
		/// <param name="negated">True if the validator must negate the result of the validation.</param>
		/// <param name="domain">List of values to be used in the validation.</param>
		public DomainValidator(bool negated, params T[] domain)
			: this(new List<T>(domain), null, negated)
		{
		}

		/// <summary>
		/// <para>Initializes a new instance of the <see cref="DomainValidator{T}"/>.</para>
		/// </summary>
		/// <param name="messageTemplate">The message template to use when logging results.</param>
		/// <param name="domain">List of values to be used in the validation.</param>
		public DomainValidator(string messageTemplate, params T[] domain)
			: this(new List<T>(domain), messageTemplate, false)
		{ }

		/// <summary>
		/// <para>Initializes a new instance of the <see cref="DomainValidator{T}"/>.</para>
		/// </summary>
		/// <param name="domain">List of values to be used in the validation.</param>
		/// <param name="negated">True if the validator must negate the result of the validation.</param>
		public DomainValidator(IEnumerable<T> domain, bool negated)
			: this(domain, null, negated)
		{
		}

		/// <summary>
		/// <para>Initializes a new instance of the <see cref="DomainValidator{T}"/>.</para>
		/// </summary>
		/// <param name="domain">List of values to be used in the validation.</param>
		/// <param name="messageTemplate">The message template to use when logging results.</param>
		/// <param name="negated">True if the validator must negate the result of the validation.</param>
		public DomainValidator(IEnumerable<T> domain, string messageTemplate, bool negated)
			: base(messageTemplate, null, negated)
		{
			ValidatorArgumentsValidatorHelper.ValidateDomainValidator(domain);

			this.domain = domain;
		}

		/// <summary>
		/// Implements the validation logic for the receiver.
		/// </summary>
		/// <param name="objectToValidate">The object to validate.</param>
		/// <param name="currentTarget">The object on the behalf of which the validation is performed.</param>
		/// <param name="key">The key that identifies the source of <paramref name="objectToValidate"/>.</param>
		/// <param name="validationResults">The validation results to which the outcome of the validation should be stored.</param>
		protected override void DoValidate(T objectToValidate, object currentTarget, string key, ValidationResults validationResults)
		{
			bool logError = false;
			bool isObjectToValidateNull = objectToValidate == null;

			if (!isObjectToValidateNull)
			{
				logError = true;
				foreach (T element in domain)
				{
					if (element.Equals(objectToValidate))
					{
						logError = false;
						break;
					}
				}
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
		/// Gets the Default Message Template when the validator is non negated.
		/// </summary>
		protected override string DefaultNonNegatedMessageTemplate
		{
			get { return Resources.DomainNonNegatedDefaultMessageTemplate; }
		}

		/// <summary>
		/// Gets the Default Message Template when the validator is negated.
		/// </summary>
		protected override string DefaultNegatedMessageTemplate
		{
			get { return Resources.DomainNegatedDefaultMessageTemplate; }
		}

        /// <summary>
        /// The set of items we're checking for membership in.
        /// </summary>
		public List<T> Domain
		{
			get { return new List<T>(this.domain); }
		}
	}
}
