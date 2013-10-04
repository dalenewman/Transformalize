#region License
// /*
// See license included in this library folder.
// */
#endregion

using System;
using Transformalize.Libs.NLog.Common;

namespace Transformalize.Libs.NLog.Targets.Wrappers
{
    /// <summary>
    ///     Sends log messages to a randomly selected target.
    /// </summary>
    /// <seealso href="http://nlog-project.org/wiki/RandomizeGroup_target">Documentation on NLog Wiki</seealso>
    /// <example>
    ///     <p>
    ///         This example causes the messages to be written to either file1.txt or file2.txt
    ///         chosen randomly on a per-message basis.
    ///     </p>
    ///     <p>
    ///         To set up the target in the <a href="config.html">configuration file</a>,
    ///         use the following syntax:
    ///     </p>
    ///     <code lang="XML" source="examples/targets/Configuration File/RandomizeGroup/NLog.config" />
    ///     <p>
    ///         The above examples assume just one target and a single rule. See below for
    ///         a programmatic configuration that's equivalent to the above config file:
    ///     </p>
    ///     <code lang="C#" source="examples/targets/Configuration API/RandomizeGroup/Simple/Example.cs" />
    /// </example>
    [Target("RandomizeGroup", IsCompound = true)]
    public class RandomizeGroupTarget : CompoundTargetBase
    {
        private readonly Random random = new Random();

        /// <summary>
        ///     Initializes a new instance of the <see cref="RandomizeGroupTarget" /> class.
        /// </summary>
        public RandomizeGroupTarget()
            : this(new Target[0])
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="RandomizeGroupTarget" /> class.
        /// </summary>
        /// <param name="targets">The targets.</param>
        public RandomizeGroupTarget(params Target[] targets)
            : base(targets)
        {
        }

        /// <summary>
        ///     Forwards the log event to one of the sub-targets.
        ///     The sub-target is randomly chosen.
        /// </summary>
        /// <param name="logEvent">The log event.</param>
        protected override void Write(AsyncLogEventInfo logEvent)
        {
            if (Targets.Count == 0)
            {
                logEvent.Continuation(null);
                return;
            }

            int selectedTarget;

            lock (random)
            {
                selectedTarget = random.Next(Targets.Count);
            }

            Targets[selectedTarget].WriteAsyncLogEvent(logEvent);
        }
    }
}