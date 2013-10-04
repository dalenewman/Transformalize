#region License
// /*
// See license included in this library folder.
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