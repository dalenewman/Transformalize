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
	/// Represents the validation information for a method.
	/// </summary>
	/// <seealso cref="ValidatedTypeReference"/>
	/// <seealso cref="ValidationRulesetData"/>
	/// <seealso cref="ValidatedMemberReference"/>
    [ResourceDescription(typeof(DesignResources), "ValidatedMethodReferenceDescription")]
    [ResourceDisplayName(typeof(DesignResources), "ValidatedMethodReferenceDisplayName")]
    [NameProperty("Name", NamePropertyDisplayFormat="Method: {0}")]
	public sealed class ValidatedMethodReference : ValidatedMemberReference
	{
		/// <summary>
		/// <para>Initializes a new instance of the <see cref="ValidatedMethodReference"/> class.</para>
		/// </summary>
		public ValidatedMethodReference()
		{ }

		/// <summary>
		/// <para>Initializes a new instance of the <see cref="ValidatedMethodReference"/> class with a name.</para>
		/// </summary>
		/// <param name="name">The name for the instance.</param>
		public ValidatedMethodReference(string name)
			: base(name)
		{ }
	}
}
