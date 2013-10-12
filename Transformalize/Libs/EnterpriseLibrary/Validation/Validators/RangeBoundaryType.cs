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
	/// Indicates how to interpret a range boundary.
	/// </summary>
	public enum RangeBoundaryType
	{
		/// <summary>
		/// Ignore the range boundary.
		/// </summary>
		Ignore = 0,

		/// <summary>
		/// Allow values equal to the boundary.
		/// </summary>
		Inclusive = 1,

		/// <summary>
		/// Reject values equal to the boundary.
		/// </summary>
		Exclusive = 2
	}
}
