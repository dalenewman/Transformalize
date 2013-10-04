#region License
// /*
// See license included in this library folder.
// */
#endregion

using Transformalize.Libs.NLog.Common;

namespace Transformalize.Libs.NLog.Targets.Wrappers
{
    /// <summary>
    ///     Distributes log events to targets in a round-robin fashion.
    /// </summary>
    /// <seealso href="http://nlog-project.org/wiki/RoundRobinGroup_target">Documentation on NLog Wiki</seealso>
    /// <example>
    ///     <p>
    ///         This example causes the messages to be written to either file1.txt or file2.txt.
    ///         Each odd message is written to file2.txt, each even message goes to file1.txt.
    ///     </p>
    ///     <p>
    ///         To set up the target in the <a href="config.html">configuration file</a>,
    ///         use the following syntax:
    ///     </p>
    ///     <code lang="XML" source="examples/targets/Configuration File/RoundRobinGroup/NLog.config" />
    ///     <p>
    ///         The above examples assume just one target and a single rule. See below for
    ///         a programmatic configuration that's equivalent to the above config file:
    ///     </p>
    ///     <code lang="C#" source="examples/targets/Configuration API/RoundRobinGroup/Simple/Example.cs" />
    /// </example>
    [Target("RoundRobinGroup", IsCompound = true)]
    public class RoundRobinGroupTarget : CompoundTargetBase
    {
        private readonly object lockObject = new object();
        private int currentTarget;

        /// <summary>
        ///     Initializes a new instance of the <see cref="RoundRobinGroupTarget" /> class.
        /// </summary>
        public RoundRobinGroupTarget()
            : this(new Target[0])
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="RoundRobinGroupTarget" /> class.
        /// </summary>
        /// <param name="targets">The targets.</param>
        public RoundRobinGroupTarget(params Target[] targets)
            : base(targets)
        {
        }

        /// <summary>
        ///     Forwards the write to one of the targets from
        ///     the <see cref="NLog.Targets" /> collection.
        /// </summary>
        /// <param name="logEvent">The log event.</param>
        /// <remarks>
        ///     The writes are routed in a round-robin fashion.
        ///     The first log event goes to the first target, the second
        ///     one goes to the second target and so on looping to the
        ///     first target when there are no more targets available.
        ///     In general request N goes to Targets[N % Targets.Count].
        /// </remarks>
        protected override void Write(AsyncLogEventInfo logEvent)
        {
            if (Targets.Count == 0)
            {
                logEvent.Continuation(null);
                return;
            }

            int selectedTarget;

            lock (lockObject)
            {
                selectedTarget = currentTarget;
                currentTarget = (currentTarget + 1)%Targets.Count;
            }

            Targets[selectedTarget].WriteAsyncLogEvent(logEvent);
        }
    }
}