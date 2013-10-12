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
using Transformalize.Libs.EnterpriseLibrary.Common.Configuration;
using Transformalize.Libs.EnterpriseLibrary.Validation.Configuration;

namespace Transformalize.Libs.EnterpriseLibrary.Validation.Validators
{
    /// <summary>
    /// Performs validation on objects by applying the validation rules specified for a supplied type.
    /// </summary>
    /// <seealso cref="ValidationFactory"/>
    [ConfigurationElementType(typeof(ObjectValidatorData))]
    public class ObjectValidator : Validator
    {
        private readonly Type targetType;
        private readonly string targetRuleset;
        private readonly Validator targetTypeValidator;
        private readonly ValidatorFactory validatorFactory;

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="ObjectValidator"/> for a target type
        /// using the supplied ruleset.</para>
        /// </summary>
        public ObjectValidator()
            : this(ValidationFactory.DefaultCompositeValidatorFactory)
        { }

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="ObjectValidator"/> for a target type
        /// using the supplied ruleset.</para>
        /// </summary>
        /// <param name="validatorFactory">Factory to use when building nested validators.</param>
        /// <exception cref="ArgumentNullException">when <paramref name="validatorFactory"/> is <see langword="null"/>.</exception>
        public ObjectValidator(ValidatorFactory validatorFactory)
            : this(validatorFactory, string.Empty)
        { }

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="ObjectValidator"/> for a target type
        /// using the supplied ruleset.</para>
        /// </summary>
        /// <param name="targetRuleset">The ruleset to use.</param>
        /// <param name="validatorFactory">Factory to use when building nested validators.</param>
        /// <exception cref="ArgumentNullException">when <paramref name="validatorFactory"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentNullException">when <paramref name="targetRuleset"/> is <see langword="null"/>.</exception>
        public ObjectValidator(ValidatorFactory validatorFactory, string targetRuleset)
            : base(null, null)
        {
            if (validatorFactory == null)
            {
                throw new ArgumentNullException("validatorFactory");
            }
            if (targetRuleset == null)
            {
                throw new ArgumentNullException("targetRuleset");
            }

            this.targetType = null;
            this.targetTypeValidator = null;
            this.targetRuleset = targetRuleset;
            this.validatorFactory = validatorFactory;
        }

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="ObjectValidator"/> for a target type.</para>
        /// </summary>
        /// <param name="targetType">The target type</param>
        /// <remarks>
        /// The default ruleset for <paramref name="targetType"/> will be used.
        /// </remarks>
        /// <exception cref="ArgumentNullException">when <paramref name="targetType"/> is <see langword="null"/>.</exception>
        public ObjectValidator(Type targetType)
            : this(targetType, string.Empty)
        { }

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="ObjectValidator"/> for a target type
        /// using the supplied ruleset.</para>
        /// </summary>
        /// <param name="targetType">The target type</param>
        /// <param name="targetRuleset">The ruleset to use.</param>
        /// <exception cref="ArgumentNullException">when <paramref name="targetType"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentNullException">when <paramref name="targetRuleset"/> is <see langword="null"/>.</exception>
        public ObjectValidator(Type targetType, string targetRuleset)
            : this(targetType, ValidationFactory.DefaultCompositeValidatorFactory, targetRuleset)
        { }

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="ObjectValidator"/> for a target type
        /// using the supplied ruleset.</para>
        /// </summary>
        /// <param name="targetType">The target type</param>
        /// <param name="targetRuleset">The ruleset to use.</param>
        /// <param name="validatorFactory">Factory to use when building nested validators.</param>
        /// <exception cref="ArgumentNullException">when <paramref name="targetType"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentNullException">when <paramref name="targetRuleset"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentNullException">when <paramref name="validatorFactory"/> is <see langword="null"/>.</exception>
        public ObjectValidator(Type targetType, ValidatorFactory validatorFactory, string targetRuleset)
            : base(null, null)
        {
            if (targetType == null)
            {
                throw new ArgumentNullException("targetType");
            }
            if (validatorFactory == null)
            {
                throw new ArgumentNullException("validatorFactory");
            }
            if (targetRuleset == null)
            {
                throw new ArgumentNullException("targetRuleset");
            }

            this.targetType = targetType;
            this.targetTypeValidator = validatorFactory.CreateValidator(targetType, targetRuleset);
            this.targetRuleset = targetRuleset;
            this.validatorFactory = null;
        }

        /// <summary>
        /// Validates by applying the validation rules for the target type specified for the receiver.
        /// </summary>
        /// <param name="objectToValidate">The object to validate.</param>
        /// <param name="currentTarget">The object on the behalf of which the validation is performed.</param>
        /// <param name="key">The key that identifies the source of <paramref name="objectToValidate"/>.</param>
        /// <param name="validationResults">The validation results to which the outcome of the validation should be stored.</param>
        /// <remarks>
        /// If <paramref name="objectToValidate"/> is <see langword="null"/> validation is ignored.
        /// <para/>
        /// A referece to an instance of a type not compatible with the configured target type
        /// causes a validation failure.
        /// </remarks>
        public override void DoValidate(object objectToValidate,
            object currentTarget,
            string key,
            ValidationResults validationResults)
        {
            if (objectToValidate != null)
            {
                Type objectToValidateType = objectToValidate.GetType();
                if (this.targetType == null || this.targetType.IsAssignableFrom(objectToValidateType))
                {
                    // reset the current target and the key
                    this.GetValidator(objectToValidateType).DoValidate(objectToValidate, objectToValidate, null, validationResults);
                }
                else
                {
                    // unlikely
                    this.LogValidationResult(validationResults, Resources.ObjectValidatorInvalidTargetType, currentTarget, key);
                }
            }
        }

        private Validator GetValidator(Type objectToValidateType)
        {
            if (this.targetTypeValidator != null)
            {
                return this.targetTypeValidator;
            }
            else
            {
                return this.validatorFactory.CreateValidator(objectToValidateType, this.targetRuleset);
            }
        }

        /// <summary>
        /// Gets the message template to use when logging results no message is supplied.
        /// </summary>
        protected override string DefaultMessageTemplate
        {
            get { return null; }
        }

        /// <summary>
        /// Target type being validated.
        /// </summary>
        public Type TargetType
        {
            get { return this.targetType; }
        }

        /// <summary>
        /// Rule set to use when creating target validators.
        /// </summary>
        public string TargetRuleset
        {
            get { return this.targetRuleset; }
        }
    }
}
