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
using System.Collections;
using System.Collections.Generic;

namespace Transformalize.Libs.EnterpriseLibrary.Validation
{
	/// <summary>
	/// Represents the result of validating an object.
	/// </summary>
	[Serializable]
	public class ValidationResults : IEnumerable<ValidationResult>
	{
		private List<ValidationResult> validationResults;

		/// <summary>
		/// <para>Initializes a new instance of the <see cref="ValidationResults"/> class with the section name.</para>
		/// </summary>
		public ValidationResults()
		{
			validationResults = new List<ValidationResult>();
		}

		/// <summary>
		/// <para>Adds a <see cref="ValidationResult"/>.</para>
		/// </summary>
		/// <param name="validationResult">The validation result to add.</param>
		public void AddResult(ValidationResult validationResult)
		{
			this.validationResults.Add(validationResult);
		}

		/// <summary>
		/// <para>Adds all the <see cref="ValidationResult"/> instances from <paramref name="sourceValidationResults"/>.</para>
		/// </summary>
		/// <param name="sourceValidationResults">The source for validation results to add.</param>
		public void AddAllResults(IEnumerable<ValidationResult> sourceValidationResults)
		{
			this.validationResults.AddRange(sourceValidationResults);
		}

		/// <summary>
		/// Returns a new instance of <see cref="ValidationResults"/> that includes the results from the receiver that
		/// match the provided tag names.
		/// </summary>
		/// <param name="tagFilter">The indication of whether to include or ignore the matching results.</param>
		/// <param name="tags">The list of tag names to match.</param>
		/// <returns>A <see cref="ValidationResults"/> containing the filtered results.</returns>
		public ValidationResults FindAll(TagFilter tagFilter, params string[] tags)
		{
			// workaround for params behavior - a single null parameter will be interpreted 
			// as null array, not as an array with null as element
			if (tags == null)
			{
				tags = new string[] { null };
			}

			ValidationResults filteredValidationResults = new ValidationResults();

			foreach (ValidationResult validationResult in this)
			{
				bool matches = false;

				foreach (string tag in tags)
				{
					if ((tag == null && validationResult.Tag == null)
						|| (tag != null && tag.Equals(validationResult.Tag)))
					{
						matches = true;
						break;
					}
				}

				// if ignore, look for !match
				// if include, look for match
				if (matches ^ (tagFilter == TagFilter.Ignore))
				{
					filteredValidationResults.AddResult(validationResult);
				}
			}

			return filteredValidationResults;
		}

		/// <summary>
		/// Gets the indication of whether the validation represented by the receiver was successful.
		/// </summary>
		/// <remarks>
		/// An unsuccessful validation will be represented by a <see cref="ValidationResult"/> instance with
		/// <see cref="ValidationResult"/> elements, regardless of these elements' tags.
		/// </remarks>
		public bool IsValid
		{
			get { return validationResults.Count == 0; }
		}

		/// <summary>
		/// Gets the count of results.
		/// </summary>
		public int Count
		{
			get { return this.validationResults.Count; }
		}

		IEnumerator<ValidationResult> IEnumerable<ValidationResult>.GetEnumerator()
		{
			return validationResults.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return validationResults.GetEnumerator();
		}
	}
}
