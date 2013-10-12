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
	/// Represents a collection of validator configuration objects.
	/// </summary>
	/// <seealso cref="ValidatorData"/>
    [ConfigurationCollection(typeof(ValidatorData), AddItemName = "validator", ClearItemsName="clear", RemoveItemName="remove")]
	public class ValidatorDataCollection : NameTypeConfigurationElementCollection<ValidatorData, CustomValidatorData>
	{
		/// <summary>
		/// <para>Initializes a new instance of the <see cref="ValidatorDataCollection"/> class.</para>
		/// </summary>
		public ValidatorDataCollection()
		{
			this.AddElementName = "validator";
		}
	}
}
