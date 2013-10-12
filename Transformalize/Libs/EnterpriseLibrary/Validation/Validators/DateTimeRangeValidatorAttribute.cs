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

namespace Transformalize.Libs.EnterpriseLibrary.Validation.Validators
{
    /// <summary>
    /// Attribute to specify date range validation on a property, method or field.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property
        | AttributeTargets.Field
        | AttributeTargets.Method
        | AttributeTargets.Parameter,
        AllowMultiple = true,
        Inherited = false)]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1019",
        Justification = "Fields are used internally")]
    public sealed class DateTimeRangeValidatorAttribute : ValueValidatorAttribute
    {
        private object lowerBound;
        private DateTime effectiveLowerBound;
        private RangeBoundaryType lowerBoundType;
        private object upperBound;
        private DateTime effectiveUpperBound;
        private RangeBoundaryType upperBoundType;

        /// <summary>
        /// Initializes a new instance with an upper bound.
        /// </summary>
        /// <param name="upperBound">An ISO8601 formatted string representing the upper bound, like "2006-01-20T00:00:00".</param>
        public DateTimeRangeValidatorAttribute(string upperBound)
            : this(null, RangeBoundaryType.Ignore, upperBound, RangeBoundaryType.Inclusive)
        { }

        /// <summary>
        /// Initializes a new instance with an upper bound.
        /// </summary>
        /// <param name="upperBound">The upper bound.</param>
        public DateTimeRangeValidatorAttribute(DateTime upperBound)
            : this(default(DateTime), RangeBoundaryType.Ignore, upperBound, RangeBoundaryType.Inclusive)
        { }

        /// <summary>
        /// Initializes a new instance with lower and upper bounds.
        /// </summary>
        /// <param name="lowerBound">An ISO8601 formatted string representing the lower bound, like "2006-01-20T00:00:00".</param>
        /// <param name="upperBound">An ISO8601 formatted string representing the upper bound, like "2006-01-20T00:00:00".</param>
        public DateTimeRangeValidatorAttribute(string lowerBound, string upperBound)
            : this(lowerBound, RangeBoundaryType.Inclusive, upperBound, RangeBoundaryType.Inclusive)
        { }

        /// <summary>
        /// Initializes a new instance with lower and upper bounds.
        /// </summary>
        /// <param name="lowerBound">The lower bound.</param>
        /// <param name="upperBound">The upper bound.</param>
        public DateTimeRangeValidatorAttribute(DateTime lowerBound, DateTime upperBound)
            : this(lowerBound, RangeBoundaryType.Inclusive, upperBound, RangeBoundaryType.Inclusive)
        { }

        /// <summary>
        /// Initializes a new instance with lower and upper bounds, and bound types.
        /// </summary>
        /// <param name="lowerBound">An ISO8601 formatted string representing the lower bound, like "2006-01-20T00:00:00".</param>
        /// <param name="lowerBoundType">The bound type for the lower bound.</param>
        /// <param name="upperBound">An ISO8601 formatted string representing the upper bound, like "2006-01-20T00:00:00".</param>
        /// <param name="upperBoundType">The bound type for the upper bound.</param>
        public DateTimeRangeValidatorAttribute(
            string lowerBound,
            RangeBoundaryType lowerBoundType,
            string upperBound,
            RangeBoundaryType upperBoundType)
            : this(lowerBound, ConvertToISO8601Date(lowerBound, "lowerBound"), lowerBoundType, upperBound, ConvertToISO8601Date(upperBound, "upperBound"), upperBoundType)
        { }

        /// <summary>
        /// Initializes a new instance with lower and upper bounds, and bound types.
        /// </summary>
        /// <param name="lowerBound">The lower bound.</param>
        /// <param name="lowerBoundType">The bound type for the lower bound.</param>
        /// <param name="upperBound">The upper bound.</param>
        /// <param name="upperBoundType">The bound type for the upper bound.</param>
        public DateTimeRangeValidatorAttribute(
            DateTime lowerBound,
            RangeBoundaryType lowerBoundType,
            DateTime upperBound,
            RangeBoundaryType upperBoundType)
            : this(lowerBound, lowerBound, lowerBoundType, upperBound, upperBound, upperBoundType)
        { }

        private DateTimeRangeValidatorAttribute(
            object lowerBound,
            DateTime effectiveLowerBound,
            RangeBoundaryType lowerBoundType,
            object upperBound,
            DateTime effectiveUpperBound,
            RangeBoundaryType upperBoundType)
        {
            this.lowerBound = lowerBound;
            this.effectiveLowerBound = effectiveLowerBound;
            this.lowerBoundType = lowerBoundType;
            this.upperBound = upperBound;
            this.effectiveUpperBound = effectiveUpperBound;
            this.upperBoundType = upperBoundType;
        }

        /// <summary>
        /// The lower bound.
        /// </summary>
        public object LowerBound
        {
            get { return lowerBound; }
        }

        /// <summary>
        /// The bound type for the lower bound.
        /// </summary>
        public RangeBoundaryType LowerBoundType
        {
            get { return lowerBoundType; }
        }

        /// <summary>
        /// The upper bound.
        /// </summary>
        public object UpperBound
        {
            get { return upperBound; }
        }

        /// <summary>
        /// The bound type for the upper bound.
        /// </summary>
        public RangeBoundaryType UpperBoundType
        {
            get { return upperBoundType; }
        }

        /// <summary>
        /// Creates the <see cref="DateTimeRangeValidator"/> described by the configuration object.
        /// </summary>
        /// <param name="targetType">The type of object that will be validated by the validator.</param>
        /// <returns>The created <see cref="Validator"/>.</returns>
        protected override Validator DoCreateValidator(Type targetType)
        {
            return new DateTimeRangeValidator(
                this.effectiveLowerBound,
                this.lowerBoundType,
                this.effectiveUpperBound,
                this.upperBoundType,
                Negated);
        }

        private static DateTime ConvertToISO8601Date(string iso8601DateString, string paramName)
        {
            if (string.IsNullOrEmpty(iso8601DateString))
            {
                return default(DateTime);
            }
            try
            {
                return DateTime.ParseExact(iso8601DateString, "s", CultureInfo.InvariantCulture);
            }
            catch (FormatException e)
            {
                throw new ArgumentException(Resources.ExceptionInvalidDate, "paramName", e);
            }
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
