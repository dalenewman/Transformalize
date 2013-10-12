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
	/// Represents the behavior of the <see cref="ContainsCharactersValidator"/>.
	/// </summary>
	public enum ContainsCharacters
	{
		/// <summary>
		/// Indicates that validation is considered successful when at least one character in the
		/// character set is included in the validated value.
		/// </summary>
		Any,
		/// <summary>
		/// Indicates that validation is considered successful when at all the characters in the
		/// character set are included in the validated value.
		/// </summary>
		All
	}

}
