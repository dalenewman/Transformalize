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

using System.Globalization;
using Transformalize.Libs.EnterpriseLibrary.Common.Configuration;
using Transformalize.Libs.EnterpriseLibrary.Validation.Configuration;

namespace Transformalize.Libs.EnterpriseLibrary.Validation.Validators
{
    /// <summary>
    /// Performs validation on strings by comparing their lengths to the specified boundaries. 
    /// </summary>
    /// <remarks>
    /// <see langword="null"/> is logged as a failure.
    /// </remarks>
    [ConfigurationElementType(typeof(StringLengthValidatorData))]
    public class StringLengthValidator : ValueValidator<string>
    {
        private RangeChecker<int> rangeChecker;

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="StringLengthValidator"/> class with an upper bound constraint.</para>
        /// </summary>
        /// <param name="upperBound">The upper bound.</param>
        /// <remarks>
        /// No lower bound constraints will be checked by this instance, and the upper bound check will be <see cref="RangeBoundaryType.Inclusive"/>.
        /// </remarks>
        public StringLengthValidator(int upperBound)
            : this(0, RangeBoundaryType.Ignore, upperBound, RangeBoundaryType.Inclusive)
        { }

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="StringLengthValidator"/> class with an upper bound constraint.</para>
        /// </summary>
        /// <param name="upperBound">The upper bound.</param>
        /// <param name="negated">True if the validator must negate the result of the validation.</param>
        /// <remarks>
        /// No lower bound constraints will be checked by this instance, and the upper bound check will be <see cref="RangeBoundaryType.Inclusive"/>.
        /// </remarks>
        public StringLengthValidator(int upperBound, bool negated)
            : this(0, RangeBoundaryType.Ignore, upperBound, RangeBoundaryType.Inclusive, negated)
        { }

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="StringLengthValidator"/> class with lower and 
        /// upper bound constraints.</para>
        /// </summary>
        /// <param name="lowerBound">The lower bound.</param>
        /// <param name="upperBound">The upper bound.</param>
        /// <remarks>
        /// Both bound checks will be <see cref="RangeBoundaryType.Inclusive"/>.
        /// </remarks>
        public StringLengthValidator(int lowerBound, int upperBound)
            : this(lowerBound, RangeBoundaryType.Inclusive, upperBound, RangeBoundaryType.Inclusive)
        { }

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="StringLengthValidator"/> class with lower and 
        /// upper bound constraints.</para>
        /// </summary>
        /// <param name="lowerBound">The lower bound.</param>
        /// <param name="upperBound">The upper bound.</param>
        /// <param name="negated">True if the validator must negate the result of the validation.</param>
        /// <remarks>
        /// Both bound checks will be <see cref="RangeBoundaryType.Inclusive"/>.
        /// </remarks>
        public StringLengthValidator(int lowerBound, int upperBound, bool negated)
            : this(lowerBound, RangeBoundaryType.Inclusive, upperBound, RangeBoundaryType.Inclusive, negated)
        { }

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="StringLengthValidator"/> class with fully specified
        /// bound constraints.</para>
        /// </summary>
        /// <param name="lowerBound">The lower bound.</param>
        /// <param name="lowerBoundType">The indication of how to perform the lower bound check.</param>
        /// <param name="upperBound">The upper bound.</param>
        /// <param name="upperBoundType">The indication of how to perform the upper bound check.</param>
        /// <seealso cref="RangeBoundaryType"/>
        public StringLengthValidator(int lowerBound, RangeBoundaryType lowerBoundType,
            int upperBound, RangeBoundaryType upperBoundType)
            : this(lowerBound, lowerBoundType, upperBound, upperBoundType, null)
        { }

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="StringLengthValidator"/> class with fully specified
        /// bound constraints.</para>
        /// </summary>
        /// <param name="lowerBound">The lower bound.</param>
        /// <param name="lowerBoundType">The indication of how to perform the lower bound check.</param>
        /// <param name="upperBound">The upper bound.</param>
        /// <param name="upperBoundType">The indication of how to perform the upper bound check.</param>
        /// <param name="negated">True if the validator must negate the result of the validation.</param>
        /// <seealso cref="RangeBoundaryType"/>
        public StringLengthValidator(int lowerBound, RangeBoundaryType lowerBoundType,
            int upperBound, RangeBoundaryType upperBoundType, bool negated)
            : this(lowerBound, lowerBoundType, upperBound, upperBoundType, null, negated)
        { }

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="StringLengthValidator"/> class with fully specified
        /// bound constraints and a message template.</para>
        /// </summary>
        /// <param name="lowerBound">The lower bound.</param>
        /// <param name="lowerBoundType">The indication of how to perform the lower bound check.</param>
        /// <param name="upperBound">The upper bound.</param>
        /// <param name="upperBoundType">The indication of how to perform the upper bound check.</param>
        /// <param name="messageTemplate">The message template to use when logging results.</param>
        /// <seealso cref="RangeBoundaryType"/>
        public StringLengthValidator(int lowerBound, RangeBoundaryType lowerBoundType,
            int upperBound, RangeBoundaryType upperBoundType,
            string messageTemplate)
            : this(lowerBound, lowerBoundType, upperBound, upperBoundType, messageTemplate, false)
        {
            this.rangeChecker = new RangeChecker<int>(lowerBound, lowerBoundType, upperBound, upperBoundType);
        }

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="StringLengthValidator"/> class with fully specified
        /// bound constraints and a message template.</para>
        /// </summary>
        /// <param name="lowerBound">The lower bound.</param>
        /// <param name="lowerBoundType">The indication of how to perform the lower bound check.</param>
        /// <param name="upperBound">The upper bound.</param>
        /// <param name="upperBoundType">The indication of how to perform the upper bound check.</param>
        /// <param name="messageTemplate">The message template to use when logging results.</param>
        /// <param name="negated">True if the validator must negate the result of the validation.</param>
        /// <seealso cref="RangeBoundaryType"/>
        public StringLengthValidator(int lowerBound, RangeBoundaryType lowerBoundType,
            int upperBound, RangeBoundaryType upperBoundType,
            string messageTemplate,
            bool negated)
            : base(messageTemplate, null, negated)
        {
            this.rangeChecker = new RangeChecker<int>(lowerBound, lowerBoundType, upperBound, upperBoundType);
        }

