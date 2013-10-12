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

namespace Transformalize.Libs.EnterpriseLibrary.Validation
{
	/// <summary>
	/// Specifies the kind of filtering to perform for <see cref="ValidationResults.FindAll"/>
	/// </summary>
	public enum TagFilter
	{
		/// <summary>
		/// Include results with the supplied tags.
		/// </summary>
		Include,

		/// <summary>
		/// Ignore results with the supplied tags.
		/// </summary>
		Ignore
	}
}
