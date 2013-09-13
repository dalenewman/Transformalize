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

using System.Text;
using Transformalize.Libs.NLog.Config;
using Transformalize.Libs.NLog.Layouts;

namespace Transformalize.Libs.NLog.LayoutRenderers.Wrappers
{
    /// <summary>
    ///     Decodes text "encrypted" with ROT-13.
    /// </summary>
    /// <remarks>
    ///     See <a href="http://en.wikipedia.org/wiki/ROT13">http://en.wikipedia.org/wiki/ROT13</a>.
    /// </remarks>
    public abstract class WrapperLayoutRendererBase : LayoutRenderer
    {
        /// <summary>
        ///     Gets or sets the wrapped layout.
        /// </summary>
        /// <docgen category='Transformation Options' order='10' />
        [DefaultParameter]
        public Layout Inner { get; set; }

        /// <summary>
        ///     Renders the inner message, processes it and appends it to the specified <see cref="StringBuilder" />.
        /// </summary>
        /// <param name="builder">
        ///     The <see cref="StringBuilder" /> to append the rendered data to.
        /// </param>
        /// <param name="logEvent">Logging event.</param>
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            var msg = RenderInner(logEvent);
            builder.Append(Transform(msg));
        }

        /// <summary>
        ///     Transforms the output of another layout.
        /// </summary>
        /// <param name="text">Output to be transform.</param>
        /// <returns>Transformed text.</returns>
        protected abstract string Transform(string text);

        /// <summary>
        ///     Renders the inner layout contents.
        /// </summary>
        /// <param name="logEvent">The log event.</param>
        /// <returns>Contents of inner layout.</returns>
        protected virtual string RenderInner(LogEventInfo logEvent)
        {
            return Inner.Render(logEvent);
        }
    }
}