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

namespace Transformalize.Libs.EnterpriseLibrary.Validation
{
	/// <summary>
	/// Represents the result of an atomic validation.
	/// </summary>
	[Serializable]
	public class ValidationResult
	{
		private string message;
		private string key;
		private string tag;
		[NonSerialized]
		private object target;
		[NonSerialized]
		private Validator validator;
		private IEnumerable<ValidationResult> nestedValidationResults;

		private static readonly IEnumerable<ValidationResult> NoNestedValidationResults = new ValidationResult[0];

		/// <summary>
		/// Initializes this object with a message.
		/// </summary>
		public ValidationResult(string message, object target, string key, string tag, Validator validator)
			: this(message, target, key, tag, validator, NoNestedValidationResults)
		{ }


		/// <summary>
		/// Initializes this object with a message.
		/// </summary>
		public ValidationResult(string message, object target, string key, string tag, Validator validator,
			IEnumerable<ValidationResult> nestedValidationResults)
		{
			this.message = message;
			this.key = key;
			this.target = target;
			this.tag = tag;
			this.validator = validator;
			this.nestedValidationResults = nestedValidationResults;
		}

		/// <summary>
		/// Gets a name describing the location of the validation result.
		/// </summary>
		public string Key
		{
			get { return this.key; }
		}

		/// <summary>
		/// Gets a message describing the failure.
		/// </summary>
		public string Message
		{
			get { return this.message; }
		}

		/// <summary>
		/// Gets a value characterizing the result.
		/// </summary>
		/// <remarks>
		/// The meaning for a tag is determined by the client code consuming the <see cref="ValidationResults"/>.
		/// </remarks>
		/// <seealso cref="ValidationResults.FindAll"/>
		public string Tag
		{
			get { return tag; }
		}

		/// <summary>
		/// Gets the object to which the validation rule was applied.
		/// </summary>
		/// <remarks>
		/// This object might not be the object for which validation was requested initially.
		/// </remarks>
		public object Target
		{
			get { return this.target; }
		}

		/// <summary>
		/// Gets the validator that performed the failing validation.
		/// </summary>
		public Validator Validator
		{
			get { return this.validator; }
		}

		/// <summary>
		/// Gets the nested validation results for a composite failed validation.
		/// </summary>
		public IEnumerable<ValidationResult> NestedValidationResults
		{
			get { return this.nestedValidationResults; }
		}
	}
}
