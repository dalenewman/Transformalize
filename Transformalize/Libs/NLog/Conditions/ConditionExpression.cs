#region License
// /*
// See license included in this library folder.
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