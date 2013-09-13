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

using System;
using System.Collections.Generic;
using System.Threading;
using Transformalize.Libs.NLog.Common;

namespace Transformalize.Libs.NLog.Targets.Wrappers
{
    /// <summary>
    ///     Writes log events to all targets.
    /// </summary>
    /// <seealso href="http://nlog-project.org/wiki/SplitGroup_target">Documentation on NLog Wiki</seealso>
    /// <example>
    ///     <p>
    ///         This example causes the messages to be written to both file1.txt or file2.txt
    ///     </p>
    ///     <p>
    ///         To set up the target in the <a href="config.html">configuration file</a>,
    ///         use the following syntax:
    ///     </p>
    ///     <code lang="XML" source="examples/targets/Configuration File/SplitGroup/NLog.config" />
    ///     <p>
    ///         The above examples assume just one target and a single rule. See below for
    ///         a programmatic configuration that's equivalent to the above config file:
    ///     </p>
    ///     <code lang="C#" source="examples/targets/Configuration API/SplitGroup/Simple/Example.cs" />
    /// </example>
    [Target("SplitGroup", IsCompound = true)]
    public class SplitGroupTarget : CompoundTargetBase
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="SplitGroupTarget" /> class.
        /// </summary>
        public SplitGroupTarget()
            : this(new Target[0])
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="SplitGroupTarget" /> class.
        /// </summary>
        /// <param name="targets">The targets.</param>
        public SplitGroupTarget(params Target[] targets)
            : base(targets)
        {
        }

        /// <summary>
        ///     Forwards the specified log event to all sub-targets.
        /// </summary>
        /// <param name="logEvent">The log event.</param>
        protected override void Write(AsyncLogEventInfo logEvent)
        {
            AsyncHelpers.ForEachItemSequentially(Targets, logEvent.Continuation, (t, cont) => t.WriteAsyncLogEvent(logEvent.LogEvent.WithContinuation(cont)));
        }

        /// <summary>
        ///     Writes an array of logging events to the log target. By default it iterates on all
        ///     events and passes them to "Write" method. Inheriting classes can use this method to
        ///     optimize batch writes.
        /// </summary>
        /// <param name="logEvents">Logging events to be written out.</param>
        protected override void Write(AsyncLogEventInfo[] logEvents)
        {
            InternalLogger.Trace("Writing {0} events", logEvents.Length);

            for (var i = 0; i < logEvents.Length; ++i)
            {
                logEvents[i].Continuation = CountedWrap(logEvents[i].Continuation, Targets.Count);
            }

            foreach (var t in Targets)
            {
                InternalLogger.Trace("Sending {0} events to {1}", logEvents.Length, t);
                t.WriteAsyncLogEvents(logEvents);
            }
        }

        private static AsyncContinuation CountedWrap(AsyncContinuation originalContinuation, int counter)
        {
            if (counter == 1)
            {
                return originalContinuation;
            }

            var exceptions = new List<Exception>();

            AsyncContinuation wrapper =
                ex =>
                    {
                        if (ex != null)
                        {
                            lock (exceptions)
                            {
                                exceptions.Add(ex);
                            }
                        }

                        var c = Interlocked.Decrement(ref counter);

                        if (c == 0)
                        {
                            var combinedException = AsyncHelpers.GetCombinedException(exceptions);
                            InternalLogger.Trace("Combined exception: {0}", combinedException);
                            originalContinuation(combinedException);
                        }
                        else
                        {
                            InternalLogger.Trace("{0} remaining.", c);
                        }
                    };

            return wrapper;
        }
    }
}