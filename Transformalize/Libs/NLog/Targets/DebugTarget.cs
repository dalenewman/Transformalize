#region License
// /*
// See license included in this library folder.
// */
#endregion
namespace Transformalize.Libs.NLog.Targets
{
    /// <summary>
    ///     Mock target - useful for testing.
    /// </summary>
    /// <seealso href="http://nlog-project.org/wiki/Debug_target">Documentation on NLog Wiki</seealso>
    /// <example>
    ///     <p>
    ///         To set up the target in the <a href="config.html">configuration file</a>,
    ///         use the following syntax:
    ///     </p>
    ///     <code lang="XML" source="examples/targets/Configuration File/Debug/NLog.config" />
    ///     <p>
    ///         This assumes just one target and a single rule. More configuration
    ///         options are described <a href="config.html">here</a>.
    ///     </p>
    ///     <p>
    ///         To set up the log target programmatically use code like this:
    ///     </p>
    ///     <code lang="C#" source="examples/targets/Configuration API/Debug/Simple/Example.cs" />
    /// </example>
    [Target("Debug")]
    public sealed class DebugTarget : TargetWithLayout
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="DebugTarget" /> class.
        /// </summary>
        /// <remarks>
        ///     The default value of the layout is: <code>${longdate}|${level:uppercase=true}|${logger}|${message}</code>
        /// </remarks>
        public DebugTarget()
        {
            LastMessage = string.Empty;
            Counter = 0;
        }

        /// <summary>
        ///     Gets the number of times this target has been called.
        /// </summary>
        /// <docgen category='Debugging Options' order='10' />
        public int Counter { get; private set; }

        /// <summary>
        ///     Gets the last message rendered by this target.
        /// </summary>
        /// <docgen category='Debugging Options' order='10' />
        public string LastMessage { get; private set; }

        /// <summary>
        ///     Increases the number of messages.
        /// </summary>
        /// <param name="logEvent">The logging event.</param>
        protected override void Write(LogEventInfo logEvent)
        {
            Counter++;
            LastMessage = Layout.Render(logEvent);
        }
    }
}