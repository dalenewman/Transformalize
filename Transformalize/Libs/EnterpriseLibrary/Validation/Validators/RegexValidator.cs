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
using System.Text.RegularExpressions;
using Transformalize.Libs.EnterpriseLibrary.Common;
using Transformalize.Libs.EnterpriseLibrary.Common.Configuration;
using Transformalize.Libs.EnterpriseLibrary.Validation.Configuration;

namespace Transformalize.Libs.EnterpriseLibrary.Validation.Validators
{
    /// <summary>
    /// Performs validation on strings by matching them to a <see cref="Regex"/>.
    /// </summary>
    [ConfigurationElementType(typeof(RegexValidatorData))]
    public class RegexValidator : ValueValidator<string>
    {
        private string pattern;
        private RegexOptions options;
        private string patternResourceName;
        private Type patternResourceType;

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="RegexValidator"/> class with a regex pattern.</para>
        /// </summary>
        /// <param name="pattern">The pattern to match.</param>
        public RegexValidator(string pattern)
            : this(pattern, RegexOptions.None)
        { }

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="RegexValidator"/> class with a regex pattern.</para>
        /// </summary>
        /// <param name="patternResourceName">The resource name containing the pattern for the regular expression.</param>
        /// <param name="patternResourceType">The type containing the resource for the regular expression.</param>
        public RegexValidator(string patternResourceName, Type patternResourceType)
            : this(patternResourceName, patternResourceType, RegexOptions.None)
        { }

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="RegexValidator"/> class with a regex pattern.</para>
        /// </summary>
        /// <param name="pattern">The pattern to match.</param>
        /// <param name="negated">True if the validator must negate the result of the validation.</param>
        public RegexValidator(string pattern, bool negated)
            : this(pattern, RegexOptions.None, negated)
        { }

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="RegexValidator"/> class with a regex pattern.</para>
        /// </summary>
        /// <param name="patternResourceName">The resource name containing the pattern for the regular expression.</param>
        /// <param name="patternResourceType">The type containing the resource for the regular expression.</param>
        /// <param name="negated">True if the validator must negate the result of the validation.</param>
        public RegexValidator(string patternResourceName, Type patternResourceType, bool negated)
            : this(patternResourceName, patternResourceType, RegexOptions.None, negated)
        { }

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="RegexValidator"/> class with a regex pattern and 
        /// matching options.</para>
        /// </summary>
        /// <param name="pattern">The pattern to match.</param>
        /// <param name="options">The <see cref="RegexOptions"/> to use when matching.</param>
        public RegexValidator(string pattern, RegexOptions options)
            : this(pattern, options, null)
        { }

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="RegexValidator"/> class with a regex pattern.</para>
        /// </summary>
        /// <param name="patternResourceName">The resource name containing the pattern for the regular expression.</param>
        /// <param name="patternResourceType">The type containing the resource for the regular expression.</param>
        /// <param name="options">The <see cref="RegexOptions"/> to use when matching.</param>
        public RegexValidator(string patternResourceName, Type patternResourceType, RegexOptions options)
            : this(patternResourceName, patternResourceType, options, null)
        { }

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="RegexValidator"/> class with a regex pattern and 
        /// matching options.</para>
        /// </summary>
        /// <param name="pattern">The pattern to match.</param>
        /// <param name="options">The <see cref="RegexOptions"/> to use when matching.</param>
        /// <param name="negated">True if the validator must negate the result of the validation.</param>
        public RegexValidator(string pattern, RegexOptions options, bool negated)
            : this(pattern, options, null, negated)
        { }

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="RegexValidator"/> class with a regex pattern and 
        /// matching options.</para>
        /// </summary>
        /// <param name="patternResourceName">The resource name containing the pattern for the regular expression.</param>
        /// <param name="patternResourceType">The type containing the resource for the regular expression.</param>
        /// <param name="options">The <see cref="RegexOptions"/> to use when matching.</param>
        /// <param name="negated">True if the validator must negate the result of the validation.</param>
        public RegexValidator(string patternResourceName, Type patternResourceType, RegexOptions options, bool negated)
            : this(patternResourceName, patternResourceType, options, null, negated)
        { }

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="RegexValidator"/> class with a regex pattern
        /// and a failure message template.</para>
        /// </summary>
        /// <param name="pattern">The pattern to match.</param>
        /// <param name="messageTemplate">The message template.</param>
        public RegexValidator(string pattern, string messageTemplate)
            : this(pattern, RegexOptions.None, messageTemplate)
        { }

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="RegexValidator"/> class with a regex pattern
        /// and a failure message template.</para>
        /// </summary>
        /// <param name="patternResourceName">The resource name containing the pattern for the regular expression.</param>
        /// <param name="patternResourceType">The type containing the resource for the regular expression.</param>
        /// <param name="messageTemplate">The message template.</param>
        public RegexValidator(string patternResourceName, Type patternResourceType, string messageTemplate)
            : this(patternResourceName, patternResourceType, RegexOptions.None, messageTemplate)
        { }

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="RegexValidator"/> class with a regex pattern
        /// and a failure message template.</para>
        /// </summary>
        /// <param name="pattern">The pattern to match.</param>
        /// <param name="messageTemplate">The message template.</param>
        /// <param name="negated">True if the validator must negate the result of the validation.</param>
        public RegexValidator(string pattern, string messageTemplate, bool negated)
            : this(pattern, RegexOptions.None, messageTemplate, negated)
        { }

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="RegexValidator"/> class with a regex pattern
        /// and a failure message template.</para>
        /// </summary>
        /// <param name="patternResourceName">The resource name containing the pattern for the regular expression.</param>
        /// <param name="patternResourceType">The type containing the resource for the regular expression.</param>
        /// <param name="messageTemplate">The message template.</param>
        /// <param name="negated">True if the validator must negate the result of the validation.</param>
        public RegexValidator(string patternResourceName, Type patternResourceType, string messageTemplate, bool negated)
            : this(patternResourceName, patternResourceType, RegexOptions.None, messageTemplate, negated)
        { }

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="RegexValidator"/> class with a regex pattern, 
        /// matching options and a failure message template.</para>
        /// </summary>
        /// <param name="pattern">The pattern to match.</param>
        /// <param name="options">The <see cref="RegexOptions"/> to use when matching.</param>
        /// <param name="messageTemplate">The message template.</param>
        public RegexValidator(string pattern, RegexOptions options, string messageTemplate)
            : this(pattern, options, messageTemplate, false)
        { }

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="RegexValidator"/> class with a regex pattern, 
        /// matching options and a failure message template.</para>
        /// </summary>
        /// <param name="patternResourceName">The resource name containing the pattern for the regular expression.</param>
        /// <param name="patternResourceType">The type containing the resource for the regular expression.</param>
        /// <param name="options">The <see cref="RegexOptions"/> to use when matching.</param>
        /// <param name="messageTemplate">The message template.</param>
        public RegexValidator(string patternResourceName, Type patternResourceType, RegexOptions options, string messageTemplate)
            : this(patternResourceName, patternResourceType, options, messageTemplate, false)
        { }

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="RegexValidator"/> class with a regex pattern, 
        /// matching options and a failure message template.</para>
        /// </summary>
        /// <param name="pattern">The pattern to match.</param>
        /// <param name="options">The <see cref="RegexOptions"/> to use when matching.</param>
        /// <param name="messageTemplate">The message template.</param>
        /// <param name="negated">True if the validator must negate the result of the validation.</param>
        public RegexValidator(string pattern, RegexOptions options, string messageTemplate, bool negated)
            : this(pattern, null, null, options, messageTemplate, negated)
        { }

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="RegexValidator"/> class with a regex pattern, 
        /// matching options and a failure message template.</para>
        /// </summary>
        /// <param name="options">The <see cref="RegexOptions"/> to use when matching.</param>
        /// <param name="messageTemplate">The message template.</param>
        /// <param name="negated">True if the validator must negate the result of the validation.</param>
        /// <param name="patternResourceName">The resource name containing the pattern for the regular expression.</param>
        /// <param name="patternResourceType">The type containing the resource for the regular expression.</param>
        public RegexValidator(string patternResourceName, Type patternResourceType, RegexOptions options, string messageTemplate, bool negated)
            : this(null, patternResourceName, patternResourceType, options, messageTemplate, negated)
        { }

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="RegexValidator"/> class with a regex pattern, 
        /// matching options and a failure message template.</para>
        /// </summary>
        /// <param name="pattern">The pattern to match.</param>
        /// <param name="options">The <see cref="RegexOptions"/> to use when matching.</param>
        /// <param name="messageTemplate">The message template.</param>
        /// <param name="negated">True if the validator must negate the result of the validation.</param>
        /// <param name="patternResourceName">The resource name containing the pattern for the regular expression.</param>
        /// <param name="patternResourceType">The type containing the resource for the regular expression.</param>
        public RegexValidator(string pattern, string patternResourceName, Type patternResourceType, RegexOptions options, string messageTemplate, bool negated)
            : base(messageTemplate, null, negated)
        {
            ValidatorArgumentsValidatorHelper.ValidateRegexValidator(pattern, patternResourceName, patternResourceType);

            this.pattern = pattern;
            this.options = options;
            this.patternResourceName = patternResourceName;
            this.patternResourceType = patternResourceType;
        }

