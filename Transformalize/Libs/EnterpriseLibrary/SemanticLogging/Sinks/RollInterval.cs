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
    /// Defines the frequency when the file need to be rolled.
    /// </summary>
    public enum RollInterval
    {
        /// <summary>
        /// None Interval.
        /// </summary>
        None,

        /// <summary>
        /// Minute Interval.
        /// </summary>
        Minute,

        /// <summary>
        /// Hour interval.
        /// </summary>
        Hour,

        /// <summary>
        /// Day Interval.
        /// </summary>
        Day,

        /// <summary>
        /// Week Interval.
        /// </summary>
        Week,

        /// <summary>
        /// Month Interval.
        /// </summary>
        Month,

        /// <summary>
        /// Year Interval.
        /// </summary>
        Year,

        /// <summary>
        /// At Midnight.
        /// </summary>
        Midnight
    }
}
