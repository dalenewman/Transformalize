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
	/// Represents a collection of validated properties.
	/// </summary>
    /// <seealso cref="ValidatedPropertyReference"/>
    [ConfigurationCollection(typeof(ValidatedPropertyReference), AddItemName = "property", ClearItemsName = "clear", RemoveItemName = "remove")]
	public class ValidatedPropertyReferenceCollection : NamedElementCollection<ValidatedPropertyReference>
	{
		/// <summary>
		/// <para>Initializes a new instance of the <see cref="ValidatedPropertyReferenceCollection"/> class.</para>
		/// </summary>
		public ValidatedPropertyReferenceCollection()
		{
			this.AddElementName = "property";
		}
	}
}
