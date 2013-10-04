#region License
// /*
// See license included in this library folder.
// */
#endregion

using Transformalize.Libs.NLog.Conditions;
using Transformalize.Libs.NLog.Config;

namespace Transformalize.Libs.NLog.Filters
{
    /// <summary>
    ///     Matches when the specified condition is met.
    /// </summary>
    /// <remarks>
    ///     Conditions are expressed using a simple language
    ///     described <a href="conditions.html">here</a>.
    /// </remarks>
    [Filter("when")]
    public class ConditionBasedFilter : Filter
    {
        private static readonly object boxedTrue = true;

        /// <summary>
        ///     Gets or sets the condition expression.
        /// </summary>
        /// <docgen category='Filtering Options' order='10' />
        [RequiredParameter]
        public ConditionExpression Condition { get; set; }

        /// <summary>
        ///     Checks whether log event should be logged or not.
        /// </summary>
        /// <param name="logEvent">Log event.</param>
        /// <returns>
        ///     <see cref="FilterResult.Ignore" /> - if the log event should be ignored<br />
        ///     <see cref="FilterResult.Neutral" /> - if the filter doesn't want to decide<br />
        ///     <see cref="FilterResult.Log" /> - if the log event should be logged<br />
        ///     .
        /// </returns>
        protected override FilterResult Check(LogEventInfo logEvent)
        {
            var val = Condition.Evaluate(logEvent);
            if (boxedTrue.Equals(val))
            {
                return Action;
            }

            return FilterResult.Neutral;
        }
    }
}