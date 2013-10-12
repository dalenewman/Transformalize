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

using System;

namespace Transformalize.Libs.EnterpriseLibrary.Validation.Validators
{
	/// <summary>
	/// Indicates the target type defines self validation methods.
	/// </summary>
	/// <remarks>
	/// Types without this attribute will not be scanned for self validation methods.
	/// </remarks>
	/// <seealso cref="SelfValidationAttribute"/>
	/// <seealso cref="SelfValidationValidator"/>
	[AttributeUsage(AttributeTargets.Class, Inherited = false)]
	public sealed class HasSelfValidationAttribute : Attribute
	{ }
}
