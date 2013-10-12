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

namespace Transformalize.Libs.EnterpriseLibrary.Validation
{
    /// <summary>
    /// Represents logic used to validate an instance of <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of objects the can be validated.</typeparam>
    /// <remarks>
    /// Generic validators will still answer the non-generic validation requests, logging a failure when the 
    /// validation target is not compatible with the <typeparamref name="T"/>.
    /// </remarks>
    /// <seealso cref="Validator"/>
    public abstract class Validator<T> : Validator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Validator{T}"/> class.
        /// </summary>
        /// <param name="messageTemplate">The template to use when logging validation results, or <see langword="null"/> we the
        /// default message template is to be used.</param>
        /// <param name="tag">The tag to set when logging validation results, or <see langword="null"/>.</param>
        protected Validator(string messageTemplate, string tag)
            : base(messageTemplate, tag)
        { }

        /// <summary>
        /// Applies the validation logic represented by the receiver on an instance of <typeparamref name="T"/>, 
        /// returning the validation results.
        /// </summary>
        /// <param name="target">The instance of <typeparamref name="T"/> to validate.</param>
        /// <returns>The <see cref="ValidationResults"/> representing the outcome of the validation.</returns>
        public ValidationResults Validate(T target)
        {
            ValidationResults validationResults = new ValidationResults();

            Validate(target, validationResults);

            return validationResults;
        }

        /// <summary>
        /// Applies the validation logic represented by the receiver on an instance of <typeparamref name="T"/>, 
        /// adding the validation results to <paramref name="validationResults"/>.
        /// </summary>
        /// <param name="target">The instance of <typeparamref name="T"/> to validate.</param>
        /// <param name="validationResults">The validation results to which the outcome of the validation should be stored.</param>
        public void Validate(T target, ValidationResults validationResults)
        {
            if (null == validationResults)
                throw new ArgumentNullException("validationResults");

            DoValidate(target, target, null, validationResults);
        }

        /// <summary>
        /// Implements the validation logic for the receiver.
        /// </summary>
        /// <param name="objectToValidate">The object to validate.</param>
        /// <param name="currentTarget">The object on the behalf of which the validation is performed.</param>
        /// <param name="key">The key that identifies the source of <paramref name="objectToValidate"/>.</param>
        /// <param name="validationResults">The validation results to which the outcome of the validation should be stored.</param>
        /// <remarks>
        /// The implementation for this method will perform type checking and converstion before forwarding the 
        /// validation request to method <see cref="Validator{T}.DoValidate(T, object, string, ValidationResults)"/>.
        /// </remarks>
        /// <see cref="Validator.DoValidate"/>
        public override void DoValidate(object objectToValidate,
            object currentTarget,
            string key,
            ValidationResults validationResults)
        {
            // null values need to be avoided when checking for type compliance for value types
            if (objectToValidate == null)
            {
                if (typeof(T).IsValueType)
                {
                    string message
                        = string.Format(
                            CultureInfo.CurrentCulture,
                            Resources.ExceptionValidatingNullOnValueType,
                            typeof(T).FullName);
                    LogValidationResult(validationResults, message, currentTarget, key);
                    return;
                }
            }
            else
            {
                if (!(objectToValidate == null || objectToValidate is T))
                {
                    string message
                        = string.Format(
                            CultureInfo.CurrentCulture,
                            Resources.ExceptionInvalidTargetType,
                            typeof(T).FullName,
                            objectToValidate.GetType().FullName);
                    LogValidationResult(validationResults, message, currentTarget, key);
                    return;
                }
            }

            this.DoValidate((T)objectToValidate, currentTarget, key, validationResults);
        }

        /// <summary>
        /// Implements the validation logic for the receiver.
        /// </summary>
        /// <param name="objectToValidate">The instance of <typeparamref name="T"/> to validate.</param>
        /// <param name="currentTarget">The object on the behalf of which the validation is performed.</param>
        /// <param name="key">The key that identifies the source of <paramref name="objectToValidate"/>.</param>
        /// <param name="validationResults">The validation results to which the outcome of the validation should be stored.</param>
        /// <remarks>
        /// Subclasses must provide a concrete implementation the validation logic.
        /// </remarks>
        /// <see cref="Validator.DoValidate"/>
        protected abstract void DoValidate(T objectToValidate, object currentTarget, string key, ValidationResults validationResults);
    }
}
