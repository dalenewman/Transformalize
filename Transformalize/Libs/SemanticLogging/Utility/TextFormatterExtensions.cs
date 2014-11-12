// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System.Globalization;
using System.IO;
using Transformalize.Libs.SemanticLogging.Formatters;

namespace Transformalize.Libs.SemanticLogging.Utility
{
    /// <summary>
    /// Extensions for <see cref="IEventTextFormatter"/>.
    /// </summary>
    public static class EventTextFormatterExtensions
    {
        /// <summary>
        /// Formats the event as a string.
        /// </summary>
        /// <param name="entry">The entry to format.</param>
        /// <param name="formatter">The formatter to use.</param>
        /// <returns>A formatted entry.</returns>
        public static string WriteEvent(this IEventTextFormatter formatter, EventEntry entry)
        {
            Guard.ArgumentNotNull(formatter, "formatter");

            using (var writer = new StringWriter(CultureInfo.CurrentCulture))
            {
                formatter.WriteEvent(entry, writer);
                return writer.ToString();
            }
        }
    }
}
