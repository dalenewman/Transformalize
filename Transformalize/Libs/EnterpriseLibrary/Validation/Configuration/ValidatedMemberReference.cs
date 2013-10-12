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

using System.Configuration;
using Transformalize.Libs.EnterpriseLibrary.Common.Configuration;
using Transformalize.Libs.EnterpriseLibrary.Common.Configuration.Design;

namespace Transformalize.Libs.EnterpriseLibrary.Validation.Configuration
{
	/// <summary>
	/// Represents the validation information for a member of a type.
	/// </summary>
	/// <seealso cref="ValidationRulesetData"/>
	/// <seealso cref="ValidatedMemberReference"/>
	[ViewModel(ValidationDesignTime.ViewModelTypeNames.ValidatedMemberReferenceViewModel)]
    public abstract class ValidatedMemberReference : NamedConfigurationElement
	{
		/// <summary>
		/// <para>Initializes a new instance of the <see cref="ValidatedMemberReference"/> class.</para>
		/// </summary>
		protected ValidatedMemberReference()
		{ }

		/// <summary>
		/// <para>Initializes a new instance of the <see cref="ValidatedMemberReference"/> class with a name.</para>
		/// </summary>
		/// <param name="name">The name for the instance.</param>
		protected ValidatedMemberReference(string name)
			: base(name)
		{ }

		private const string ValidatorsPropertyName = "";
		/// <summary>
		/// Gets the collection of validators configured for the member.
		/// </summary>
		[ConfigurationProperty(ValidatorsPropertyName, IsDefaultCollection = true)]
        [ResourceDescription(typeof(DesignResources), "ValidatedMemberReferenceValidatorsDescription")]
        [ResourceDisplayName(typeof(DesignResources), "ValidatedMemberReferenceValidatorsDisplayName")]
        [PromoteCommands]
		public ValidatorDataCollection Validators
		{
			get { return (ValidatorDataCollection)this[ValidatorsPropertyName]; }
		}
	}
}
