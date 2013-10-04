#region License
// /*
// See license included in this library folder.
// */
#endregion

using System.ComponentModel;

namespace Transformalize.Libs.NLog.Targets
{
    /// <summary>
    ///     Discards log messages. Used mainly for debugging and benchmarking.
    /// </summary>
    /// <seealso href="http://nlog-project.org/wiki/Null_target">Documentation on NLog Wiki</seealso>
    /// <example>
    ///     <p>
    ///         To set up the target in the <a href="config.html">configuration file</a>,
    ///         use the following syntax:
    ///     </p>
    ///     <code lang="XML" source="examples/targets/Configuration File/Null/NLog.config" />
    ///     <p>
    ///         This assumes just one target and a single rule. More configuration
    ///         options are described <a href="config.html">here</a>.
    ///     </p>
    ///     <p>
    ///         To set up the log target programmatically use code like this:
    ///     </p>
    ///     <code lang="C#" source="examples/targets/Configuration API/Null/Simple/Example.cs" />
    /// </example>
    [Target("Null")]
    public sealed class NullTarget : TargetWithLayout
    {
        /// <summary>
        ///     Gets or sets a value indicating whether to perform layout calculation.
        /// </summary>
        /// <docgen category='Layout Options' order='10' />
        [DefaultValue(false)]
        public bool FormatMessage { get; set; }

        /// <summary>
        ///     Does nothing. Optionally it calculates the layout text but
        ///     discards the results.
        /// </summary>
        /// <param name="logEvent">The logging event.</param>
        protected override void Write(LogEventInfo logEvent)
        {
            if (FormatMessage)
            {
                Layout.Render(logEvent);
            }
        }
    }
}