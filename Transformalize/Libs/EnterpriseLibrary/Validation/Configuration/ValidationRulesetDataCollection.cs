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
	/// <seealso cref="ValidationRulesetData"/>
    [ConfigurationCollection(typeof(ValidationRulesetData), AddItemName = "ruleset", ClearItemsName = "clear", RemoveItemName = "remove")]
	public class ValidationRulesetDataCollection : NamedElementCollection<ValidationRulesetData>
	{
		/// <summary>
		/// <para>Initializes a new instance of the <see cref="ValidationRulesetDataCollection"/> class.</para>
		/// </summary>
		public ValidationRulesetDataCollection()
		{
			this.AddElementName = "ruleset";
		}
	}
}
