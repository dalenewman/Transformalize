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
using System.Collections.Generic;
using System.Configuration;
using Transformalize.Libs.EnterpriseLibrary.Common.Configuration.Design;
using Transformalize.Libs.EnterpriseLibrary.Validation.Validators;

namespace Transformalize.Libs.EnterpriseLibrary.Validation.Configuration
{
    /// <summary>
    /// Configuration object to describe an instance of class <see cref="OrCompositeValidator"/>.
    /// </summary>
    /// <seealso cref="OrCompositeValidator"/>
    /// <seealso cref="ValidatorData"/>
    [ResourceDescription(typeof(DesignResources), "OrCompositeValidatorDataDescription")]
    [ResourceDisplayName(typeof(DesignResources), "OrCompositeValidatorDataDisplayName")]
    public class OrCompositeValidatorData : ValidatorData
    {
        /// <summary>
        /// <para>Initializes a new instance of the <see cref="OrCompositeValidatorData"/> class.</para>
        /// </summary>
        public OrCompositeValidatorData() { Type = typeof(OrCompositeValidator); }

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="OrCompositeValidatorData"/> class with a name.</para>
        /// </summary>
        /// <param name="name">The name for the instance.</param>
        public OrCompositeValidatorData(string name)
            : base(name, typeof(OrCompositeValidator))
        { }

        private const string ValidatorsPropertyName = "";
        /// <summary>
        /// Gets the collection with the definitions for the validators composed by 
        /// the represented <see cref="OrCompositeValidator"/>.
        /// </summary>
        [ConfigurationProperty(ValidatorsPropertyName, IsDefaultCollection = true)]
        [ResourceDescription(typeof(DesignResources), "OrCompositeValidatorDataValidatorsDescription")]
        [ResourceDisplayName(typeof(DesignResources), "OrCompositeValidatorDataValidatorsDisplayName")]
        [PromoteCommands]
        public ValidatorDataCollection Validators
        {
            get { return (ValidatorDataCollection)this[ValidatorsPropertyName]; }
        }

        /// <summary>
        /// Creates the <see cref="OrCompositeValidator"/> described by the configuration object.
        /// </summary>
        /// <param name="targetType">The type of object that will be validated by the validator.</param>
        /// <param name="ownerType">The type of the object from which the value to validate is extracted.</param>
        /// <param name="memberValueAccessBuilder">The <see cref="MemberValueAccessBuilder"/> to use for validators that
        /// require access to properties.</param>
        /// <param name="validatorFactory">Factory to use when building nested validators.</param>
        /// <returns>The created <see cref="OrCompositeValidator"/>.</returns>
        protected override Validator DoCreateValidator(
            Type targetType,
            Type ownerType,
            MemberValueAccessBuilder memberValueAccessBuilder,
            ValidatorFactory validatorFactory)
        {
            List<Validator> childValidators = new List<Validator>(this.Validators.Count);
            foreach (IValidatorDescriptor validatorData in this.Validators)
            {
                childValidators.Add(validatorData.CreateValidator(targetType, ownerType, memberValueAccessBuilder, validatorFactory));
            }

            return new OrCompositeValidator(childValidators.ToArray());
        }
    }
}
