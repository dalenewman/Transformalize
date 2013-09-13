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

using Transformalize.Libs.NLog.Config;

namespace Transformalize.Libs.NLog.Layouts
{
    /// <summary>
    ///     A specialized layout that supports header and footer.
    /// </summary>
    [Layout("LayoutWithHeaderAndFooter")]
    [ThreadAgnostic]
    public class LayoutWithHeaderAndFooter : Layout
    {
        /// <summary>
        ///     Gets or sets the body layout (can be repeated multiple times).
        /// </summary>
        /// <docgen category='Layout Options' order='10' />
        public Layout Layout { get; set; }

        /// <summary>
        ///     Gets or sets the header layout.
        /// </summary>
        /// <docgen category='Layout Options' order='10' />
        public Layout Header { get; set; }

        /// <summary>
        ///     Gets or sets the footer layout.
        /// </summary>
        /// <docgen category='Layout Options' order='10' />
        public Layout Footer { get; set; }

        /// <summary>
        ///     Renders the layout for the specified logging event by invoking layout renderers.
        /// </summary>
        /// <param name="logEvent">The logging event.</param>
        /// <returns>The rendered layout.</returns>
        protected override string GetFormattedMessage(LogEventInfo logEvent)
        {
            return Layout.Render(logEvent);
        }
    }
}