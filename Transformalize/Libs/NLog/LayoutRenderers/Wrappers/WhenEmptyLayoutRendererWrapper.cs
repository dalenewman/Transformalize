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
using Transformalize.Libs.NLog.Layouts;

namespace Transformalize.Libs.NLog.LayoutRenderers.Wrappers
{
    /// <summary>
    ///     Outputs alternative layout when the inner layout produces empty result.
    /// </summary>
    [LayoutRenderer("whenEmpty")]
    [AmbientProperty("WhenEmpty")]
    [ThreadAgnostic]
    public sealed class WhenEmptyLayoutRendererWrapper : WrapperLayoutRendererBase
    {
        /// <summary>
        ///     Gets or sets the layout to be rendered when original layout produced empty result.
        /// </summary>
        /// <docgen category="Transformation Options" order="10" />
        [RequiredParameter]
        public Layout WhenEmpty { get; set; }

        /// <summary>
        ///     Transforms the output of another layout.
        /// </summary>
        /// <param name="text">Output to be transform.</param>
        /// <returns>Transformed text.</returns>
        protected override string Transform(string text)
        {
            return text;
        }

        /// <summary>
        ///     Renders the inner layout contents.
        /// </summary>
        /// <param name="logEvent">The log event.</param>
        /// <returns>
        ///     Contents of inner layout.
        /// </returns>
        protected override string RenderInner(LogEventInfo logEvent)
        {
            var inner = base.RenderInner(logEvent);
            if (!string.IsNullOrEmpty(inner))
            {
                return inner;
            }

            // render WhenEmpty when the inner layout was empty
            return WhenEmpty.Render(logEvent);
        }
    }
}