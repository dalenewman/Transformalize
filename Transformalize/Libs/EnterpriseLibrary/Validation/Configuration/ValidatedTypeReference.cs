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
using System.ComponentModel;
using Transformalize.Libs.EnterpriseLibrary.Common.Configuration;
using Transformalize.Libs.EnterpriseLibrary.Common.Configuration.Design;

namespace Transformalize.Libs.EnterpriseLibrary.Validation.Configuration
{
	/// <summary>
	/// Represents validation information for a type and its members.
	/// </summary>
    /// <seealso cref="ValidationRulesetData"/>
    [TypePickingCommand("Name", Replace = CommandReplacement.DefaultAddCommandReplacement, CommandModelTypeName = ValidationDesignTime.CommandTypeNames.AddValidatedTypeCommand)]
    [ResourceDescription(typeof(DesignResources), "ValidatedTypeReferenceDescription")]
    [ResourceDisplayName(typeof(DesignResources), "ValidatedTypeReferenceDisplayName")]
    [ViewModel(ValidationDesignTime.ViewModelTypeNames.ValidatedTypeReferenceViewModel)]
	public class ValidatedTypeReference : NamedConfigurationElement
	{
		/// <summary>
		/// <para>Initializes a new instance of the <see cref="ValidatedTypeReference"/> class.</para>
		/// </summary>
		public ValidatedTypeReference()
		{ }

		/// <summary>
		/// <para>Initializes a new instance of the <see cref="ValidatedTypeReference"/> class with a type.</para>
		/// </summary>
		/// <param name="type">The represented type.</param>
		public ValidatedTypeReference(Type type)
			: base(GetFullName(type))
		{ }

        private static string GetFullName(Type type)
        {
            if (type == null) throw new ArgumentNullException("type");

            return type.FullName;
        }
        /// <summary>
        /// 
        /// </summary>
        [DesignTimeReadOnly(true)]
        public override string Name
        {
            get { return base.Name; }
            set { base.Name = value; }
        }

		private const string RulesetsPropertyName = "";
		/// <summary>
		/// Gets the collection with the validation rulesets configured the represented type.
		/// </summary>
		[ConfigurationProperty(RulesetsPropertyName, IsDefaultCollection = true)]
        [ResourceDescription(typeof(DesignResources), "ValidatedTypeReferenceRulesetsDescription")]
        [ResourceDisplayName(typeof(DesignResources), "ValidatedTypeReferenceRulesetsDisplayName")]
        [PromoteCommands]
		public ValidationRulesetDataCollection Rulesets
		{
			get { return (ValidationRulesetDataCollection)this[RulesetsPropertyName]; }
		}

		private const string DefaultRulePropertyName = "defaultRuleset";

		/// <summary>
		/// Gets or sets the default ruleset for the represented type.
		/// </summary>
		[ConfigurationProperty(DefaultRulePropertyName)]
        [Reference(typeof(ValidationRulesetData), ScopeIsDeclaringElement = true)]
        [ResourceDescription(typeof(DesignResources), "ValidatedTypeReferenceDefaultRulesetDescription")]
        [ResourceDisplayName(typeof(DesignResources), "ValidatedTypeReferenceDefaultRulesetDisplayName")]
		public string DefaultRuleset
		{
			get { return (string)this[DefaultRulePropertyName]; }
			set { this[DefaultRulePropertyName] = value; }
		}

        private const string AssemblyNamePropertyName = "assemblyName";
		/// <summary>
		/// Used to resolve the reference type in designtime. This property is ignored at runtime.
		/// </summary>
		[ConfigurationProperty(AssemblyNamePropertyName)]
        [ResourceDescription(typeof(DesignResources), "ValidatedTypeReferenceAssemblyNameDescription")]
        [ResourceDisplayName(typeof(DesignResources), "ValidatedTypeReferenceAssemblyNameDisplayName")]
        [Browsable(false)]
        public string AssemblyName
        {
            get { return (string)this[AssemblyNamePropertyName]; }
            set { this[AssemblyNamePropertyName] = value; }
        }
	}
}