        /// <summary>
        /// Validates by comparing the length for <paramref name="objectToValidate"/> with the constraints
        /// specified for the validator.
        /// </summary>
        /// <param name="objectToValidate">The object to validate.</param>
        /// <param name="currentTarget">The object on the behalf of which the validation is performed.</param>
        /// <param name="key">The key that identifies the source of <paramref name="objectToValidate"/>.</param>
        /// <param name="validationResults">The validation results to which the outcome of the validation should be stored.</param>
        /// <remarks>
        /// <see langword="null"/> is considered a failed validation.
        /// </remarks>
        protected override void DoValidate(string objectToValidate,
            object currentTarget,
            string key,
            ValidationResults validationResults)
        {
            if (objectToValidate != null)
            {
                if (this.rangeChecker.IsInRange(objectToValidate.Length) == Negated)
                {
                    LogValidationResult(validationResults, GetMessage(objectToValidate, key), currentTarget, key);
                }
            }
            else
            {
                LogValidationResult(validationResults, GetMessage(objectToValidate, key), currentTarget, key);
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
            get { return Resources.StringLengthValidatorNonNegatedDefaultMessageTemplate; }
        }

        /// <summary>
        /// Gets the Default Message Template when the validator is negated.
        /// </summary>
        protected override string DefaultNegatedMessageTemplate
        {
            get { return Resources.StringLengthValidatorNegatedDefaultMessageTemplate; }
        }

        /// <summary>
        /// Lower bound for string length.
        /// </summary>
        public int LowerBound
        {
            get { return this.rangeChecker.LowerBound; }
        }

        /// <summary>
        /// Upper bound for string length.
        /// </summary>
        public int UpperBound
        {
            get { return this.rangeChecker.UpperBound; }
        }

        /// <summary>
        /// Is lower bound included, excluded, or ignored?
        /// </summary>
        public RangeBoundaryType LowerBoundType
        {
            get { return this.rangeChecker.LowerBoundType; }
        }

        /// <summary>
        /// Is upper bound included, excluded, or ignored?
        /// </summary>
        public RangeBoundaryType UpperBoundType
        {
            get { return this.rangeChecker.UpperBoundType; }
        }
    }
}
