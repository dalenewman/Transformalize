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
	/// Represents a <see cref="RelativeDateTimeValidatorAttribute"/>.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property
		| AttributeTargets.Field
		| AttributeTargets.Method
		| AttributeTargets.Parameter,
		AllowMultiple = true,
		Inherited = false)]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1019",
		Justification = "Fields are used internally")]
	public sealed class RelativeDateTimeValidatorAttribute : ValueValidatorAttribute
	{
		private int lowerBound;
		private DateTimeUnit lowerUnit;
		private RangeBoundaryType lowerBoundType;
		private int upperBound;
		private DateTimeUnit upperUnit;
		private RangeBoundaryType upperBoundType;

		/// <summary>
		/// <para>Initializes a new instance of the <see cref="RelativeDateTimeValidatorAttribute"/>.</para>
		/// </summary>
		/// <param name="upperBound">The upper bound</param>
		/// <param name="upperUnit">The upper bound unit of time.</param>
		public RelativeDateTimeValidatorAttribute(int upperBound, DateTimeUnit upperUnit)
			: this(0, DateTimeUnit.None, RangeBoundaryType.Ignore, upperBound, upperUnit, RangeBoundaryType.Inclusive)
		{ }

		/// <summary>
		/// <para>Initializes a new instance of the <see cref="RelativeDateTimeValidatorAttribute"/>.</para>
		/// </summary>
		/// <param name="upperBound">The upper bound</param>
		/// <param name="upperUnit">The upper bound unit of time.</param>
		/// <param name="upperBoundType">The indication of how to perform the upper bound check.</param>
		public RelativeDateTimeValidatorAttribute(int upperBound, DateTimeUnit upperUnit, RangeBoundaryType upperBoundType)
			: this(0, DateTimeUnit.None, RangeBoundaryType.Ignore, upperBound, upperUnit, upperBoundType)
		{ }

		/// <summary>
		/// <para>Initializes a new instance of the <see cref="RelativeDateTimeValidatorAttribute"/>.</para>
		/// </summary>
		/// <param name="lowerBound">The lower bound.</param>
		/// <param name="lowerUnit">The lower bound unit of time.</param>
		/// <param name="upperBound">The upper bound</param>
		/// <param name="upperUnit">The upper bound unit of time.</param>
		public RelativeDateTimeValidatorAttribute(int lowerBound, DateTimeUnit lowerUnit, int upperBound, DateTimeUnit upperUnit)
			: this(lowerBound, lowerUnit, RangeBoundaryType.Inclusive, upperBound, upperUnit, RangeBoundaryType.Inclusive)
		{ }

		/// <summary>
		/// <para>Initializes a new instance of the <see cref="RelativeDateTimeValidatorAttribute"/>.</para>
		/// </summary>
		/// <param name="lowerBound">The lower bound.</param>
		/// <param name="lowerUnit">The lower bound unit of time.</param>
		/// <param name="lowerBoundType">The indication of how to perform the lower bound check.</param>
		/// <param name="upperBound">The upper bound</param>
		/// <param name="upperUnit">The upper bound unit of time.</param>
		/// <param name="upperBoundType">The indication of how to perform the upper bound check.</param>
		public RelativeDateTimeValidatorAttribute(int lowerBound, DateTimeUnit lowerUnit, RangeBoundaryType lowerBoundType,
			int upperBound, DateTimeUnit upperUnit, RangeBoundaryType upperBoundType)
		{
			ValidatorArgumentsValidatorHelper.ValidateRelativeDatimeValidator(lowerBound, lowerUnit, lowerBoundType, upperBound, upperUnit, upperBoundType);

			this.lowerBound = lowerBound;
			this.lowerUnit = lowerUnit;
			this.lowerBoundType = lowerBoundType;
			this.upperBound = upperBound;
			this.upperUnit = upperUnit;
			this.upperBoundType = upperBoundType;
		}

        /// <summary>
        /// The lower bound
        /// </summary>
	    public int LowerBound
	    {
	        get { return lowerBound; }
	    }

        /// <summary>
        /// The lower bound unit of time.
        /// </summary>
	    public DateTimeUnit LowerUnit
	    {
	        get { return lowerUnit; }
	    }

        /// <summary>
        /// The indication of how to perform the lower bound check.
        /// </summary>
	    public RangeBoundaryType LowerBoundType
	    {
	        get { return lowerBoundType; }
	    }

        /// <summary>
        /// The upper bound
        /// </summary>
	    public int UpperBound
	    {
	        get { return upperBound; }
	    }

        /// <summary>
        /// The upper bound unit of time.
        /// </summary>
	    public DateTimeUnit UpperUnit
	    {
	        get { return upperUnit; }
	    }

        /// <summary>
        /// The indication of how to perform the upper bound check.
        /// </summary>
	    public RangeBoundaryType UpperBoundType
	    {
	        get { return upperBoundType; }
	    }

		/// <summary>
		/// Creates the <see cref="RelativeDateTimeValidator"/> described by the attribute object.
		/// </summary>
		/// <param name="targetType">The type of object that will be validated by the validator.</param>
		/// <remarks>This operation must be overriden by subclasses.</remarks>
		/// <returns>The created <see cref="RelativeDateTimeValidator"/>.</returns>
		protected override Validator DoCreateValidator(Type targetType)
		{
			return new RelativeDateTimeValidator(LowerBound, LowerUnit, LowerBoundType, UpperBound, UpperUnit, UpperBoundType, Negated);
		}

        private readonly Guid typeId = Guid.NewGuid();

        /// <summary>
        /// Gets a unique identifier for this attribute.
        /// </summary>
        public override object TypeId
        {
            get
            {
                return this.typeId;
            }
        }
    }
}

