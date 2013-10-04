#region License
// /*
// See license included in this library folder.
// */
#endregion
namespace Transformalize.Libs.NLog.Conditions
{
    /// <summary>
    ///     Condition <b>and</b> expression.
    /// </summary>
    internal sealed class ConditionAndExpression : ConditionExpression
    {
        private static readonly object boxedFalse = false;
        private static readonly object boxedTrue = true;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ConditionAndExpression" /> class.
        /// </summary>
        /// <param name="left">Left hand side of the AND expression.</param>
        /// <param name="right">Right hand side of the AND expression.</param>
        public ConditionAndExpression(ConditionExpression left, ConditionExpression right)
        {
            Left = left;
            Right = right;
        }

        /// <summary>
        ///     Gets the left hand side of the AND expression.
        /// </summary>
        public ConditionExpression Left { get; private set; }

        /// <summary>
        ///     Gets the right hand side of the AND expression.
        /// </summary>
        public ConditionExpression Right { get; private set; }

        /// <summary>
        ///     Returns a string representation of this expression.
        /// </summary>
        /// <returns>A concatenated '(Left) and (Right)' string.</returns>
        public override string ToString()
        {
            return "(" + Left + " and " + Right + ")";
        }

        /// <summary>
        ///     Evaluates the expression by evaluating <see cref="Left" /> and <see cref="Right" /> recursively.
        /// </summary>
        /// <param name="context">Evaluation context.</param>
        /// <returns>The value of the conjunction operator.</returns>
        protected override object EvaluateNode(LogEventInfo context)
        {
            var bval1 = (bool) Left.Evaluate(context);
            if (!bval1)
            {
                return boxedFalse;
            }

            var bval2 = (bool) Right.Evaluate(context);
            if (!bval2)
            {
                return boxedFalse;
            }

            return boxedTrue;
        }
    }
}