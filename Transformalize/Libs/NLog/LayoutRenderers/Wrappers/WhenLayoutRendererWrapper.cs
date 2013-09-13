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

using Transformalize.Libs.NLog.Conditions;
using Transformalize.Libs.NLog.Config;

namespace Transformalize.Libs.NLog.LayoutRenderers.Wrappers
{
    /// <summary>
    ///     Only outputs the inner layout when the specified condition has been met.
    /// </summary>
    [LayoutRenderer("when")]
    [AmbientProperty("When")]
    [ThreadAgnostic]
    public sealed class WhenLayoutRendererWrapper : WrapperLayoutRendererBase
    {
        /// <summary>
        ///     Gets or sets the condition that must be met for the inner layout to be printed.
        /// </summary>
        /// <docgen category="Transformation Options" order="10" />
        [RequiredParameter]
        public ConditionExpression When { get; set; }

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
            if (true.Equals(When.Evaluate(logEvent)))
            {
                return base.RenderInner(logEvent);
            }

            return string.Empty;
        }
    }
}