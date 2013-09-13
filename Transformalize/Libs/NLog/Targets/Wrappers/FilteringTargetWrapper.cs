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

using Transformalize.Libs.NLog.Common;
using Transformalize.Libs.NLog.Conditions;
using Transformalize.Libs.NLog.Config;

namespace Transformalize.Libs.NLog.Targets.Wrappers
{
    /// <summary>
    ///     Filters log entries based on a condition.
    /// </summary>
    /// <seealso href="http://nlog-project.org/wiki/FilteringWrapper_target">Documentation on NLog Wiki</seealso>
    /// <example>
    ///     <p>This example causes the messages not contains the string '1' to be ignored.</p>
    ///     <p>
    ///         To set up the target in the <a href="config.html">configuration file</a>,
    ///         use the following syntax:
    ///     </p>
    ///     <code lang="XML" source="examples/targets/Configuration File/FilteringWrapper/NLog.config" />
    ///     <p>
    ///         The above examples assume just one target and a single rule. See below for
    ///         a programmatic configuration that's equivalent to the above config file:
    ///     </p>
    ///     <code lang="C#" source="examples/targets/Configuration API/FilteringWrapper/Simple/Example.cs" />
    /// </example>
    [Target("FilteringWrapper", IsWrapper = true)]
    public class FilteringTargetWrapper : WrapperTargetBase
    {
        private static readonly object boxedBooleanTrue = true;

        /// <summary>
        ///     Initializes a new instance of the <see cref="FilteringTargetWrapper" /> class.
        /// </summary>
        public FilteringTargetWrapper()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="FilteringTargetWrapper" /> class.
        /// </summary>
        /// <param name="wrappedTarget">The wrapped target.</param>
        /// <param name="condition">The condition.</param>
        public FilteringTargetWrapper(Target wrappedTarget, ConditionExpression condition)
        {
            WrappedTarget = wrappedTarget;
            Condition = condition;
        }

        /// <summary>
        ///     Gets or sets the condition expression. Log events who meet this condition will be forwarded
        ///     to the wrapped target.
        /// </summary>
        /// <docgen category='Filtering Options' order='10' />
        [RequiredParameter]
        public ConditionExpression Condition { get; set; }

        /// <summary>
        ///     Checks the condition against the passed log event.
        ///     If the condition is met, the log event is forwarded to
        ///     the wrapped target.
        /// </summary>
        /// <param name="logEvent">Log event.</param>
        protected override void Write(AsyncLogEventInfo logEvent)
        {
            var v = Condition.Evaluate(logEvent.LogEvent);
            if (boxedBooleanTrue.Equals(v))
            {
                WrappedTarget.WriteAsyncLogEvent(logEvent);
            }
            else
            {
                logEvent.Continuation(null);
            }
        }
    }
}