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
	/// Describes a <see cref="StringLengthValidator"/>.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property
		| AttributeTargets.Field
		| AttributeTargets.Method
		| AttributeTargets.Parameter,
		AllowMultiple = true,
		Inherited = false)]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1019",
		Justification = "Fields are used internally")]
	public sealed class StringLengthValidatorAttribute : ValueValidatorAttribute
	{
		private int lowerBound;
		private RangeBoundaryType lowerBoundType;
		private int upperBound;
		private RangeBoundaryType upperBoundType;

		/// <summary>
		/// <para>Initializes a new instance of the <see cref="StringLengthValidatorAttribute"/> class with an upper bound constraint.</para>
		/// </summary>
		/// <param name="upperBound">The upper bound.</param>
		public StringLengthValidatorAttribute(int upperBound)
			: this(0, RangeBoundaryType.Ignore, upperBound, RangeBoundaryType.Inclusive)
		{ }

		/// <summary>
		/// <para>Initializes a new instance of the <see cref="StringLengthValidatorAttribute"/> class with lower and 
		/// upper bound constraints.</para>
		/// </summary>
		/// <param name="lowerBound">The lower bound.</param>
		/// <param name="upperBound">The upper bound.</param>
		public StringLengthValidatorAttribute(int lowerBound, int upperBound)
			: this(lowerBound, RangeBoundaryType.Inclusive, upperBound, RangeBoundaryType.Inclusive)
		{ }

		/// <summary>
		/// <para>Initializes a new instance of the <see cref="StringLengthValidatorAttribute"/> class with fully specified
		/// bound constraints.</para>
		/// </summary>
		/// <param name="lowerBound">The lower bound.</param>
		/// <param name="lowerBoundType">The indication of how to perform the lower bound check.</param>
		/// <param name="upperBound">The upper bound.</param>
		/// <param name="upperBoundType">The indication of how to perform the upper bound check.</param>
		/// <seealso cref="RangeBoundaryType"/>
		public StringLengthValidatorAttribute(int lowerBound,
			RangeBoundaryType lowerBoundType,
			int upperBound,
			RangeBoundaryType upperBoundType)
		{
			this.lowerBound = lowerBound;
			this.lowerBoundType = lowerBoundType;
			this.upperBound = upperBound;
			this.upperBoundType = upperBoundType;
		}

        /// <summary>
        /// The lower bound.
        /// </summary>
	    public int LowerBound
	    {
	        get { return lowerBound; }
	    }

        /// <summary>
        /// The indication of how to perform the lower bound check.
        /// </summary>
	    public RangeBoundaryType LowerBoundType
	    {
	        get { return lowerBoundType; }
	    }

        /// <summary>
        /// The upper bound.
        /// </summary>
	    public int UpperBound
	    {
	        get { return upperBound; }
	    }

        /// <summary>
        /// The indication of how to perform the upper bound check.
        /// </summary>
	    public RangeBoundaryType UpperBoundType
	    {
	        get { return upperBoundType; }
	    }

		/// <summary>
		/// Creates the <see cref="StringLengthValidator"/> described by the configuration object.
		/// </summary>
		/// <param name="targetType">The type of object that will be validated by the validator.</param>
		/// <returns>The created <see cref="Validator"/>.</returns>
		protected override Validator DoCreateValidator(Type targetType)
		{
			return new StringLengthValidator(this.LowerBound,
				this.LowerBoundType,
				this.UpperBound,
				this.UpperBoundType,
				Negated);
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
