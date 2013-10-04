#region License
// /*
// See license included in this library folder.
// */
#endregion
namespace Transformalize.Libs.NLog.Conditions
{
    /// <summary>
    ///     Condition level expression (represented by the <b>level</b> keyword).
    /// </summary>
    internal sealed class ConditionLevelExpression : ConditionExpression
    {
        /// <summary>
        ///     Returns a string representation of the expression.
        /// </summary>
        /// <returns>
        ///     The '<b>level</b>' string.
        /// </returns>
        public override string ToString()
        {
            return "level";
        }

        /// <summary>
        ///     Evaluates to the current log level.
        /// </summary>
        /// <param name="context">Evaluation context. Ignored.</param>
        /// <returns>
        ///     The <see cref="LogLevel" /> object representing current log level.
        /// </returns>
        protected override object EvaluateNode(LogEventInfo context)
        {
            return context.Level;
        }
    }
}