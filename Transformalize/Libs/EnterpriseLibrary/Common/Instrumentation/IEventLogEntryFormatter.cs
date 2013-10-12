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

namespace Transformalize.Libs.EnterpriseLibrary.Common.Instrumentation
{
    /// <summary>
    /// Formats an event log entry for logging to event log.
    /// </summary>
    public interface IEventLogEntryFormatter
    {
        /// <overloads>
        /// Creates a formatted message, suitable for logging to the event log.
        /// </overloads>
        /// <summary>
        /// Creates a formatted message, suitable for logging to the event log.
        /// </summary>
        /// <param name="message">Message to be formatted, with format tags embedded.</param>
        /// <param name="extraInformation">Extra strings to be matched up with the format tags provided in <paramref name="message"></paramref>.</param>
        /// <returns>Formatted message, suitable for logging to the event log.</returns>
        string GetEntryText(string message, params string[] extraInformation);

        /// <summary>
        /// Creates a formatted message, suitable for logging to the event log.
        /// </summary>
        /// <param name="message">Message to be formatted, with format tags embedded.</param>
        /// <param name="exception">Exception containing message text to be added to event log message produced by this method</param>
        /// <param name="extraInformation">Extra strings to be matched up with the format tags provided in <paramref name="message"></paramref>.</param>
        /// <returns>Formatted message, suitable for logging to the event log.</returns>
        string GetEntryText(string message, Exception exception, params string[] extraInformation);
    }
}
