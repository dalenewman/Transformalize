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

using System.ComponentModel;
using Transformalize.Libs.NLog.Common;

namespace Transformalize.Libs.NLog.Targets.Wrappers
{
    /// <summary>
    ///     Repeats each log event the specified number of times.
    /// </summary>
    /// <seealso href="http://nlog-project.org/wiki/RepeatingWrapper_target">Documentation on NLog Wiki</seealso>
    /// <example>
    ///     <p>This example causes each log message to be repeated 3 times.</p>
    ///     <p>
    ///         To set up the target in the <a href="config.html">configuration file</a>,
    ///         use the following syntax:
    ///     </p>
    ///     <code lang="XML" source="examples/targets/Configuration File/RepeatingWrapper/NLog.config" />
    ///     <p>
    ///         The above examples assume just one target and a single rule. See below for
    ///         a programmatic configuration that's equivalent to the above config file:
    ///     </p>
    ///     <code lang="C#" source="examples/targets/Configuration API/RepeatingWrapper/Simple/Example.cs" />
    /// </example>
    [Target("RepeatingWrapper", IsWrapper = true)]
    public class RepeatingTargetWrapper : WrapperTargetBase
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="RepeatingTargetWrapper" /> class.
        /// </summary>
        public RepeatingTargetWrapper()
            : this(null, 3)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="RepeatingTargetWrapper" /> class.
        /// </summary>
        /// <param name="wrappedTarget">The wrapped target.</param>
        /// <param name="repeatCount">The repeat count.</param>
        public RepeatingTargetWrapper(Target wrappedTarget, int repeatCount)
        {
            WrappedTarget = wrappedTarget;
            RepeatCount = repeatCount;
        }

        /// <summary>
        ///     Gets or sets the number of times to repeat each log message.
        /// </summary>
        /// <docgen category='Repeating Options' order='10' />
        [DefaultValue(3)]
        public int RepeatCount { get; set; }

        /// <summary>
        ///     Forwards the log message to the <see cref="WrapperTargetBase.WrappedTarget" /> by calling the
        ///     <see
        ///         cref="Target.Write(LogEventInfo)" />
        ///     method <see cref="RepeatCount" /> times.
        /// </summary>
        /// <param name="logEvent">The log event.</param>
        protected override void Write(AsyncLogEventInfo logEvent)
        {
            AsyncHelpers.Repeat(RepeatCount, logEvent.Continuation, cont => WrappedTarget.WriteAsyncLogEvent(logEvent.LogEvent.WithContinuation(cont)));
        }
    }
}