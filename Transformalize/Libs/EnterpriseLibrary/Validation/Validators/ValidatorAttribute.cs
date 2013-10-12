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

namespace Transformalize.Libs.EnterpriseLibrary.Validation.Validators
{
    /// <summary>
    /// Describes the validation logic that must be applied to a language element when
    /// creating a <see cref="Validator"/> for a type.
    /// </summary>
    /// <remarks>
    /// Multiple validator attributes can be specified for a language element. 
    /// <para/>
    /// All the rules will be applied as an "And" composition unless the
    /// <see cref="ValidatorCompositionAttribute"/> overrides this behavior.
    /// <para/>
    /// A ruleset can be specified for the attribute, as well as message overrides 
    /// (either literal or resource based).
    /// </remarks>
    /// <seealso cref="Validation"/>
    /// <seealso cref="ValidationFactory"/>
    /// <seealso cref="Validator"/>
    /// <seealso cref="ValidatorCompositionAttribute"/>
    /// <seealso cref="IgnoreNullsAttribute"/>
    [AttributeUsage(AttributeTargets.Property
        | AttributeTargets.Field
        | AttributeTargets.Method
        | AttributeTargets.Class
        | AttributeTargets.Parameter,
        AllowMultiple = true,
        Inherited = false)]
    public abstract class ValidatorAttribute : BaseValidationAttribute, IValidatorDescriptor
    {
        Validator IValidatorDescriptor.CreateValidator(
            Type targetType,
            Type ownerType,
            MemberValueAccessBuilder memberValueAccessBuilder,
            ValidatorFactory validatorFactory)
        {
            return CreateValidator(targetType, ownerType, memberValueAccessBuilder, validatorFactory);
        }

        /// <summary>
        /// Creates the <see cref="Validator"/> described by the configuration object.
        /// </summary>
        /// <param name="targetType">The type of object that will be validated by the validator.</param>
        /// <param name="ownerType">The type of the object from which the value to validate is extracted.</param>
        /// <param name="memberValueAccessBuilder">The <see cref="MemberValueAccessBuilder"/> to use for validators that
        /// require access to properties.</param>
        /// <param name="validatorFactory">Factory to use when building nested validators.</param>
        /// <returns>The created <see cref="Validator"/>.</returns>
        /// <remarks>
        /// Sets the tag and message template to the created validator.
        /// </remarks>
        protected Validator CreateValidator(
            Type targetType,
            Type ownerType,
            MemberValueAccessBuilder memberValueAccessBuilder,
            ValidatorFactory validatorFactory)
        {
            Validator validator = DoCreateValidator(targetType, ownerType, memberValueAccessBuilder, validatorFactory);
            validator.Tag = this.Tag;
            validator.MessageTemplate = this.GetMessageTemplate();

            return validator;
        }

        /// <summary>
        /// Creates the <see cref="Validator"/> described by the configuration object.
        /// </summary>
        /// <param name="targetType">The type of object that will be validated by the validator.</param>
        /// <param name="ownerType">The type of the object from which the value to validate is extracted.</param>
        /// <param name="memberValueAccessBuilder">The <see cref="MemberValueAccessBuilder"/> to use for validators that
        /// require access to properties.</param>
        /// <param name="validatorFactory">Factory to use when building nested validators.</param>
        /// <returns>The created <see cref="Validator"/>.</returns>
        /// <remarks>
        /// The default implementation invokes <see cref="ValidatorAttribute.DoCreateValidator(Type)"/>. Subclasses requiring access to all
        /// the parameters or this method may override it instead of <see cref="ValidatorAttribute.DoCreateValidator(Type)"/>.
        /// </remarks>
        protected virtual Validator DoCreateValidator(
            Type targetType,
            Type ownerType,
            MemberValueAccessBuilder memberValueAccessBuilder,
            ValidatorFactory validatorFactory)
        {
            return DoCreateValidator(targetType);
        }

        /// <summary>
        /// Creates the <see cref="Validator"/> described by the attribute object providing validator specific
        /// information.
        /// </summary>
        /// <param name="targetType">The type of object that will be validated by the validator.</param>
        /// <remarks>This operation must be overriden by subclasses.</remarks>
        /// <returns>The created <see cref="Validator"/>.</returns>
        protected abstract Validator DoCreateValidator(Type targetType);
    }
}
