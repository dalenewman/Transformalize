//===============================================================================
// Microsoft patterns & practices Enterprise Library
// Core
//===============================================================================
// Copyright © Microsoft Corporation.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===============================================================================

using System;

namespace Transformalize.Libs.EnterpriseLibrary.Common.Configuration
{
	/// <summary>
	/// Represents the abstraction of an object with a name and a type.
	/// </summary>
	public interface IObjectWithNameAndType : IObjectWithName
	{
		/// <summary>
		/// Gets the type.
		/// </summary>
		Type Type { get; }
	}
}
