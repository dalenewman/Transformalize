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

// ReSharper disable InconsistentNaming

namespace Transformalize.Libs.EnterpriseLibrary.Validation.Validators
{
	/// <summary>
	/// Represents the different comparison behaviors available for a <see cref="PropertyComparisonValidator"/>.
	/// </summary>
	public enum ComparisonOperator
	{
		/// <summary>
		/// Comparison for equal.
		/// </summary>
		Equal,

        /// <summary>
		/// Comparison for not equal.
		/// </summary>
		NotEqual,

		/// <summary>
		/// Comparison for greater.
		/// </summary>
		GreaterThan,

		/// <summary>
		/// Comparison for greater or equal.
		/// </summary>
		GreaterThanEqual,

		/// <summary>
		/// Comparison for lesser.
		/// </summary>
		LessThan,

		/// <summary>
		/// Comparison for lesser or equal.
		/// </summary>
		LessThanEqual,

	}
}
