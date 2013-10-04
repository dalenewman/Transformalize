#region License
// /*
// See license included in this library folder.
// */
#endregion

using Transformalize.Libs.NLog.Config;

namespace Transformalize.Libs.NLog.Filters
{
    /// <summary>
    ///     An abstract filter class. Provides a way to eliminate log messages
    ///     based on properties other than logger name and log level.
    /// </summary>
    [NLogConfigurationItem]
    public abstract class Filter
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="Filter" /> class.
        /// </summary>
        protected Filter()
        {
            Action = FilterResult.Neutral;
        }

        /// <summary>
        ///     Gets or sets the action to be taken when filter matches.
        /// </summary>
        /// <docgen category='Filtering Options' order='10' />
        [RequiredParameter]
        public FilterResult Action { get; set; }

        /// <summary>
        ///     Gets the result of evaluating filter against given log event.
        /// </summary>
        /// <param name="logEvent">The log event.</param>
        /// <returns>Filter result.</returns>
        internal FilterResult GetFilterResult(LogEventInfo logEvent)
        {
            return Check(logEvent);
        }

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
        protected abstract FilterResult Check(LogEventInfo logEvent);
    }
}