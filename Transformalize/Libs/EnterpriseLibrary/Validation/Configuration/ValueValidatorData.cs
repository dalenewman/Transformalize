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

namespace Transformalize.Libs.EnterpriseLibrary.Validation.Configuration
{
    /// <summary>
    /// Configuration object to describe an instance of class <see cref="ValueValidatorData"/>.
    /// </summary>
    public abstract class ValueValidatorData : ValidatorData
    {
        /// <summary>
        /// <para>Initializes a new instance of the <see cref="ContainsCharactersValidatorData"/> class.</para>
        /// </summary>
        protected ValueValidatorData()
        { }

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="ContainsCharactersValidatorData"/> class with a name.</para>
        /// </summary>
        /// <param name="name">The name for the instance.</param>
        /// <param name="type">The runtime type.</param>
        protected ValueValidatorData(string name, Type type)
            : base(name, type)
        { }

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="ContainsCharactersValidatorData"/> class with a name.</para>
        /// </summary>
        /// <param name="name">The name for the instance.</param>
        /// <param name="type">The runtime type.</param>
        /// <param name="negated"></param>
        protected ValueValidatorData(string name, Type type, bool negated)
            : base(name, type)
        {
            this.Negated = negated;
        }


        private const string NegatedPropertyName = "negated";
        /// <summary>
        /// Gets or sets the value to specify the behavior for the represented <see cref="Validator"/> should have a negated.
        /// </summary>
        [ConfigurationProperty(NegatedPropertyName, DefaultValue = false)]
        [ResourceDescription(typeof(DesignResources), "ValueValidatorDataNegatedDescription")]
        [ResourceDisplayName(typeof(DesignResources), "ValueValidatorDataNegatedDisplayName")]
        public bool Negated
        {
            get { return (bool)this[NegatedPropertyName]; }
            set { this[NegatedPropertyName] = value; }
        }
    }
}
