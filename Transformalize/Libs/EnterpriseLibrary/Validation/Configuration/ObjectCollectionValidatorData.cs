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
using System.Configuration;
using Transformalize.Libs.EnterpriseLibrary.Common.Configuration;
using Transformalize.Libs.EnterpriseLibrary.Common.Configuration.Design;
using Transformalize.Libs.EnterpriseLibrary.Validation.Validators;

namespace Transformalize.Libs.EnterpriseLibrary.Validation.Configuration
{
    /// <summary>
    /// Configuration object to describe an instance of class <see cref="ObjectCollectionValidator"/>.
    /// </summary>
    [ResourceDescription(typeof(DesignResources), "ObjectCollectionValidatorDataDescription")]
    [ResourceDisplayName(typeof(DesignResources), "ObjectCollectionValidatorDataDisplayName")]
    public class ObjectCollectionValidatorData : ValidatorData
    {
        private static readonly AssemblyQualifiedTypeNameConverter typeConverter = new AssemblyQualifiedTypeNameConverter();

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="ObjectCollectionValidatorData"/> class.</para>
        /// </summary>
        public ObjectCollectionValidatorData() { Type = typeof(ObjectCollectionValidator); }

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="ObjectCollectionValidatorData"/> class with a name.</para>
        /// </summary>
        /// <param name="name">The name for the instance.</param>
        public ObjectCollectionValidatorData(string name)
            : base(name, typeof(ObjectCollectionValidator))
        { }

        /// <summary>
        /// Creates the <see ObjectCollectionValidator="ObjectValidator"/> described by the configuration object.
        /// </summary>
        /// <param name="targetType">The type of object that will be validated by the validator.</param>
        /// <remarks>This method must not be called on this class. Call 
        /// <see cref="ObjectCollectionValidatorData.DoCreateValidator(Type, Type, MemberValueAccessBuilder, ValidatorFactory)"/>.</remarks>
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
        /// <returns>The created <see cref="ObjectCollectionValidator"/>.</returns>
        /// <seealso cref="ObjectCollectionValidator"/>
        protected override Validator DoCreateValidator(
            Type targetType,
            Type ownerType,
            MemberValueAccessBuilder memberValueAccessBuilder,
            ValidatorFactory validatorFactory)
        {
            Type configuredTargetType = this.TargetType;

            if (configuredTargetType != null)
            {
                return new ObjectCollectionValidator(configuredTargetType, validatorFactory, this.TargetRuleset);
            }
            else
            {
                return new ObjectCollectionValidator(validatorFactory, this.TargetRuleset);
            }
        }

        /// <summary>
        /// Gets or sets the target element type.
        /// </summary>
        /// <value>
        /// The target element type.
        /// </value>
        public Type TargetType
        {
            get { return (Type)typeConverter.ConvertFrom(TargetTypeName); }
            set { TargetTypeName = typeConverter.ConvertToString(value); }
        }

		private const string TargetTypePropertyName = "targetType";
		/// <summary>
		/// Gets or sets the name of the target element type for the represented validator.
		/// </summary>
		/// <seealso cref="ObjectCollectionValidatorData.TargetTypeName"/>
		[ConfigurationProperty(TargetTypePropertyName)]
        [Editor(CommonDesignTime.EditorTypes.TypeSelector, CommonDesignTime.EditorTypes.UITypeEditor)]
        [ResourceDescription(typeof(DesignResources), "ObjectCollectionValidatorDataTargetTypeNameDescription")]
        [ResourceDisplayName(typeof(DesignResources), "ObjectCollectionValidatorDataTargetTypeNameDisplayName")]
		public string TargetTypeName
		{
			get { return (string)this[TargetTypePropertyName]; }
			set { this[TargetTypePropertyName] = value; }
		}

		private const string TargetRulesetPropertyName = "targetRuleset";
		/// <summary>
		/// Gets or sets the name for the target ruleset for the represented validator.
		/// </summary>
        [ConfigurationProperty(TargetRulesetPropertyName, DefaultValue = "")]
        [ResourceDescription(typeof(DesignResources), "ObjectCollectionValidatorDataTargetRulesetDescription")]
        [ResourceDisplayName(typeof(DesignResources), "ObjectCollectionValidatorDataTargetRulesetDisplayName")]
		public string TargetRuleset
		{
			get { return (string)this[TargetRulesetPropertyName]; }
			set { this[TargetRulesetPropertyName] = value; }
		}
	}
}
