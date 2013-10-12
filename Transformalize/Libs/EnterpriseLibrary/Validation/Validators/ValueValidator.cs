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
	/// Base class for validators on simple values.
	/// </summary>
	public abstract class ValueValidator : Validator
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ValueValidator"/> class.
		/// </summary>
		/// <param name="messageTemplate">The template to use when logging validation results, or <see langword="null"/> we the
		/// default message template is to be used.</param>
		/// <param name="tag">The tag to set when logging validation results, or <see langword="null"/>.</param>
		/// <param name="negated">Indicates if the validation logic represented by the validator should be negated.</param>
		protected ValueValidator(string messageTemplate, string tag, bool negated)
			: base(messageTemplate, tag)
		{
			this.negated = negated;
		}

		private bool negated;
		/// <summary>
		/// Gets the indication of negated validation logic.
		/// </summary>
		/// <value><see langword="true"/> if the default validation logic should be used; othwerise <see langword="false"/>.</value>
		public bool Negated
		{
			get { return negated; }
		}

		/// <summary>
		/// Gets the default message template for de validator.
		/// </summary>
		protected sealed override string DefaultMessageTemplate
		{
			get
			{
				if (this.negated)
				{
					return DefaultNegatedMessageTemplate;
				}
				else
				{
					return DefaultNonNegatedMessageTemplate;
				}
			}
		}

		/// <summary>
		/// Gets the Default Message Template when the validator is non negated.
		/// </summary>
		protected abstract string DefaultNonNegatedMessageTemplate { get; }

		/// <summary>
		/// Gets the Default Message Template when the validator is negated.
		/// </summary>
		protected abstract string DefaultNegatedMessageTemplate { get;}
	}
}
