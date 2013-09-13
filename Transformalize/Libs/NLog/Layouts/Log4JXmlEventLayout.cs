#region License

// /*
// Transformalize - Replicate, Transform, and Denormalize Your Data...
// Copyright (C) 2013 Dale Newman
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
// */

#endregion

using Transformalize.Libs.NLog.LayoutRenderers;

namespace Transformalize.Libs.NLog.Layouts
{
    /// <summary>
    ///     A specialized layout that renders Log4j-compatible XML events.
    /// </summary>
    /// <remarks>
    ///     This layout is not meant to be used explicitly. Instead you can use ${log4jxmlevent} layout renderer.
    /// </remarks>
    [Layout("Log4JXmlEventLayout")]
    public class Log4JXmlEventLayout : Layout
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="Log4JXmlEventLayout" /> class.
        /// </summary>
        public Log4JXmlEventLayout()
        {
            Renderer = new Log4JXmlEventLayoutRenderer();
        }

        /// <summary>
        ///     Gets the <see cref="Log4JXmlEventLayoutRenderer" /> instance that renders log events.
        /// </summary>
        public Log4JXmlEventLayoutRenderer Renderer { get; private set; }

        /// <summary>
        ///     Renders the layout for the specified logging event by invoking layout renderers.
        /// </summary>
        /// <param name="logEvent">The logging event.</param>
        /// <returns>The rendered layout.</returns>
        protected override string GetFormattedMessage(LogEventInfo logEvent)
        {
            string cachedValue;

            if (logEvent.TryGetCachedLayoutValue(this, out cachedValue))
            {
                return cachedValue;
            }

            return logEvent.AddCachedLayoutValue(this, Renderer.Render(logEvent));
        }
    }
}