        /// <summary>
        /// Validates by comparing <paramref name="objectToValidate"/> by matching it to the pattern
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
            bool logError = false;
            bool isObjectToValidateNull = objectToValidate == null;

            if (!isObjectToValidateNull)
            {
                string x = GetPattern();
                Regex regex = new Regex(x, this.options);
                logError = !regex.IsMatch(objectToValidate);
            }

            if (isObjectToValidateNull || (logError != Negated))
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
                this.pattern,
                this.options);
        }

        /// <summary>
        /// Gets the pattern used for building the regular expression.
        /// </summary>
        /// <returns>The regular expression pattern.</returns>
        public string GetPattern()
        {
            if (!string.IsNullOrEmpty(pattern))
            {
                return pattern;
            }
            else
            {
                return ResourceStringLoader.LoadString(patternResourceType.FullName, patternResourceName, patternResourceType.Assembly);
            }
        }

        /// <summary>
        /// Gets the Default Message Template when the validator is non negated.
        /// </summary>
        protected override string DefaultNonNegatedMessageTemplate
        {
            get { return Resources.RegexValidatorNonNegatedDefaultMessageTemplate; }
        }

        /// <summary>
        /// Gets the Default Message Template when the validator is negated.
        /// </summary>
        protected override string DefaultNegatedMessageTemplate
        {
            get { return Resources.RegexValidatorNegatedDefaultMessageTemplate; }
        }

        /// <summary>
        /// Regular expression pattern used.
        /// </summary>
        public string Pattern
        {
            get { return this.pattern; }
        }

        /// <summary>
        /// Any regex options specified.
        /// </summary>
        public RegexOptions? Options
        {
            get { return this.options; }
        }

        /// <summary>
        /// Resource name used to load regex pattern.
        /// </summary>
        public string PatternResourceName
        {
            get { return patternResourceName; }
        }

        /// <summary>
        /// Resource type used to look up regex pattern.
        /// </summary>
        public Type PatternResourceType
        {
            get { return patternResourceType; }
        }
    }
}
