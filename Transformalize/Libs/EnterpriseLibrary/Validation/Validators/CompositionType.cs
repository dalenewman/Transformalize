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

namespace Transformalize.Libs.EnterpriseLibrary.Validation.Validators
{
	/// <summary>
	/// Specifies the kind of composition that is to be used when multiple validation attributes
	/// are supplied for a member info.
	/// </summary>
	public enum CompositionType
	{
		/// <summary>
		/// Use the <see cref="AndCompositeValidator"/>.
		/// </summary>
		And,

		/// <summary>
		/// Use the <see cref="OrCompositeValidator"/>.
		/// </summary>
		Or
	}
}
