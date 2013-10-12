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

namespace Transformalize.Libs.EnterpriseLibrary.Common.Configuration
{
    /// <summary>
    /// A set of string constants listing the names of the configuration
    /// sections used by the standard set of Entlib blocks.
    /// </summary>
    public static class BlockSectionNames
    {
        /// <summary>
        /// Data Access Application Block custom settings
        /// </summary>
        public const string Data = "dataConfiguration";

        /// <summary>
        /// Logging Application Block section name
        /// </summary>
        public const string Logging = "loggingConfiguration";

        /// <summary>
        /// Exception Handling Application Block section name
        /// </summary>
        public const string ExceptionHandling = "exceptionHandling";

        /// <summary>
        /// Policy injection section name
        /// </summary>
        public const string PolicyInjection = "policyInjection";

        ///<summary>
        /// Validation section name
        ///</summary>
        public const string Validation = "validation";
    }
}
