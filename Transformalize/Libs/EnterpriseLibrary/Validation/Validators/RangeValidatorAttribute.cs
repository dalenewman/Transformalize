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
using System.ComponentModel;
using System.Globalization;

namespace Transformalize.Libs.EnterpriseLibrary.Validation.Validators
{
    /// <summary>
    /// Represents a <see cref="RangeValidator"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property
        | AttributeTargets.Field
        | AttributeTargets.Method
        | AttributeTargets.Parameter,
        AllowMultiple = true,
        Inherited = false)]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1019",
        Justification = "Fields are used internally")]
    public sealed class RangeValidatorAttribute : ValueValidatorAttribute
    {
        private Type boundType;
        private object lowerBound;
        private IComparable effectiveLowerBound;
        private RangeBoundaryType lowerBoundType;
        private object upperBound;
        private IComparable effectiveUpperBound;
        private RangeBoundaryType upperBoundType;

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="RangeValidatorAttribute"/> class with fully specified
        /// int bound constraints.</para>
        /// </summary>
        /// <param name="lowerBound">The lower bound.</param>
        /// <param name="lowerBoundType">The indication of how to perform the lower bound check.</param>
        /// <param name="upperBound">The upper bound.</param>
        /// <param name="upperBoundType">The indication of how to perform the upper bound check.</param>
        /// <seealso cref="RangeBoundaryType"/>
        public RangeValidatorAttribute(int lowerBound, RangeBoundaryType lowerBoundType,
                int upperBound, RangeBoundaryType upperBoundType)
            : this((IComparable)lowerBound, lowerBoundType, (IComparable)upperBound, upperBoundType)
        { }

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="RangeValidatorAttribute"/> class with fully specified
        /// double bound constraints.</para>
        /// </summary>
        /// <param name="lowerBound">The lower bound.</param>
        /// <param name="lowerBoundType">The indication of how to perform the lower bound check.</param>
        /// <param name="upperBound">The upper bound.</param>
        /// <param name="upperBoundType">The indication of how to perform the upper bound check.</param>
        /// <seealso cref="RangeBoundaryType"/>
        public RangeValidatorAttribute(double lowerBound, RangeBoundaryType lowerBoundType,
                double upperBound, RangeBoundaryType upperBoundType)
            : this((IComparable)lowerBound, lowerBoundType, (IComparable)upperBound, upperBoundType)
        { }

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="RangeValidatorAttribute"/> class with fully specified
        /// DateTime bound constraints.</para>
        /// </summary>
        /// <param name="lowerBound">The lower bound.</param>
        /// <param name="lowerBoundType">The indication of how to perform the lower bound check.</param>
        /// <param name="upperBound">The upper bound.</param>
        /// <param name="upperBoundType">The indication of how to perform the upper bound check.</param>
        /// <seealso cref="RangeBoundaryType"/>
        public RangeValidatorAttribute(DateTime lowerBound, RangeBoundaryType lowerBoundType,
                DateTime upperBound, RangeBoundaryType upperBoundType)
            : this((IComparable)lowerBound, lowerBoundType, (IComparable)upperBound, upperBoundType)
        { }

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="RangeValidatorAttribute"/> class with fully specified
        /// long bound constraints.</para>
        /// </summary>
        /// <param name="lowerBound">The lower bound.</param>
        /// <param name="lowerBoundType">The indication of how to perform the lower bound check.</param>
        /// <param name="upperBound">The upper bound.</param>
        /// <param name="upperBoundType">The indication of how to perform the upper bound check.</param>
        /// <seealso cref="RangeBoundaryType"/>
        public RangeValidatorAttribute(long lowerBound, RangeBoundaryType lowerBoundType,
                long upperBound, RangeBoundaryType upperBoundType)
            : this((IComparable)lowerBound, lowerBoundType, (IComparable)upperBound, upperBoundType)
        { }

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="RangeValidatorAttribute"/> class with fully specified
        /// string bound constraints.</para>
        /// </summary>
        /// <param name="lowerBound">The lower bound.</param>
        /// <param name="lowerBoundType">The indication of how to perform the lower bound check.</param>
        /// <param name="upperBound">The upper bound.</param>
        /// <param name="upperBoundType">The indication of how to perform the upper bound check.</param>
        /// <seealso cref="RangeBoundaryType"/>
        public RangeValidatorAttribute(string lowerBound, RangeBoundaryType lowerBoundType,
                string upperBound, RangeBoundaryType upperBoundType)
            : this((IComparable)lowerBound, lowerBoundType, (IComparable)upperBound, upperBoundType)
        { }

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="RangeValidatorAttribute"/> class with fully specified
        /// float bound constraints.</para>
        /// </summary>
        /// <param name="lowerBound">The lower bound.</param>
        /// <param name="lowerBoundType">The indication of how to perform the lower bound check.</param>
        /// <param name="upperBound">The upper bound.</param>
        /// <param name="upperBoundType">The indication of how to perform the upper bound check.</param>
        /// <seealso cref="RangeBoundaryType"/>
        public RangeValidatorAttribute(float lowerBound, RangeBoundaryType lowerBoundType,
                float upperBound, RangeBoundaryType upperBoundType)
            : this((IComparable)lowerBound, lowerBoundType, (IComparable)upperBound, upperBoundType)
        { }

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="RangeValidatorAttribute"/> class with fully specified
        /// bound constraints using string representations of the boundaries.</para>
        /// </summary>
        /// <param name="boundType">The type of the boundaries.</param>
        /// <param name="lowerBound">The lower bound represented with a string, or <see langword="null"/>.</param>
        /// <param name="lowerBoundType">The indication of how to perform the lower bound check.</param>
        /// <param name="upperBound">The upper bound, or <see langword="null"/>.</param>
        /// <param name="upperBoundType">The indication of how to perform the upper bound check.</param>
        /// <seealso cref="RangeBoundaryType"/>
        public RangeValidatorAttribute(Type boundType, string lowerBound, RangeBoundaryType lowerBoundType,
            string upperBound, RangeBoundaryType upperBoundType)
            : this(boundType, lowerBound, ConvertBound(boundType, lowerBound, "lowerBound"), lowerBoundType,
                    upperBound, ConvertBound(boundType, upperBound, "upperBound"), upperBoundType)
        { }

        private RangeValidatorAttribute(IComparable lowerBound, RangeBoundaryType lowerBoundType,
            IComparable upperBound, RangeBoundaryType upperBoundType)
            : this(null, lowerBound, lowerBound, lowerBoundType, upperBound, upperBound, upperBoundType)
        { }

        private RangeValidatorAttribute(
            Type boundType,
            object lowerBound, IComparable effectiveLowerBound, RangeBoundaryType lowerBoundType,
            object upperBound, IComparable effectiveUpperBound, RangeBoundaryType upperBoundType)
        {
            ValidatorArgumentsValidatorHelper.ValidateRangeValidator(effectiveLowerBound, lowerBoundType, effectiveUpperBound, upperBoundType);

            this.boundType = boundType;
            this.lowerBound = lowerBound;
            this.effectiveLowerBound = effectiveLowerBound;
            this.lowerBoundType = lowerBoundType;
            this.upperBound = upperBound;
            this.effectiveUpperBound = effectiveUpperBound;
            this.upperBoundType = upperBoundType;
        }

        private static IComparable ConvertBound(Type boundType, string bound, string boundParameter)
        {
            if (boundType == null)
            {
                throw new ArgumentNullException("boundType");
            }
            if (!typeof(IComparable).IsAssignableFrom(boundType))
            {
                throw new ArgumentException(Resources.ExceptionBoundTypeNotIComparable, "boundType");
            }

            if (bound == null)
            {
                return null;
            }

            if (boundType == typeof(DateTime))
            {
                try
                {
                    return DateTime.ParseExact(bound, "s", CultureInfo.InvariantCulture);
                }
                catch (FormatException e)
                {
                    throw new ArgumentException(Resources.ExceptionInvalidDate, boundParameter, e);
                }
            }
            else
            {
                try
                {
                    return (IComparable)TypeDescriptor.GetConverter(boundType).ConvertFrom(null, CultureInfo.InvariantCulture, bound);
                }
                catch (Exception e)
                {
                    throw new ArgumentException(Resources.ExceptionCannotConvertBound, e);
                }
            }
        }

        /// <summary>
        /// Creates the <see cref="RangeValidator"/> described by the attribute object.
        /// </summary>
        /// <param name="targetType">The type of object that will be validated by the validator.</param>
        /// <remarks>This operation must be overriden by subclasses.</remarks>
        /// <returns>The created <see cref="RangeValidator"/>.</returns>
        protected override Validator DoCreateValidator(Type targetType)
        {
            return new RangeValidator(this.effectiveLowerBound, this.lowerBoundType, this.effectiveUpperBound, this.upperBoundType, this.Negated);
        }

        /// <summary>
        /// The lower bound
        /// </summary>
        public object LowerBound { get { return this.lowerBound; } }

        /// <summary>
        /// The lower bound
        /// </summary>
        public IComparable EffectiveLowerBound { get { return this.effectiveLowerBound; } }

        /// <summary>
        /// The indication of how to perform the lower bound check.
        /// </summary>
        public RangeBoundaryType LowerBoundType { get { return this.lowerBoundType; } }

        /// <summary>
        /// The upper bound, or <see langword="null"/>.
        /// </summary>
        public object UpperBound { get { return this.upperBound; } }

        /// <summary>
        /// The upper bound
        /// </summary>
        public IComparable EffectiveUpperBound { get { return this.effectiveUpperBound; } }

        /// <summary>
        /// The indication of how to perform the upper bound check.
        /// </summary>
        public RangeBoundaryType UpperBoundType { get { return this.upperBoundType; } }

        /// <summary>
        /// The type of the validated range.
        /// </summary>
        public Type BoundType
        {
            get { return boundType; }
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
