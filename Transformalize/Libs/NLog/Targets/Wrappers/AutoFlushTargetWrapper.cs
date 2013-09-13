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

namespace Transformalize.Libs.NLog.Targets.Wrappers
{
    /// <summary>
    ///     Causes a flush after each write on a wrapped target.
    /// </summary>
    /// <seealso href="http://nlog-project.org/wiki/AutoFlushWrapper_target">Documentation on NLog Wiki</seealso>
    /// <example>
    ///     <p>
    ///         To set up the target in the <a href="config.html">configuration file</a>,
    ///         use the following syntax:
    ///     </p>
    ///     <code lang="XML" source="examples/targets/Configuration File/AutoFlushWrapper/NLog.config" />
    ///     <p>
    ///         The above examples assume just one target and a single rule. See below for
    ///         a programmatic configuration that's equivalent to the above config file:
    ///     </p>
    ///     <code lang="C#" source="examples/targets/Configuration API/AutoFlushWrapper/Simple/Example.cs" />
    /// </example>
    [Target("AutoFlushWrapper", IsWrapper = true)]
    public class AutoFlushTargetWrapper : WrapperTargetBase
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="AutoFlushTargetWrapper" /> class.
        /// </summary>
        /// <remarks>
        ///     The default value of the layout is: <code>${longdate}|${level:uppercase=true}|${logger}|${message}</code>
        /// </remarks>
        public AutoFlushTargetWrapper()
            : this(null)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="AutoFlushTargetWrapper" /> class.
        /// </summary>
        /// <param name="wrappedTarget">The wrapped target.</param>
        public AutoFlushTargetWrapper(Target wrappedTarget)
        {
            WrappedTarget = wrappedTarget;
        }

        /// <summary>
        ///     Forwards the call to the <see cref="WrapperTargetBase.WrappedTarget" />.Write()
        ///     and calls <see cref="Target.Flush(AsyncContinuation)" /> on it.
        /// </summary>
        /// <param name="logEvent">Logging event to be written out.</param>
        protected override void Write(AsyncLogEventInfo logEvent)
        {
            WrappedTarget.WriteAsyncLogEvent(logEvent.LogEvent.WithContinuation(AsyncHelpers.PrecededBy(logEvent.Continuation, WrappedTarget.Flush)));
        }
    }
}