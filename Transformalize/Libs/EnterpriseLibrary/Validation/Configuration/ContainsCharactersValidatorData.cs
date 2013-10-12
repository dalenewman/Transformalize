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
	/// Configuration object to describe an instance of class <see cref="ContainsCharactersValidatorData"/>.
	/// </summary>
    [ResourceDescription(typeof(DesignResources), "ContainsCharactersValidatorDataDescription")]
    [ResourceDisplayName(typeof(DesignResources), "ContainsCharactersValidatorDataDisplayName")]
	public class ContainsCharactersValidatorData : ValueValidatorData
	{
		/// <summary>
		/// <para>Initializes a new instance of the <see cref="ContainsCharactersValidatorData"/> class.</para>
		/// </summary>
        public ContainsCharactersValidatorData() { Type = typeof(ContainsCharactersValidator); }

		/// <summary>
		/// <para>Initializes a new instance of the <see cref="ContainsCharactersValidatorData"/> class with a name.</para>
		/// </summary>
		/// <param name="name">The name for the instance.</param>
		public ContainsCharactersValidatorData(string name)
			: base(name, typeof(ContainsCharactersValidator))
		{ }

		private const string CharacterSetPropertyName = "characterSet";
		/// <summary>
		/// Gets or sets the string containing the characters to use by the represented 
		/// <see cref="ContainsCharactersValidator"/>.
		/// </summary>
        [ConfigurationProperty(CharacterSetPropertyName, IsRequired = true)]
        [ResourceDescription(typeof(DesignResources), "ContainsCharactersValidatorDataCharacterSetDescription")]
        [ResourceDisplayName(typeof(DesignResources), "ContainsCharactersValidatorDataCharacterSetDisplayName")]
		public string CharacterSet
		{
			get { return (string)this[CharacterSetPropertyName]; }
			set { this[CharacterSetPropertyName] = value; }
		}

		private const string ContainsCharactersPropertyName = "containsCharacter";
		/// <summary>
		/// Gets or sets the <see cref="ContainsCharacters"/> value indicating the behavior for 
		/// the represented <see cref="ContainsCharactersValidator"/>.
		/// </summary>
		[ConfigurationProperty(ContainsCharactersPropertyName, DefaultValue=ContainsCharacters.Any, IsRequired=true)]
        [ResourceDescription(typeof(DesignResources), "ContainsCharactersValidatorDataContainsCharactersDescription")]
        [ResourceDisplayName(typeof(DesignResources), "ContainsCharactersValidatorDataContainsCharactersDisplayName")]
		public ContainsCharacters ContainsCharacters
		{
			get { return (ContainsCharacters)this[ContainsCharactersPropertyName]; }
			set { this[ContainsCharactersPropertyName] = value; }
		}

		/// <summary>
		/// Creates the <see cref="ContainsCharactersValidator"/> described by the configuration object.
		/// </summary>
		/// <param name="targetType">The type of object that will be validated by the validator.</param>
		/// <returns>The created <see cref="ContainsCharactersValidator"/>.</returns>	
		protected override Validator DoCreateValidator(Type targetType)
		{
			return new ContainsCharactersValidator(CharacterSet, this.ContainsCharacters, Negated);
		}
	}
}
