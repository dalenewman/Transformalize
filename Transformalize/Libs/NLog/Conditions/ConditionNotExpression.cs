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
    ///     Condition <b>not</b> expression.
    /// </summary>
    internal sealed class ConditionNotExpression : ConditionExpression
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ConditionNotExpression" /> class.
        /// </summary>
        /// <param name="expression">The expression.</param>
        public ConditionNotExpression(ConditionExpression expression)
        {
            Expression = expression;
        }

        /// <summary>
        ///     Gets the expression to be negated.
        /// </summary>
        /// <value>The expression.</value>
        public ConditionExpression Expression { get; private set; }

        /// <summary>
        ///     Returns a string representation of the expression.
        /// </summary>
        /// <returns>
        ///     A <see cref="T:System.String" /> that represents the condition expression.
        /// </returns>
        public override string ToString()
        {
            return "(not " + Expression + ")";
        }

        /// <summary>
        ///     Evaluates the expression.
        /// </summary>
        /// <param name="context">Evaluation context.</param>
        /// <returns>Expression result.</returns>
        protected override object EvaluateNode(LogEventInfo context)
        {
            return !(bool) Expression.Evaluate(context);
        }
    }
}