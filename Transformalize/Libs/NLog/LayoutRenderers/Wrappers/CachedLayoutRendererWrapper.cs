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

using System.ComponentModel;
using Transformalize.Libs.NLog.Config;

namespace Transformalize.Libs.NLog.LayoutRenderers.Wrappers
{
    /// <summary>
    ///     Applies caching to another layout output.
    /// </summary>
    /// <remarks>
    ///     The value of the inner layout will be rendered only once and reused subsequently.
    /// </remarks>
    [LayoutRenderer("cached")]
    [AmbientProperty("Cached")]
    [ThreadAgnostic]
    public sealed class CachedLayoutRendererWrapper : WrapperLayoutRendererBase
    {
        private string cachedValue;

        /// <summary>
        ///     Initializes a new instance of the <see cref="CachedLayoutRendererWrapper" /> class.
        /// </summary>
        public CachedLayoutRendererWrapper()
        {
            Cached = true;
        }

        /// <summary>
        ///     Gets or sets a value indicating whether this <see cref="CachedLayoutRendererWrapper" /> is enabled.
        /// </summary>
        /// <docgen category='Caching Options' order='10' />
        [DefaultValue(true)]
        public bool Cached { get; set; }

        /// <summary>
        ///     Initializes the layout renderer.
        /// </summary>
        protected override void InitializeLayoutRenderer()
        {
            base.InitializeLayoutRenderer();
            cachedValue = null;
        }

        /// <summary>
        ///     Closes the layout renderer.
        /// </summary>
        protected override void CloseLayoutRenderer()
        {
            base.CloseLayoutRenderer();
            cachedValue = null;
        }

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
        /// <returns>Contents of inner layout.</returns>
        protected override string RenderInner(LogEventInfo logEvent)
        {
            if (Cached)
            {
                if (cachedValue == null)
                {
                    cachedValue = base.RenderInner(logEvent);
                }

                return cachedValue;
            }
            else
            {
                return base.RenderInner(logEvent);
            }
        }
    }
}