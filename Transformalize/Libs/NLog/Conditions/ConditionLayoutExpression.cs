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

using Transformalize.Libs.NLog.Layouts;

namespace Transformalize.Libs.NLog.Conditions
{
    /// <summary>
    ///     Condition layout expression (represented by a string literal
    ///     with embedded ${}).
    /// </summary>
    internal sealed class ConditionLayoutExpression : ConditionExpression
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ConditionLayoutExpression" /> class.
        /// </summary>
        /// <param name="layout">The layout.</param>
        public ConditionLayoutExpression(Layout layout)
        {
            Layout = layout;
        }

        /// <summary>
        ///     Gets the layout.
        /// </summary>
        /// <value>The layout.</value>
        public Layout Layout { get; private set; }

        /// <summary>
        ///     Returns a string representation of this expression.
        /// </summary>
        /// <returns>String literal in single quotes.</returns>
        public override string ToString()
        {
            return Layout.ToString();
        }

        /// <summary>
        ///     Evaluates the expression by calculating the value
        ///     of the layout in the specified evaluation context.
        /// </summary>
        /// <param name="context">Evaluation context.</param>
        /// <returns>The value of the layout.</returns>
        protected override object EvaluateNode(LogEventInfo context)
        {
            return Layout.Render(context);
        }
    }
}