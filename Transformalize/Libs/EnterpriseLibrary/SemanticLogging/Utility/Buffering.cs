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

using System;

namespace Transformalize.Libs.EnterpriseLibrary.SemanticLogging.Utility
{
    /// <summary>
    /// Buffering constants.
    /// </summary>
    public static class Buffering
    {
        /// <summary>
        /// The default buffering count.
        /// </summary>
        public const int DefaultBufferingCount = 1000;

        /// <summary>
        /// The default buffering interval.
        /// </summary>
        public static readonly TimeSpan DefaultBufferingInterval = TimeSpan.FromSeconds(30);

        /// <summary>
        /// The maximum number of entries that can be buffered while it's sending to database before the sink starts dropping entries.
        /// </summary>
        public const int DefaultMaxBufferSize = 30000;
    }
}