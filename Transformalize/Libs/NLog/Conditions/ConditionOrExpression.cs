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

namespace Transformalize.Libs.NLog.Conditions
{
    /// <summary>
    ///     Condition <b>or</b> expression.
    /// </summary>
    internal sealed class ConditionOrExpression : ConditionExpression
    {
        private static readonly object boxedFalse = false;
        private static readonly object boxedTrue = true;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ConditionOrExpression" /> class.
        /// </summary>
        /// <param name="left">Left hand side of the OR expression.</param>
        /// <param name="right">Right hand side of the OR expression.</param>
        public ConditionOrExpression(ConditionExpression left, ConditionExpression right)
        {
            LeftExpression = left;
            RightExpression = right;
        }

        /// <summary>
        ///     Gets the left expression.
        /// </summary>
        /// <value>The left expression.</value>
        public ConditionExpression LeftExpression { get; private set; }

        /// <summary>
        ///     Gets the right expression.
        /// </summary>
        /// <value>The right expression.</value>
        public ConditionExpression RightExpression { get; private set; }

        /// <summary>
        ///     Returns a string representation of the expression.
        /// </summary>
        /// <returns>
        ///     A <see cref="T:System.String" /> that represents the condition expression.
        /// </returns>
        public override string ToString()
        {
            return "(" + LeftExpression + " or " + RightExpression + ")";
        }

        /// <summary>
        ///     Evaluates the expression by evaluating <see cref="LeftExpression" /> and <see cref="RightExpression" /> recursively.
        /// </summary>
        /// <param name="context">Evaluation context.</param>
        /// <returns>The value of the alternative operator.</returns>
        protected override object EvaluateNode(LogEventInfo context)
        {
            var bval1 = (bool) LeftExpression.Evaluate(context);
            if (bval1)
            {
                return boxedTrue;
            }

            var bval2 = (bool) RightExpression.Evaluate(context);
            if (bval2)
            {
                return boxedTrue;
            }

            return boxedFalse;
        }
    }
}