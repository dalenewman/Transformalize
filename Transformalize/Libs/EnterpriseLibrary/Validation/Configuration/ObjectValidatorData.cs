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
using System.Configuration;
using Transformalize.Libs.EnterpriseLibrary.Common.Configuration.Design;
using Transformalize.Libs.EnterpriseLibrary.Validation.Validators;

namespace Transformalize.Libs.EnterpriseLibrary.Validation.Configuration
{
    /// <summary>
    /// Configuration object to describe an instance of class <see cref="ObjectValidator"/>.
    /// </summary>
    [ResourceDescription(typeof(DesignResources), "ObjectValidatorDataDescription")]
    [ResourceDisplayName(typeof(DesignResources), "ObjectValidatorDataDisplayName")]
    public class ObjectValidatorData : ValidatorData
    {
        /// <summary>
        /// <para>Initializes a new instance of the <see cref="ObjectValidatorData"/> class.</para>
        /// </summary>
        public ObjectValidatorData()
        {
            this.Type = typeof(ObjectValidator);
        }

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="ObjectValidatorData"/> class with a name.</para>
        /// </summary>
        /// <param name="name">The name for the instance.</param>
        public ObjectValidatorData(string name)
            : base(name, typeof(ObjectValidator))
        { }

        /// <summary>
        /// Creates the <see cref="ObjectValidator"/> described by the configuration object.
        /// </summary>
        /// <param name="targetType">The type of object that will be validated by the validator.</param>
        /// <remarks>This method must not be called on this class. Call 
        /// <see cref="ObjectValidatorData.DoCreateValidator(Type, Type, MemberValueAccessBuilder, ValidatorFactory)"/>.</remarks>
        protected override Validator DoCreateValidator(Type targetType)
        {
            throw new NotImplementedException(Resources.ExceptionShouldNotCall);
        }

        /// <summary>
        /// Creates the <see cref="ObjectValidator"/> described by the configuration object.
        /// </summary>
        /// <param name="targetType">The type of object that will be validated by the validator.</param>
        /// <param name="ownerType">The type of the object from which the value to validate is extracted.</param>
        /// <param name="memberValueAccessBuilder">The <see cref="MemberValueAccessBuilder"/> to use for validators that
        /// require access to properties.</param>
        /// <param name="validatorFactory">Factory to use when building nested validators.</param>
        /// <returns>The created <see cref="ObjectValidator"/>.</returns>
        /// <seealso cref="ObjectValidator"/>
        protected override Validator DoCreateValidator(
            Type targetType,
            Type ownerType,
            MemberValueAccessBuilder memberValueAccessBuilder,
            ValidatorFactory validatorFactory)
        {
            if (this.ValidateActualType)
            {
                return new ObjectValidator(validatorFactory, this.TargetRuleset);
            }
            else
            {
                return new ObjectValidator(targetType, validatorFactory, this.TargetRuleset);
            }
        }

        private const string TargetRulesetPropertyName = "targetRuleset";
        /// <summary>
        /// Gets or sets the name for the target ruleset for the represented validator.
        /// </summary>
        [ConfigurationProperty(TargetRulesetPropertyName)]
        [ResourceDescription(typeof(DesignResources), "ObjectValidatorDataTargetRulesetDescription")]
        [ResourceDisplayName(typeof(DesignResources), "ObjectValidatorDataTargetRulesetDisplayName")]
        public string TargetRuleset
        {
            get { return (string)this[TargetRulesetPropertyName]; }
            set { this[TargetRulesetPropertyName] = value; }
        }

        private const string ValidateActualTypePropertyName = "validateActualType";
        /// <summary>
        /// Gets or sets the value indicating whether to validate based on the static type or the actual type.
        /// </summary>
        [ConfigurationProperty(ValidateActualTypePropertyName, DefaultValue = false)]
        [ResourceDescription(typeof(DesignResources), "ObjectValidatorDataValidateActualTypeDescription")]
        [ResourceDisplayName(typeof(DesignResources), "ObjectValidatorDataValidateActualTypeDisplayName")]
        public bool ValidateActualType
        {
            get { return (bool)this[ValidateActualTypePropertyName]; }
            set { this[ValidateActualTypePropertyName] = value; }
        }
    }
}
