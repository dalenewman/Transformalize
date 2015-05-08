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
using Transformalize.Libs.EnterpriseLibrary.Common.Configuration;
using Transformalize.Libs.EnterpriseLibrary.Validation.Configuration;

namespace Transformalize.Libs.EnterpriseLibrary.Validation.Validators {
    /// <summary>
    /// Validates a string by checking it represents a value for a given type.
    /// </summary>
    [ConfigurationElementType(typeof(TypeConversionValidatorData))]
    public class TypeConversionValidator : ValueValidator<string> {
        private Type targetType;

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="TypeConversionValidator"/>.</para>
        /// </summary>
        /// <param name="targetType">The supplied type used to determine if the string can be converted to it.</param>
        public TypeConversionValidator(Type targetType)
            : this(targetType, false) { }

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="TypeConversionValidator"/>.</para>
        /// </summary>
        /// <param name="negated">True if the validator must negate the result of the validation.</param>
        /// <param name="targetType">The supplied type used to determine if the string can be converted to it.</param>
        public TypeConversionValidator(Type targetType, bool negated)
            : this(targetType, null, negated) { }

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="TypeConversionValidator"/>.</para>
        /// </summary>
        /// <param name="messageTemplate">The message template to use when logging results.</param>
        /// <param name="targetType">The supplied type used to determine if the string can be converted to it.</param>
        public TypeConversionValidator(Type targetType, string messageTemplate)
            : this(targetType, messageTemplate, false) { }

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="TypeConversionValidator"/>.</para>
        /// </summary>
        /// <param name="negated">True if the validator must negate the result of the validation.</param>
        /// <param name="messageTemplate">The message template to use when logging results.</param>
        /// <param name="targetType">The supplied type used to determine if the string can be converted to it.</param>
        public TypeConversionValidator(Type targetType, string messageTemplate, bool negated)
            : base(messageTemplate, null, negated) {
            ValidatorArgumentsValidatorHelper.ValidateTypeConversionValidator(targetType);

            this.targetType = targetType;
        }

        /// <summary>
        /// Implements the validation logic for the receiver.
        /// </summary>
        /// <param name="objectToValidate">The object to validate.</param>
        /// <param name="currentTarget">The object on the behalf of which the validation is performed.</param>
        /// <param name="key">The key that identifies the source of <paramref name="objectToValidate"/>.</param>
        /// <param name="validationResults">The validation results to which the outcome of the validation should be stored.</param>
        protected override void DoValidate(string objectToValidate, object currentTarget, string key, ValidationResults validationResults) {
            bool logError = false;
            bool isObjectToValidateNull = objectToValidate == null;

            if (!isObjectToValidateNull) {
                if (string.Empty.Equals(objectToValidate) && IsTheTargetTypeAValueTypeDifferentFromString()) {
                    logError = true;
                } else {
                    try {
                        // because typeconverter is failing on decimals with thousands delimiter (e.g. 100,000.00)
                        if (targetType == typeof(decimal)) {
                            decimal attempt;
                            if (!Decimal.TryParse(objectToValidate,
                                NumberStyles.Float | NumberStyles.AllowThousands | NumberStyles.AllowCurrencySymbol,
                                (IFormatProvider)CultureInfo.CurrentCulture.GetFormat(typeof(NumberFormatInfo)),
                                out attempt)) {
                                logError = true;
                            }
                        } else {
                            // because typeconverter just too loose with dates
                            if (targetType == typeof(DateTime) && objectToValidate.Length < 6) {
                                logError = true;
                            } else {
                                var typeConverter = TypeDescriptor.GetConverter(targetType);
                                var convertedValue = typeConverter.ConvertFromString(null, CultureInfo.CurrentCulture,
                                    objectToValidate);
                                if (convertedValue == null) {
                                    logError = true;
                                }
                            }
                        }
                    } catch (Exception) {
                        logError = true;
                    }
                }
            }

            if (isObjectToValidateNull || (logError != Negated)) {
                this.LogValidationResult(validationResults,
                    GetMessage(objectToValidate, key),
                    currentTarget,
                    key);
                return;
            }
        }

        private bool IsTheTargetTypeAValueTypeDifferentFromString() {
            TypeCode targetTypeCode = Type.GetTypeCode(targetType);
            return targetTypeCode != TypeCode.Object && targetTypeCode != TypeCode.String;
        }


        /// <summary>
        /// Gets the message representing a failed validation.
        /// </summary>
        /// <param name="objectToValidate">The object for which validation was performed.</param>
        /// <param name="key">The key representing the value being validated for <paramref name="objectToValidate"/>.</param>
        /// <returns>The message representing the validation failure.</returns>
        protected internal override string GetMessage(object objectToValidate, string key) {
            return string.Format(
                CultureInfo.CurrentCulture,
                this.MessageTemplate,
                objectToValidate,
                key,
                this.Tag,
                this.targetType.FullName);
        }

        /// <summary>
        /// Gets the Default Message Template when the validator is non negated.
        /// </summary>
        protected override string DefaultNonNegatedMessageTemplate {
            get { return Resources.TypeConversionNonNegatedDefaultMessageTemplate; }
        }

        /// <summary>
        /// Gets the Default Message Template when the validator is negated.
        /// </summary>
        protected override string DefaultNegatedMessageTemplate {
            get { return Resources.TypeConversionNegatedDefaultMessageTemplate; }
        }

        /// <summary>
        /// Target type for conversion.
        /// </summary>
        public Type TargetType {
            get { return this.targetType; }
        }
    }
}

