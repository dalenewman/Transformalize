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
using System.Collections;
using Transformalize.Libs.EnterpriseLibrary.Common.Configuration;
using Transformalize.Libs.EnterpriseLibrary.Validation.Configuration;

namespace Transformalize.Libs.EnterpriseLibrary.Validation.Validators
{
    /// <summary>
    /// Performs validation on collection objects by applying the validation rules specified for a supplied type
    /// to its members.
    /// </summary>
    /// <seealso cref="ValidationFactory"/>
    [ConfigurationElementType(typeof(ObjectCollectionValidatorData))]
    public class ObjectCollectionValidator : Validator
    {
        private readonly Type targetType;
        private readonly string targetRuleset;
        private readonly Validator targetTypeValidator;
        private readonly ValidatorFactory validatorFactory;

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="ObjectCollectionValidator"/>.</para>
        /// </summary>
        /// <remarks>
        /// The default ruleset will be used for validation.
        /// </remarks>
        public ObjectCollectionValidator()
            : this(ValidationFactory.DefaultCompositeValidatorFactory)
        { }

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="ObjectCollectionValidator"/> 
        /// using the supplied ruleset.</para>
        /// </summary>
        /// <param name="validatorFactory">Factory to use when building nested validators.</param>
        public ObjectCollectionValidator(ValidatorFactory validatorFactory)
            : this(validatorFactory, string.Empty)
        { }

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="ObjectCollectionValidator"/> 
        /// using the supplied ruleset.</para>
        /// </summary>
        /// <param name="validatorFactory">Factory to use when building nested validators.</param>
        /// <param name="targetRuleset">The ruleset to use.</param>
        /// <exception cref="ArgumentNullException">when <paramref name="targetRuleset"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentNullException">when <paramref name="validatorFactory"/> is <see langword="null"/>.</exception>
        public ObjectCollectionValidator(ValidatorFactory validatorFactory, string targetRuleset)
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
        /// <para>Initializes a new instance of the <see cref="ObjectCollectionValidator"/> for a target type.</para>
        /// </summary>
        /// <param name="targetType">The target type</param>
        /// <remarks>
        /// The default ruleset for <paramref name="targetType"/> will be used.
        /// </remarks>
        /// <exception cref="ArgumentNullException">when <paramref name="targetType"/> is <see langword="null"/>.</exception>
        public ObjectCollectionValidator(Type targetType)
            : this(targetType, string.Empty)
        { }

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="ObjectCollectionValidator"/> for a target type
        /// using the supplied ruleset.</para>
        /// </summary>
        /// <param name="targetType">The target type</param>
        /// <param name="targetRuleset">The ruleset to use.</param>
        /// <exception cref="ArgumentNullException">when <paramref name="targetType"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentNullException">when <paramref name="targetRuleset"/> is <see langword="null"/>.</exception>
        public ObjectCollectionValidator(Type targetType, string targetRuleset)
            : this(targetType, ValidationFactory.DefaultCompositeValidatorFactory, targetRuleset)
        { }

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="ObjectCollectionValidator"/> for a target type
        /// using the supplied ruleset.</para>
        /// </summary>
        /// <param name="targetType">The target type</param>
        /// <param name="validatorFactory">Factory to use when building nested validators.</param>
        /// <param name="targetRuleset">The ruleset to use.</param>
        /// <exception cref="ArgumentNullException">when <paramref name="targetType"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentNullException">when <paramref name="targetRuleset"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentNullException">when <paramref name="validatorFactory"/> is <see langword="null"/>.</exception>
        public ObjectCollectionValidator(Type targetType, ValidatorFactory validatorFactory, string targetRuleset)
            : base(null, null)
        {
            if (targetType == null)
            {
                throw new ArgumentNullException("targetType");
            }
            if (targetRuleset == null)
            {
                throw new ArgumentNullException("targetRuleset");
            }
            if (validatorFactory == null)
            {
                throw new ArgumentNullException("validatorFactory");
            }

            this.targetType = targetType;
            this.targetTypeValidator = validatorFactory.CreateValidator(targetType, targetRuleset);
            this.targetRuleset = targetRuleset;
            this.validatorFactory = null;
        }

        /// <summary>
        /// Validates by applying the validation rules for the target type specified for the receiver to the elements
        /// in <paramref name="objectToValidate"/>.
        /// </summary>
        /// <param name="objectToValidate">The object to validate.</param>
        /// <param name="currentTarget">The object on the behalf of which the validation is performed.</param>
        /// <param name="key">The key that identifies the source of <paramref name="objectToValidate"/>.</param>
        /// <param name="validationResults">The validation results to which the outcome of the validation should be stored.</param>
        /// <remarks>
        /// If <paramref name="objectToValidate"/> is <see langword="null"/> validation is ignored.
        /// <para/>
        /// A referece to a non collection object causes a validation failure and the validation rules
        /// for the configured target type will not be applied.
        /// <para/>
        /// Elements in the collection of a type not compatible with the configured target type causes a validation failure but
        /// do not affect validation on other elements.
        /// </remarks>
        public override void DoValidate(object objectToValidate,
            object currentTarget,
            string key,
            ValidationResults validationResults)
        {
            if (objectToValidate != null)
            {
                IEnumerable enumerable = objectToValidate as IEnumerable;
                if (enumerable != null)
                {
                    foreach (object element in enumerable)
                    {
                        if (element != null)
                        {
                            var elementType = element.GetType();

                            if (this.targetType == null || this.targetType.IsAssignableFrom(elementType))
                            {
                                // reset the current target and the key
                                this.GetValidator(elementType).DoValidate(element, element, null, validationResults);
                            }
                            else
                            {
                                // unlikely
                                this.LogValidationResult(validationResults,
                                    Resources.ObjectCollectionValidatorIncompatibleElementInTargetCollection,
                                    element,
                                    null);
                            }
                        }
                    }
                }
                else
                {
                    this.LogValidationResult(validationResults, Resources.ObjectCollectionValidatorTargetNotCollection, currentTarget, key);
                }
            }
        }

        private Validator GetValidator(Type elementType)
        {
            if (this.targetTypeValidator != null)
            {
                return this.targetTypeValidator;
            }
            else
            {
                return this.validatorFactory.CreateValidator(elementType, this.targetRuleset);
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
        /// Type of target being validated.
        /// </summary>
        public Type TargetType
        {
            get { return this.targetType; }
        }

        /// <summary>
        /// Ruleset to use when creating target validators.
        /// </summary>
        public string TargetRuleset
        {
            get { return this.targetRuleset; }
        }
    }
}
