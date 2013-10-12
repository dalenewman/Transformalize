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

using System.Diagnostics.CodeAnalysis;

namespace Transformalize.Libs.EnterpriseLibrary.Common.Utility
{
    /// <summary>
    /// Resolves string objects. 
    /// </summary>
    public interface IStringResolver
    {
        /// <summary>
        /// Returns a string represented by the receiver.
        /// </summary>
        /// <returns>The string object.</returns>
        [SuppressMessage("Microsoft.Design", "CA1024", Justification = "May be computationally expensive.")]
        string GetString();
    }
}
