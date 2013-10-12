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

using System.Collections.Generic;
using System.Globalization;
using Transformalize.Libs.EnterpriseLibrary.Common.Configuration;
using Transformalize.Libs.EnterpriseLibrary.Validation.Configuration;

namespace Transformalize.Libs.EnterpriseLibrary.Validation.Validators
{
    /// <summary>
    /// Performs validation on strings by verifying if it contains a character set using the <see cref="ContainsCharacters"/> mode.
    /// </summary>
    [ConfigurationElementType(typeof(ContainsCharactersValidatorData))]
    public class ContainsCharactersValidator : ValueValidator<string>
    {
        private string characterSet;
        private ContainsCharacters containsCharacters = ContainsCharacters.Any;

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="ContainsCharactersValidator"/>.</para>
        /// </summary>
        /// <param name="characterSet">The string containing the set of allowed characters.</param>
        public ContainsCharactersValidator(string characterSet)
            : this(characterSet, ContainsCharacters.Any)
        {
        }

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="ContainsCharactersValidator"/>.</para>
        /// </summary>
        /// <param name="characterSet">The string containing the set of allowed characters.</param>
        /// <param name="containsCharacters">The <see cref="ContainsCharacters"/> specifying the kind of character matching behavior.</param>
        public ContainsCharactersValidator(string characterSet, ContainsCharacters containsCharacters)
            : this(characterSet, containsCharacters, false)
        {
        }

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="ContainsCharactersValidator"/>.</para>
        /// </summary>
        /// <param name="characterSet">The string containing the set of allowed characters.</param>
        /// <param name="containsCharacters">The <see cref="ContainsCharacters"/> specifying the kind of character matching behavior.</param>
        /// <param name="messageTemplate">The message template to use when logging results.</param>
        public ContainsCharactersValidator(string characterSet, ContainsCharacters containsCharacters, string messageTemplate)
            : this(characterSet, containsCharacters, messageTemplate, false)
        { }

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="ContainsCharactersValidator"/>.</para>
        /// </summary>
        /// <param name="characterSet">The string containing the set of allowed characters.</param>
        /// <param name="containsCharacters">The <see cref="ContainsCharacters"/> specifying the kind of character matching behavior.</param>
        /// <param name="negated">Indicates if the validation logic represented by the validator should be negated.</param>
        public ContainsCharactersValidator(string characterSet, ContainsCharacters containsCharacters, bool negated)
            : this(characterSet, containsCharacters, null, negated)
        { }

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="ContainsCharactersValidator"/>.</para>
        /// </summary>
        /// <param name="characterSet">The string containing the set of allowed characters.</param>
        /// <param name="containsCharacters">The <see cref="ContainsCharacters"/> specifying the kind of character matching behavior.</param>
        /// <param name="messageTemplate">The message template to use when logging results.</param>
        /// <param name="negated">Indicates if the validation logic represented by the validator should be negated.</param>
        public ContainsCharactersValidator(string characterSet, ContainsCharacters containsCharacters, string messageTemplate, bool negated)
            : base(messageTemplate, null, negated)
        {
            ValidatorArgumentsValidatorHelper.ValidateContainsCharacterValidator(characterSet);

            this.characterSet = characterSet;
            this.containsCharacters = containsCharacters;
        }

        /// <summary>
        /// Implements the validation logic for the receiver.
        /// </summary>
        /// <param name="objectToValidate">The object to validate.</param>
        /// <param name="currentTarget">The object on the behalf of which the validation is performed.</param>
        /// <param name="key">The key that identifies the source of <paramref name="objectToValidate"/>.</param>
        /// <param name="validationResults">The validation results to which the outcome of the validation should be stored.</param>
        protected override void DoValidate(string objectToValidate, object currentTarget, string key, ValidationResults validationResults)
        {
            bool logError = false;
            bool isObjectToValidateNull = objectToValidate == null;

            if (!isObjectToValidateNull)
            {
                if (ContainsCharacters.Any == this.ContainsCharacters)
                {
                    List<char> characterSetArray = new List<char>(characterSet);
                    bool containsCharacterFromSet = false;
                    foreach (char ch in objectToValidate)
                    {
                        if (characterSetArray.Contains(ch))
                        {
                            containsCharacterFromSet = true;
                            break;
                        }

                    }
                    logError = !containsCharacterFromSet;
                }
                else if (ContainsCharacters.All == this.ContainsCharacters)
                {
                    List<char> objectToValidateArray = new List<char>(objectToValidate);
                    bool containsAllCharacters = true;
                    foreach (char ch in CharacterSet)
                    {
                        if (!objectToValidateArray.Contains(ch))
                        {
                            containsAllCharacters = false;
                            break;
                        }
                    }
                    logError = !containsAllCharacters;
                }
            }

            if (isObjectToValidateNull || (logError != Negated))
            {
                this.LogValidationResult(validationResults,
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
                this.CharacterSet,
                this.ContainsCharacters);
        }

        /// <summary>
        /// Gets the Default Message Template when the validator is non negated.
        /// </summary>
        protected override string DefaultNonNegatedMessageTemplate
        {
            get { return Resources.ContainsCharactersNonNegatedDefaultMessageTemplate; }
        }

        /// <summary>
        /// Gets the Default Message Template when the validator is negated.
        /// </summary>
        protected override string DefaultNegatedMessageTemplate
        {
            get { return Resources.ContainsCharactersNegatedDefaultMessageTemplate; }
        }

        /// <summary>
        /// The character set to validate against.
        /// </summary>
        public string CharacterSet
        {
            get { return characterSet; }
        }

        /// <summary>
        /// How does this validator check?
        /// </summary>
        public ContainsCharacters ContainsCharacters
        {
            get { return containsCharacters; }
        }
    }
}
