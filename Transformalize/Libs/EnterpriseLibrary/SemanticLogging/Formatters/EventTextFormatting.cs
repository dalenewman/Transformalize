#region license
// ==============================================================================
// Microsoft patterns & practices Enterprise Library
// Semantic Logging Application Block
// ==============================================================================
// Copyright © Microsoft Corporation.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
// ==============================================================================
#endregion

namespace Transformalize.Libs.EnterpriseLibrary.SemanticLogging.Formatters
{
    /// <summary>
    /// Specifies formatting options .
    /// </summary>
    public enum EventTextFormatting
    {
        /// <summary>
        /// No special formatting applied. This is the default.
        /// </summary>
        None,

        /// <summary>
        /// Causes child objects to be indented.
        /// </summary>
        Indented
    }
}
