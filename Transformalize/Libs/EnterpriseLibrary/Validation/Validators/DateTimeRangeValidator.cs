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
using Transformalize.Libs.EnterpriseLibrary.Common.Configuration;
using Transformalize.Libs.EnterpriseLibrary.Validation.Configuration;

namespace Transformalize.Libs.EnterpriseLibrary.Validation.Validators
{
	/// <summary>
	/// Performs validation on <see cref="DateTime"/> instances by comparing them to the specified boundaries. 
	/// </summary>
	/// <remarks>
	/// <see langword="null"/> is logged as a failure.
	/// </remarks>
	[ConfigurationElementType(typeof(DateTimeRangeValidatorData))]
	public class DateTimeRangeValidator : RangeValidator<DateTime>
	{
		/// <summary>
		/// <para>Initializes a new instance of the <see cref="DateTimeRangeValidator"/> class with an upper bound constraint.</para>
		/// </summary>
		/// <param name="upperBound">The upper bound.</param>
		/// <remarks>
		/// No lower bound constraints will be checked by this instance, and the upper bound check will be <see cref="RangeBoundaryType.Inclusive"/>.
		/// </remarks>
		public DateTimeRangeValidator(DateTime upperBound)
			: base(upperBound)
		{ }

		/// <summary>
		/// <para>Initializes a new instance of the <see cref="DateTimeRangeValidator"/> class with an upper bound constraint.</para>
		/// </summary>
		/// <param name="upperBound">The upper bound.</param>
		/// <param name="negated">True if the validator must negate the result of the validation.</param>
		/// <remarks>
		/// No lower bound constraints will be checked by this instance, and the upper bound check will be <see cref="RangeBoundaryType.Inclusive"/>.
		/// </remarks>
		public DateTimeRangeValidator(DateTime upperBound, bool negated)
			: base(upperBound, negated)
		{ }

		/// <summary>
		/// <para>Initializes a new instance of the <see cref="DateTimeRangeValidator"/> class with lower and 
		/// upper bound constraints.</para>
		/// </summary>
		/// <param name="lowerBound">The lower bound.</param>
		/// <param name="upperBound">The upper bound.</param>
		/// <remarks>
		/// Both bound checks will be <see cref="RangeBoundaryType.Inclusive"/>.
		/// </remarks>
		public DateTimeRangeValidator(DateTime lowerBound, DateTime upperBound)
			: base(lowerBound, upperBound)
		{ }

		/// <summary>
		/// <para>Initializes a new instance of the <see cref="DateTimeRangeValidator"/> class with lower and 
		/// upper bound constraints.</para>
		/// </summary>
		/// <param name="lowerBound">The lower bound.</param>
		/// <param name="upperBound">The upper bound.</param>
		/// <param name="negated">True if the validator must negate the result of the validation.</param>
		/// <remarks>
		/// Both bound checks will be <see cref="RangeBoundaryType.Inclusive"/>.
		/// </remarks>
		public DateTimeRangeValidator(DateTime lowerBound, DateTime upperBound, bool negated)
			: base(lowerBound, upperBound, negated)
		{ }

		/// <summary>
		/// <para>Initializes a new instance of the <see cref="DateTimeRangeValidator"/> class with fully specified
		/// bound constraints.</para>
		/// </summary>
		/// <param name="lowerBound">The lower bound.</param>
		/// <param name="lowerBoundType">The indication of how to perform the lower bound check.</param>
		/// <param name="upperBound">The upper bound.</param>
		/// <param name="upperBoundType">The indication of how to perform the upper bound check.</param>
		/// <seealso cref="RangeBoundaryType"/>
		public DateTimeRangeValidator(DateTime lowerBound, RangeBoundaryType lowerBoundType,
			DateTime upperBound, RangeBoundaryType upperBoundType)
			: base(lowerBound, lowerBoundType, upperBound, upperBoundType)
		{ }

		/// <summary>
		/// <para>Initializes a new instance of the <see cref="DateTimeRangeValidator"/> class with fully specified
		/// bound constraints.</para>
		/// </summary>
		/// <param name="lowerBound">The lower bound.</param>
		/// <param name="lowerBoundType">The indication of how to perform the lower bound check.</param>
		/// <param name="upperBound">The upper bound.</param>
		/// <param name="upperBoundType">The indication of how to perform the upper bound check.</param>
		/// <param name="negated">True if the validator must negate the result of the validation.</param>
		/// <seealso cref="RangeBoundaryType"/>
		public DateTimeRangeValidator(DateTime lowerBound, RangeBoundaryType lowerBoundType,
			DateTime upperBound, RangeBoundaryType upperBoundType, bool negated)
			: base(lowerBound, lowerBoundType, upperBound, upperBoundType, negated)
		{ }

		/// <summary>
		/// <para>Initializes a new instance of the <see cref="DateTimeRangeValidator"/> class with fully specified
		/// bound constraints and a message template.</para>
		/// </summary>
		/// <param name="lowerBound">The lower bound.</param>
		/// <param name="lowerBoundType">The indication of how to perform the lower bound check.</param>
		/// <param name="upperBound">The upper bound.</param>
		/// <param name="upperBoundType">The indication of how to perform the upper bound check.</param>
		/// <param name="messageTemplate">The message template to use when logging results.</param>
		/// <seealso cref="RangeBoundaryType"/>
		public DateTimeRangeValidator(DateTime lowerBound, RangeBoundaryType lowerBoundType,
			DateTime upperBound, RangeBoundaryType upperBoundType,
			string messageTemplate)
			: base(lowerBound, lowerBoundType, upperBound, upperBoundType, messageTemplate)
		{ }

		/// <summary>
		/// <para>Initializes a new instance of the <see cref="DateTimeRangeValidator"/> class with fully specified
		/// bound constraints and a message template.</para>
		/// </summary>
		/// <param name="lowerBound">The lower bound.</param>
		/// <param name="lowerBoundType">The indication of how to perform the lower bound check.</param>
		/// <param name="upperBound">The upper bound.</param>
		/// <param name="upperBoundType">The indication of how to perform the upper bound check.</param>
		/// <param name="messageTemplate">The message template to use when logging results.</param>
		/// <param name="negated">True if the validator must negate the result of the validation.</param>
		/// <seealso cref="RangeBoundaryType"/>
		public DateTimeRangeValidator(DateTime lowerBound, RangeBoundaryType lowerBoundType,
			DateTime upperBound, RangeBoundaryType upperBoundType,
			string messageTemplate, bool negated)
			: base(lowerBound, lowerBoundType, upperBound, upperBoundType, messageTemplate, negated)
		{ }
	}
}
