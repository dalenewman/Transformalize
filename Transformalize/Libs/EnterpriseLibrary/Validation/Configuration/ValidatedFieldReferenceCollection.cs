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

namespace Transformalize.Libs.EnterpriseLibrary.Validation.Configuration
{
	/// <summary>
	/// Represents a collection of validated fields.
	/// </summary>
	/// <seealso cref="ValidatedFieldReference"/>
    [ConfigurationCollection(typeof(ValidatedFieldReference), AddItemName = "field", ClearItemsName = "clear", RemoveItemName = "remove")]
	public class ValidatedFieldReferenceCollection : NamedElementCollection<ValidatedFieldReference>
	{
		/// <summary>
		/// <para>Initializes a new instance of the <see cref="ValidatedFieldReferenceCollection"/> class.</para>
		/// </summary>
		public ValidatedFieldReferenceCollection()
		{
			this.AddElementName = "field";
		}
	}
}
