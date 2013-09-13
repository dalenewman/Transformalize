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

using System.Collections.Generic;
using Transformalize.Libs.NLog.Common;
using Transformalize.Libs.NLog.Conditions;
using Transformalize.Libs.NLog.Config;

namespace Transformalize.Libs.NLog.Targets.Wrappers
{
    /// <summary>
    ///     Filters buffered log entries based on a set of conditions that are evaluated on a group of events.
    /// </summary>
    /// <seealso href="http://nlog-project.org/wiki/PostFilteringWrapper_target">Documentation on NLog Wiki</seealso>
    /// <remarks>
    ///     PostFilteringWrapper must be used with some type of buffering target or wrapper, such as
    ///     AsyncTargetWrapper, BufferingWrapper or ASPNetBufferingWrapper.
    /// </remarks>
    /// <example>
    ///     <p>
    ///         This example works like this. If there are no Warn,Error or Fatal messages in the buffer
    ///         only Info messages are written to the file, but if there are any warnings or errors,
    ///         the output includes detailed trace (levels &gt;= Debug). You can plug in a different type
    ///         of buffering wrapper (such as ASPNetBufferingWrapper) to achieve different
    ///         functionality.
    ///     </p>
    ///     <p>
    ///         To set up the target in the <a href="config.html">configuration file</a>,
    ///         use the following syntax:
    ///     </p>
    ///     <code lang="XML" source="examples/targets/Configuration File/PostFilteringWrapper/NLog.config" />
    ///     <p>
    ///         The above examples assume just one target and a single rule. See below for
    ///         a programmatic configuration that's equivalent to the above config file:
    ///     </p>
    ///     <code lang="C#" source="examples/targets/Configuration API/PostFilteringWrapper/Simple/Example.cs" />
    /// </example>
    [Target("PostFilteringWrapper", IsWrapper = true)]
    public class PostFilteringTargetWrapper : WrapperTargetBase
    {
        private static readonly object boxedTrue = true;

        /// <summary>
        ///     Initializes a new instance of the <see cref="PostFilteringTargetWrapper" /> class.
        /// </summary>
        public PostFilteringTargetWrapper()
        {
            Rules = new List<FilteringRule>();
        }

        /// <summary>
        ///     Gets or sets the default filter to be applied when no specific rule matches.
        /// </summary>
        /// <docgen category='Filtering Options' order='10' />
        public ConditionExpression DefaultFilter { get; set; }

        /// <summary>
        ///     Gets the collection of filtering rules. The rules are processed top-down
        ///     and the first rule that matches determines the filtering condition to
        ///     be applied to log events.
        /// </summary>
        /// <docgen category='Filtering Rules' order='10' />
        [ArrayParameter(typeof (FilteringRule), "when")]
        public IList<FilteringRule> Rules { get; private set; }

        /// <summary>
        ///     Evaluates all filtering rules to find the first one that matches.
        ///     The matching rule determines the filtering condition to be applied
        ///     to all items in a buffer. If no condition matches, default filter
        ///     is applied to the array of log events.
        /// </summary>
        /// <param name="logEvents">Array of log events to be post-filtered.</param>
        protected override void Write(AsyncLogEventInfo[] logEvents)
        {
            ConditionExpression resultFilter = null;

            InternalLogger.Trace("Running {0} on {1} events", this, logEvents.Length);

            // evaluate all the rules to get the filtering condition
            for (var i = 0; i < logEvents.Length; ++i)
            {
                foreach (var rule in Rules)
                {
                    var v = rule.Exists.Evaluate(logEvents[i].LogEvent);

                    if (boxedTrue.Equals(v))
                    {
                        InternalLogger.Trace("Rule matched: {0}", rule.Exists);

                        resultFilter = rule.Filter;
                        break;
                    }
                }

                if (resultFilter != null)
                {
                    break;
                }
            }

            if (resultFilter == null)
            {
                resultFilter = DefaultFilter;
            }

            if (resultFilter == null)
            {
                WrappedTarget.WriteAsyncLogEvents(logEvents);
            }
            else
            {
                InternalLogger.Trace("Filter to apply: {0}", resultFilter);

                // apply the condition to the buffer
                var resultBuffer = new List<AsyncLogEventInfo>();

                for (var i = 0; i < logEvents.Length; ++i)
                {
                    var v = resultFilter.Evaluate(logEvents[i].LogEvent);
                    if (boxedTrue.Equals(v))
                    {
                        resultBuffer.Add(logEvents[i]);
                    }
                    else
                    {
                        // anything not passed down will be notified about successful completion
                        logEvents[i].Continuation(null);
                    }
                }

                InternalLogger.Trace("After filtering: {0} events.", resultBuffer.Count);
                if (resultBuffer.Count > 0)
                {
                    InternalLogger.Trace("Sending to {0}", WrappedTarget);
                    WrappedTarget.WriteAsyncLogEvents(resultBuffer.ToArray());
                }
            }
        }
    }
}