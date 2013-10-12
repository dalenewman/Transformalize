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
using System.Globalization;
using System.Text;

namespace Transformalize.Libs.EnterpriseLibrary.Common.Instrumentation
{
    /// <summary>
    /// Formats an event log entry to the defined format.
    /// </summary>
    public class EventLogEntryFormatter : IEventLogEntryFormatter
    {
        private string applicationName;
        private string blockName;

        private static readonly string[] emptyExtraInformation = new string[0];

        /// <overloads>
        /// Initializes this object with the specified information.
        /// </overloads>
        /// <summary>
        /// Initializes this object with the name of the specific block using this class.
        /// </summary>
        /// <param name="blockName">Name of block using this functionality.</param>
        public EventLogEntryFormatter(string blockName)
            : this(GetApplicationName(), blockName)
        {
        }

        /// <summary>
        /// Initializes this object	with the given application and block names.
        /// </summary>
        /// <param name="applicationName">Name of the application.</param>
        /// <param name="blockName">Name of the block using this functionality.</param>
        public EventLogEntryFormatter(string applicationName, string blockName)
        {
            this.applicationName = applicationName;
            this.blockName = blockName;
        }

        /// <overloads>
        /// Creates a formatted message, suitable for logging to the event log.
        /// </overloads>
        /// <summary>
        /// Creates a formatted message, suitable for logging to the event log.
        /// </summary>
        /// <param name="message">Message to be formatted, with format tags embedded.</param>
        /// <param name="extraInformation">Extra strings to be matched up with the format tags provided in <paramref name="message"></paramref>.</param>
        /// <returns>Formatted message, suitable for logging to the event log.</returns>
        public string GetEntryText(string message, params string[] extraInformation)
        {
            return BuildEntryText(message, null, extraInformation);
        }

        /// <summary>
        /// Creates a formatted message, suitable for logging to the event log.
        /// </summary>
        /// <param name="message">Message to be formatted, with format tags embedded.</param>
        /// <param name="exception">Exception containing message text to be added to event log message produced by this method</param>
        /// <param name="extraInformation">Extra strings to be matched up with the format tags provided in <paramref name="message"></paramref>.</param>
        /// <returns>Formatted message, suitable for logging to the event log.</returns>
        public string GetEntryText(string message, Exception exception, params string[] extraInformation)
        {
            return BuildEntryText(message, exception, extraInformation);
        }

        private string BuildEntryText(string message, Exception exception, string[] extraInformation)
        {
            // add header
            StringBuilder entryTextBuilder
                = new StringBuilder(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        Resources.EventLogEntryHeaderTemplate,
                        applicationName,
                        blockName));
            entryTextBuilder.AppendLine();

            // add message
            entryTextBuilder.AppendLine(message);

            //add extra info
            for (int i = 0; i < extraInformation.Length; i++)
            {
                entryTextBuilder.AppendLine(extraInformation[i]);
            }

            // add exception
            if (exception != null)
            {
                entryTextBuilder.AppendLine(
                   string.Format(
                       CultureInfo.CurrentCulture,
                       Resources.EventLogEntryExceptionTemplate,
                       exception.ToString()));
            }

            return entryTextBuilder.ToString();
        }

        private static string GetApplicationName()
        {
            return AppDomain.CurrentDomain.FriendlyName;
        }

        private string EntryTemplate
        {
            get
            {
                return "";
            }
        }
    }
}
