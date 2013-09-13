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

using System;
using Transformalize.Libs.NLog.Config;
using Transformalize.Libs.NLog.Internal;

namespace Transformalize.Libs.NLog.Conditions
{
    /// <summary>
    ///     Base class for representing nodes in condition expression trees.
    /// </summary>
    [NLogConfigurationItem]
    [ThreadAgnostic]
    public abstract class ConditionExpression
    {
        /// <summary>
        ///     Converts condition text to a condition expression tree.
        /// </summary>
        /// <param name="conditionExpressionText">Condition text to be converted.</param>
        /// <returns>Condition expression tree.</returns>
        public static implicit operator ConditionExpression(string conditionExpressionText)
        {
            return ConditionParser.ParseExpression(conditionExpressionText);
        }

        /// <summary>
        ///     Evaluates the expression.
        /// </summary>
        /// <param name="context">Evaluation context.</param>
        /// <returns>Expression result.</returns>
        public object Evaluate(LogEventInfo context)
        {
            try
            {
                return EvaluateNode(context);
            }
            catch (Exception exception)
            {
                if (exception.MustBeRethrown())
                {
                    throw;
                }

                throw new ConditionEvaluationException("Exception occurred when evaluating condition", exception);
            }
        }

        /// <summary>
        ///     Returns a string representation of the expression.
        /// </summary>
        /// <returns>
        ///     A <see cref="T:System.String" /> that represents the condition expression.
        /// </returns>
        public abstract override string ToString();

        /// <summary>
        ///     Evaluates the expression.
        /// </summary>
        /// <param name="context">Evaluation context.</param>
        /// <returns>Expression result.</returns>
        protected abstract object EvaluateNode(LogEventInfo context);
    }
}