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

namespace Transformalize.Libs.EnterpriseLibrary.SemanticLogging.Sinks
{
    /// <summary>
    /// Defines the behavior when the roll file is created.
    /// </summary>
    public enum RollFileExistsBehavior
    {
        /// <summary>
        /// Overwrites the file if it already exists.
        /// </summary>
        Overwrite,

        /// <summary>
        /// Use a sequence number at the end of the generated file if it already exists.
        /// </summary>
        /// <remarks>
        /// If it fails again then increment the sequence until a non existent filename is found.
        /// </remarks>
        Increment
    }
}
