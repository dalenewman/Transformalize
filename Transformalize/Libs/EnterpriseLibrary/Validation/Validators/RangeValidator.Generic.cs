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
    /// Performs validation on <typeparamref name="T"/>
    /// instances by comparing them to the specified boundaries. 
    /// </summary>
    /// <remarks>
    /// <see langword="null"/> is logged as a failure.
    /// </remarks>
    /// <typeparam name="T">The type of the objects to compare. Must implement <see cref="IComparable{T}"/>.</typeparam>
    public class RangeValidator<T> : ValueValidator<T>
        where T : IComparable
    {
        private RangeChecker<T> rangeChecker;

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="RangeValidator{T}"/> class with an upper bound constraint.</para>
        /// </summary>
        /// <param name="upperBound">The upper bound.</param>
        /// <remarks>
        /// No lower bound constraints will be checked by this instance, and the upper bound check will be <see cref="RangeBoundaryType.Inclusive"/>.
        /// </remarks>
        public RangeValidator(T upperBound)
            : this(default(T), RangeBoundaryType.Ignore, upperBound, RangeBoundaryType.Inclusive)
        { }

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="RangeValidator{T}"/> class with an upper bound constraint.</para>
        /// </summary>
        /// <param name="upperBound">The upper bound.</param>
        /// <param name="negated">True if the validator must negate the result of the validation.</param>
        /// <remarks>
        /// No lower bound constraints will be checked by this instance, and the upper bound check will be <see cref="RangeBoundaryType.Inclusive"/>.
        /// </remarks>
        protected RangeValidator(T upperBound, bool negated)
            : this(default(T), RangeBoundaryType.Ignore, upperBound, RangeBoundaryType.Inclusive, negated)
        { }

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="RangeValidator{T}"/> class with lower and 
        /// upper bound constraints.</para>
        /// </summary>
        /// <param name="lowerBound">The lower bound.</param>
        /// <param name="upperBound">The upper bound.</param>
        /// <remarks>
        /// Both bound checks will be <see cref="RangeBoundaryType.Inclusive"/>.
        /// </remarks>
        public RangeValidator(T lowerBound, T upperBound)
            : this(lowerBound, RangeBoundaryType.Inclusive, upperBound, RangeBoundaryType.Inclusive)
        { }

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="RangeValidator{T}"/> class with lower and 
        /// upper bound constraints.</para>
        /// </summary>
        /// <param name="lowerBound">The lower bound.</param>
        /// <param name="upperBound">The upper bound.</param>
        /// <param name="negated">True if the validator must negate the result of the validation.</param>
        /// <remarks>
        /// Both bound checks will be <see cref="RangeBoundaryType.Inclusive"/>.
        /// </remarks>
        protected RangeValidator(T lowerBound, T upperBound, bool negated)
            : this(lowerBound, RangeBoundaryType.Inclusive, upperBound, RangeBoundaryType.Inclusive, negated)
        { }

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="RangeValidator{T}"/> class with fully specified
        /// bound constraints.</para>
        /// </summary>
        /// <param name="lowerBound">The lower bound.</param>
        /// <param name="lowerBoundType">The indication of how to perform the lower bound check.</param>
        /// <param name="upperBound">The upper bound.</param>
        /// <param name="upperBoundType">The indication of how to perform the upper bound check.</param>
        /// <seealso cref="RangeBoundaryType"/>
        public RangeValidator(T lowerBound, RangeBoundaryType lowerBoundType,
            T upperBound, RangeBoundaryType upperBoundType)
            : this(lowerBound, lowerBoundType, upperBound, upperBoundType, null)
        { }

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="RangeValidator{T}"/> class with fully specified
        /// bound constraints.</para>
        /// </summary>
        /// <param name="lowerBound">The lower bound.</param>
        /// <param name="lowerBoundType">The indication of how to perform the lower bound check.</param>
        /// <param name="upperBound">The upper bound.</param>
        /// <param name="upperBoundType">The indication of how to perform the upper bound check.</param>
        /// <param name="negated">True if the validator must negate the result of the validation.</param>
        /// <seealso cref="RangeBoundaryType"/>
        protected RangeValidator(T lowerBound, RangeBoundaryType lowerBoundType,
            T upperBound, RangeBoundaryType upperBoundType, bool negated)
            : this(lowerBound, lowerBoundType, upperBound, upperBoundType, null, negated)
        { }

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="RangeValidator{T}"/> class with fully specified
        /// bound constraints and a message template.</para>
        /// </summary>
        /// <param name="lowerBound">The lower bound.</param>
        /// <param name="lowerBoundType">The indication of how to perform the lower bound check.</param>
        /// <param name="upperBound">The upper bound.</param>
        /// <param name="upperBoundType">The indication of how to perform the upper bound check.</param>
        /// <param name="messageTemplate">The message template to use when logging results.</param>
        /// <seealso cref="RangeBoundaryType"/>
        public RangeValidator(T lowerBound, RangeBoundaryType lowerBoundType,
            T upperBound, RangeBoundaryType upperBoundType,
            string messageTemplate)
            : this(lowerBound, lowerBoundType, upperBound, upperBoundType, messageTemplate, false)
        {
            this.rangeChecker = new RangeChecker<T>(lowerBound, lowerBoundType, upperBound, upperBoundType);
        }

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="RangeValidator{T}"/> class with fully specified
        /// bound constraints and a message template.</para>
        /// </summary>
        /// <param name="lowerBound">The lower bound.</param>
        /// <param name="lowerBoundType">The indication of how to perform the lower bound check.</param>
        /// <param name="upperBound">The upper bound.</param>
        /// <param name="upperBoundType">The indication of how to perform the upper bound check.</param>
        /// <param name="messageTemplate">The message template to use when logging results.</param>
        /// <param name="negated">True if the validator must negate the result of the validation.</param>
        /// <seealso cref="RangeBoundaryType"/>
        public RangeValidator(T lowerBound, RangeBoundaryType lowerBoundType,
            T upperBound, RangeBoundaryType upperBoundType,
            string messageTemplate, bool negated)
            : base(messageTemplate, null, negated)
        {
            ValidatorArgumentsValidatorHelper.ValidateRangeValidator(lowerBound, lowerBoundType, upperBound, upperBoundType);

            this.rangeChecker = new RangeChecker<T>(lowerBound, lowerBoundType, upperBound, upperBoundType);
        }

        /// <summary>
        /// Validates by comparing <paramref name="objectToValidate"/> with the constraints
        /// specified for the validator.
        /// </summary>
        /// <param name="objectToValidate">The object to validate.</param>
        /// <param name="currentTarget">The object on the behalf of which the validation is performed.</param>
        /// <param name="key">The key that identifies the source of <paramref name="objectToValidate"/>.</param>
        /// <param name="validationResults">The validation results to which the outcome of the validation should be stored.</param>
        /// <remarks>
        /// <see langword="null"/> is considered a failed validation.
        /// </remarks>
        protected override void DoValidate(T objectToValidate,
            object currentTarget,
            string key,
            ValidationResults validationResults)
        {
            bool logError = false;
            bool isObjectToValidateNull = objectToValidate == null;

            logError = !isObjectToValidateNull && !this.rangeChecker.IsInRange(objectToValidate);

            if (isObjectToValidateNull || (logError != Negated))
            {
                LogValidationResult(validationResults,
                    GetMessage(objectToValidate, key),
                    currentTarget,
                    key);
            }
        }

        /// <summary>
        /// Gets the message representing a failed validation.
        /// </summary>
        /// <param name="objectToValidate">The object for which validation was performed.</param>
        /// <param name="key">The key representing the value being validated for <paramref name="objectToValidate"/>.</param>
        /// <returns>The message representing the validation failure.</returns>
        protected internal override string GetMessage(object objectToValidate, string key)
        {
            return string.Format(
                CultureInfo.CurrentCulture,
                this.MessageTemplate,
                objectToValidate,
                key,
                this.Tag,
                this.rangeChecker.LowerBound,
                this.rangeChecker.LowerBoundType,
                this.rangeChecker.UpperBound,
                this.rangeChecker.UpperBoundType);
        }

        /// <summary>
        /// Gets the Default Message Template when the validator is non negated.
        /// </summary>
        protected override string DefaultNonNegatedMessageTemplate
        {
            get { return Resources.RangeValidatorNonNegatedDefaultMessageTemplate; }
        }

        /// <summary>
        /// Gets the Default Message Template when the validator is negated.
        /// </summary>
        protected override string DefaultNegatedMessageTemplate
        {
            get { return Resources.RangeValidatorNegatedDefaultMessageTemplate; }
        }

        /// <summary>
        /// Internal helper object used to do the actual range check.
        /// </summary>
        public RangeChecker<T> RangeChecker
        {
            get { return rangeChecker; }
        }

        /// <summary>
        /// Lower bound of range.
        /// </summary>
        public T LowerBound
        {
            get { return this.rangeChecker.LowerBound; }
        }

        /// <summary>
        /// Upper bound of range.
        /// </summary>
        public T UpperBound
        {
            get { return this.rangeChecker.UpperBound; }
        }

        /// <summary>
        /// Is lower bound included, excluded, or ignored.
        /// </summary>
        public RangeBoundaryType LowerBoundType
        {
            get { return this.rangeChecker.LowerBoundType; }
        }

        /// <summary>
        /// Is upper bound included, excluded, or ignored.
        /// </summary>
        public RangeBoundaryType UpperBoundType
        {
            get { return this.rangeChecker.UpperBoundType; }
        }
    }
}
