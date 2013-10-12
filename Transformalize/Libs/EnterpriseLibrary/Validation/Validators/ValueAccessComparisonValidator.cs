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
    /// Performs validation by comparing the provided value with another value extracted from the validation target.
    /// </summary>
    public class ValueAccessComparisonValidator : ValueValidator
    {
        private ValueAccess valueAccess;
        private ComparisonOperator comparisonOperator;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValueAccessComparisonValidator"/> class.
        /// </summary>
        /// <param name="valueAccess">The <see cref="ValueAccess"/> to use when extracting the comparison operand.</param>
        /// <param name="comparisonOperator">The <see cref="ComparisonOperator"/> that specifies the kind of 
        /// comparison to perform.</param>
        public ValueAccessComparisonValidator(ValueAccess valueAccess, ComparisonOperator comparisonOperator)
            : this(valueAccess, comparisonOperator, null, null)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValueAccessComparisonValidator"/> class.
        /// </summary>
        /// <param name="valueAccess">The <see cref="ValueAccess"/> to use when extracting the comparison operand.</param>
        /// <param name="comparisonOperator">The <see cref="ComparisonOperator"/> that specifies the kind of 
        /// comparison to perform.</param>
        /// <param name="messageTemplate">The message template to use when logging validation failures.</param>
        /// <param name="tag">The tag that describes the purpose of the validator.</param>
        public ValueAccessComparisonValidator(ValueAccess valueAccess,
            ComparisonOperator comparisonOperator,
            string messageTemplate,
            string tag)
            : this(valueAccess, comparisonOperator, messageTemplate, tag, false)
        {
            this.valueAccess = valueAccess;
            this.comparisonOperator = comparisonOperator;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValueAccessComparisonValidator"/> class.
        /// </summary>
        /// <param name="valueAccess">The <see cref="ValueAccess"/> to use when extracting the comparison operand.</param>
        /// <param name="comparisonOperator">The <see cref="ComparisonOperator"/> that specifies the kind of 
        /// comparison to perform.</param>
        /// <param name="messageTemplate">The message template to use when logging validation failures.</param>
        /// <param name="negated">Indicates if the validation logic represented by the validator should be negated.</param>
        public ValueAccessComparisonValidator(ValueAccess valueAccess,
            ComparisonOperator comparisonOperator,
            string messageTemplate,
            bool negated)
            : this(valueAccess, comparisonOperator, messageTemplate, null, negated)
        {
            this.valueAccess = valueAccess;
            this.comparisonOperator = comparisonOperator;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValueAccessComparisonValidator"/> class.
        /// </summary>
        /// <param name="valueAccess">The <see cref="ValueAccess"/> to use when extracting the comparison operand.</param>
        /// <param name="comparisonOperator">The <see cref="ComparisonOperator"/> that specifies the kind of 
        /// comparison to perform.</param>
        /// <param name="messageTemplate">The message template to use when logging validation failures.</param>
        /// <param name="tag">The tag that describes the purpose of the validator.</param>
        /// <param name="negated">Indicates if the validation logic represented by the validator should be negated.</param>
        public ValueAccessComparisonValidator(ValueAccess valueAccess,
            ComparisonOperator comparisonOperator,
            string messageTemplate,
            string tag,
            bool negated)
            : base(messageTemplate, tag, negated)
        {
            if (valueAccess == null)
            {
                throw new ArgumentNullException("valueAccess");
            }

            this.valueAccess = valueAccess;
            this.comparisonOperator = comparisonOperator;
        }

        /// <summary>
        /// Validates by comparing <paramref name="objectToValidate"/> with the result of extracting a value from
        /// <paramref name="currentTarget"/> usign the configured <see cref="ValueAccess"/> using
        /// the configured <see cref="ComparisonOperator"/>.
        /// </summary>
        /// <param name="objectToValidate">The object to validate.</param>
        /// <param name="currentTarget">The object on the behalf of which the validation is performed.</param>
        /// <param name="key">The key that identifies the source of <paramref name="objectToValidate"/>.</param>
        /// <param name="validationResults">The validation results to which the outcome of the validation should be stored.</param>
        public override void DoValidate(object objectToValidate,
            object currentTarget,
            string key,
            ValidationResults validationResults)
        {
            object comparand;
            string valueAccessFailureMessage;
            bool status = this.valueAccess.GetValue(currentTarget, out comparand, out valueAccessFailureMessage);

            if (!status)
            {
                LogValidationResult(validationResults,
                    string.Format(
                        CultureInfo.CurrentCulture,
                        Resources.ValueAccessComparisonValidatorFailureToRetrieveComparand,
                        this.valueAccess.Key,
                        valueAccessFailureMessage),
                    currentTarget,
                    key);
                return;
            }

            bool valid = false;

            if (this.comparisonOperator == ComparisonOperator.Equal || this.comparisonOperator == ComparisonOperator.NotEqual)
            {
                valid = (objectToValidate != null ? objectToValidate.Equals(comparand) : comparand == null)
                    ^ (this.comparisonOperator == ComparisonOperator.NotEqual)
                    ^ this.Negated;
            }
            else
            {
                IComparable comparableObjectToValidate = objectToValidate as IComparable;
                if (comparableObjectToValidate != null
                    && comparand != null
                    && comparableObjectToValidate.GetType() == comparand.GetType())
                {
                    int comparison = comparableObjectToValidate.CompareTo(comparand);

                    switch (this.comparisonOperator)
                    {
                        case ComparisonOperator.GreaterThan:
                            valid = comparison > 0;
                            break;
                        case ComparisonOperator.GreaterThanEqual:
                            valid = comparison >= 0;
                            break;
                        case ComparisonOperator.LessThan:
                            valid = comparison < 0;
                            break;
                        case ComparisonOperator.LessThanEqual:
                            valid = comparison <= 0;
                            break;
                        default:
                            break;
                    }

                    valid = valid ^ this.Negated;
                }
                else
                {
                    valid = false;
                }
            }

            if (!valid)
            {
                LogValidationResult(validationResults,
                    string.Format(
                        CultureInfo.CurrentCulture,
                        MessageTemplate,
                        objectToValidate,
                        key,
                        this.Tag,
                        comparand,
                        this.valueAccess.Key,
                        comparisonOperator),
                    currentTarget,
                    key);
            }
        }

        /// <summary>
        /// Gets the Default Message Template when the validator is non negated.
        /// </summary>
        protected override string DefaultNonNegatedMessageTemplate
        {
            get { return Resources.ValueAccessComparisonValidatorNonNegatedDefaultMessageTemplate; }
        }

        /// <summary>
        /// Gets the Default Message Template when the validator is negated.
        /// </summary>
        protected override string DefaultNegatedMessageTemplate
        {
            get { return Resources.ValueAccessComparisonValidatorNegatedDefaultMessageTemplate; }
        }

        /// <summary>
        /// Object used to access target value.
        /// </summary>
        public ValueAccess ValueAccess
        {
            get { return valueAccess; }
        }

        /// <summary>
        /// How are the values compared?
        /// </summary>
        public ComparisonOperator ComparisonOperator
        {
            get { return comparisonOperator; }
        }
    }
}
