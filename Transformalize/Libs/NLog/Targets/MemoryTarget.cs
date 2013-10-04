#region License
// /*
// See license included in this library folder.
// */
#endregion

using System.Collections.Generic;

namespace Transformalize.Libs.NLog.Targets
{
    /// <summary>
    ///     Writes log messages to an ArrayList in memory for programmatic retrieval.
    /// </summary>
    /// <seealso href="http://nlog-project.org/wiki/Memory_target">Documentation on NLog Wiki</seealso>
    /// <example>
    ///     <p>
    ///         To set up the target in the <a href="config.html">configuration file</a>,
    ///         use the following syntax:
    ///     </p>
    ///     <code lang="XML" source="examples/targets/Configuration File/Memory/NLog.config" />
    ///     <p>
    ///         This assumes just one target and a single rule. More configuration
    ///         options are described <a href="config.html">here</a>.
    ///     </p>
    ///     <p>
    ///         To set up the log target programmatically use code like this:
    ///     </p>
    ///     <code lang="C#" source="examples/targets/Configuration API/Memory/Simple/Example.cs" />
    /// </example>
    [Target("Memory")]
    public sealed class MemoryTarget : TargetWithLayout
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="MemoryTarget" /> class.
        /// </summary>
        /// <remarks>
        ///     The default value of the layout is: <code>${longdate}|${level:uppercase=true}|${logger}|${message}</code>
        /// </remarks>
        public MemoryTarget()
        {
            Logs = new List<string>();
        }

        /// <summary>
        ///     Gets the list of logs gathered in the <see cref="MemoryTarget" />.
        /// </summary>
        public IList<string> Logs { get; private set; }

        /// <summary>
        ///     Renders the logging event message and adds it to the internal ArrayList of log messages.
        /// </summary>
        /// <param name="logEvent">The logging event.</param>
        protected override void Write(LogEventInfo logEvent)
        {
            var msg = Layout.Render(logEvent);

            Logs.Add(msg);
        }
    }
}