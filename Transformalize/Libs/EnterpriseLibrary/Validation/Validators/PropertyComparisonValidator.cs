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

using Transformalize.Libs.EnterpriseLibrary.Common.Configuration;
using Transformalize.Libs.EnterpriseLibrary.Validation.Configuration;

namespace Transformalize.Libs.EnterpriseLibrary.Validation.Validators
{
	/// <summary>
	/// Performs validation by comparing the a value with the value of a property on the target
	/// object by using a specified comparison operation.
	/// </summary>
	[ConfigurationElementType(typeof(PropertyComparisonValidatorData))]
	public class PropertyComparisonValidator : ValueAccessComparisonValidator
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="PropertyComparisonValidator"/> class.
		/// </summary>
		/// <param name="valueAccess">The <see cref="ValueAccess"/> to use to extract the value to compare.</param>
		/// <param name="comparisonOperator">The <see cref="ComparisonOperator"/> representing the kind of comparison to perform.</param>
		public PropertyComparisonValidator(ValueAccess valueAccess, ComparisonOperator comparisonOperator)
			: base(valueAccess, comparisonOperator)
		{ }

		/// <summary>
		/// Initializes a new instance of the <see cref="PropertyComparisonValidator"/> class.
		/// </summary>
		/// <param name="valueAccess">The <see cref="ValueAccess"/> to use to extract the value to compare.</param>
		/// <param name="comparisonOperator">The <see cref="ComparisonOperator"/> representing the kind of comparison to perform.</param>
		/// <param name="negated">Indicates if the validation logic represented by the validator should be negated.</param>
		public PropertyComparisonValidator(ValueAccess valueAccess, ComparisonOperator comparisonOperator, bool negated)
			: base(valueAccess, comparisonOperator, null, negated)
		{ }
	}
}
