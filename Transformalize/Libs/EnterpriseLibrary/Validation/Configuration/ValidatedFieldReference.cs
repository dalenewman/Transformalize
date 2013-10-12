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

using Transformalize.Libs.EnterpriseLibrary.Common.Configuration.Design;

namespace Transformalize.Libs.EnterpriseLibrary.Validation.Configuration
{
	/// <summary>
	/// Represents the validation information for a field.
	/// </summary>
	/// <seealso cref="ValidatedTypeReference"/>
	/// <seealso cref="ValidationRulesetData"/>
	/// <seealso cref="ValidatedMemberReference"/>
    [ResourceDescription(typeof(DesignResources), "ValidatedFieldReferenceDescription")]
    [ResourceDisplayName(typeof(DesignResources), "ValidatedFieldReferenceDisplayName")]
    [NameProperty("Name", NamePropertyDisplayFormat = "Field: {0}")]
	public sealed class ValidatedFieldReference : ValidatedMemberReference
	{
		/// <summary>
		/// <para>Initializes a new instance of the <see cref="ValidatedFieldReference"/> class.</para>
		/// </summary>
		public ValidatedFieldReference()
		{ }
            
		/// <summary>
		/// <para>Initializes a new instance of the <see cref="ValidatedFieldReference"/> class with a name.</para>
		/// </summary>
		/// <param name="name">The name for the instance.</param>
		public ValidatedFieldReference(string name)
			: base(name)
		{ }
	}
}
