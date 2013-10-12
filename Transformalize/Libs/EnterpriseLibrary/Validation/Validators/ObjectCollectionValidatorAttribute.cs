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
    /// Represents an <see cref="ObjectValidator"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property
        | AttributeTargets.Field
        | AttributeTargets.Method
        | AttributeTargets.Parameter
        | AttributeTargets.Class,
        AllowMultiple = true,
        Inherited = false)]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1019",
        Justification = "Fields are used internally")]
    public sealed class ObjectCollectionValidatorAttribute : ValidatorAttribute
    {
        private readonly Type targetType;
        private string targetRuleset;

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="ObjectValidatorAttribute"/> class.</para>
        /// </summary>
        public ObjectCollectionValidatorAttribute()
        {
            this.targetType = null;
            this.targetRuleset = string.Empty;
        }

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="ObjectValidatorAttribute"/> class with a target type.</para>
        /// </summary>
        /// <param name="targetType">The target type.</param>
        /// <exception cref="ArgumentNullException">when <paramref name="targetType"/> is <see langword="null"/>.</exception>
        public ObjectCollectionValidatorAttribute(Type targetType)
            : this(targetType, string.Empty)
        { }

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="ObjectValidatorAttribute"/> class with a target type
        /// and a specific ruleset.</para>
        /// </summary>
        /// <param name="targetType">The target type.</param>
        /// <param name="targetRuleset">.</param>
        /// <exception cref="ArgumentNullException">when <paramref name="targetType"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentNullException">when <paramref name="targetRuleset"/> is <see langword="null"/>.</exception>
        /// <seealso cref="ObjectValidator(Type, string)"/>
        public ObjectCollectionValidatorAttribute(Type targetType, string targetRuleset)
        {
            if (targetType == null)
            {
                throw new ArgumentNullException("targetType");
            }
            if (targetRuleset == null)
            {
                throw new ArgumentNullException("targetRuleset");
            }

            this.targetType = targetType;
            this.targetRuleset = targetRuleset;
        }

        /// <summary>
        /// Creates the <see cref="Validator"/> described by the attribute object providing validator specific
        /// information.
        /// </summary>
        /// <param name="targetType">The type of object that will be validated by the validator.</param>
        /// <remarks>This method must not be called on this class. Call 
        /// <see cref="ObjectValidatorAttribute.DoCreateValidator(Type, Type, MemberValueAccessBuilder, ValidatorFactory)"/>.</remarks>
        protected override Validator DoCreateValidator(Type targetType)
        {
            throw new NotImplementedException(Resources.ExceptionShouldNotCall);
        }

        /// <summary>
        /// Creates the <see cref="ObjectCollectionValidator"/> described by the configuration object.
        /// </summary>
        /// <param name="targetType">The type of object that will be validated by the validator.</param>
        /// <param name="ownerType">The type of the object from which the value to validate is extracted.</param>
        /// <param name="memberValueAccessBuilder">The <see cref="MemberValueAccessBuilder"/> to use for validators that
        /// require access to properties.</param>
        /// <param name="validatorFactory">Factory to use when building nested validators.</param>
        /// <returns>The created <see cref="Validator"/>.</returns>
        protected override Validator DoCreateValidator(
            Type targetType,
            Type ownerType,
            MemberValueAccessBuilder memberValueAccessBuilder,
            ValidatorFactory validatorFactory)
        {
            // avoid supplied target type - that's the type of the collection
            if (this.TargetType != null)
            {
                return new ObjectCollectionValidator(this.TargetType, validatorFactory, this.targetRuleset);
            }
            else
            {
                return new ObjectCollectionValidator(validatorFactory, this.targetRuleset);
            }
        }

        /// <summary>
        /// Gets the target ruleset for the validated objects.
        /// </summary>
        public string TargetRuleset
        {
            get { return this.targetRuleset; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                this.targetRuleset = value;
            }
        }

        /// <summary>
        /// The target type
        /// </summary>
        public Type TargetType
        {
            get { return targetType; }
        }

        private readonly Guid typeId = Guid.NewGuid();

        /// <summary>
        /// Gets a unique identifier for this attribute.
        /// </summary>
        public override object TypeId
        {
            get
            {
                return this.typeId;
            }
        }
    }
}
