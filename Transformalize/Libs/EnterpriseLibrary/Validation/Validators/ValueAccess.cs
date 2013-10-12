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
	/// Represents the logic of how to access values from a source object.
	/// </summary>
	public abstract class ValueAccess
	{
		/// <summary>
		/// Retrieves a value from <paramref name="source"/>.
		/// </summary>
		/// <param name="source">The source for the value.</param>
		/// <param name="value">The value retrieved from the <paramref name="source"/>.</param>
		/// <param name="valueAccessFailureMessage">A message describing the failure to access the value, if any.</param>
		/// <returns><see langword="true"/> when the retrieval was successful; <see langword="false"/> otherwise.</returns>
		/// <remarks>Subclasses provide concrete value accessing behaviors.</remarks>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1007",
			Justification = "Generics are not appropriate here")]
		public abstract bool GetValue(object source, out object value, out string valueAccessFailureMessage);

		/// <summary>
		/// Gets a hint of the location of the value relative to the object where it was retrieved from.
		/// </summary>
		public abstract string Key { get; }
	}
}
