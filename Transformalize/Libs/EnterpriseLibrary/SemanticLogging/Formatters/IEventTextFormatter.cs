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

using System.IO;

namespace Transformalize.Libs.EnterpriseLibrary.SemanticLogging.Formatters
{
    /// <summary>
    /// Provides a generic interface for an event text formatter used 
    /// whenever an event has been written by an event source for which the event listener has enabled events.
    /// </summary>
    public interface IEventTextFormatter
    {
        /// <summary>
        /// Writes the event.
        /// </summary>
        /// <param name="eventEntry">The event entry.</param>
        /// <param name="writer">The writer.</param>
        void WriteEvent(EventEntry eventEntry, TextWriter writer);
    }
}
