#region License
// /*
// See license included in this library folder.
// */
#endregion
#define TRACE

using System.Diagnostics;

#if !NET_CF && !SILVERLIGHT

namespace Transformalize.Libs.NLog.Targets
{
    /// <summary>
    ///     Sends log messages through System.Diagnostics.Trace.
    /// </summary>
    /// <seealso href="http://nlog-project.org/wiki/Trace_target">Documentation on NLog Wiki</seealso>
    /// <example>
    ///     <p>
    ///         To set up the target in the <a href="config.html">configuration file</a>,
    ///         use the following syntax:
    ///     </p>
    ///     <code lang="XML" source="examples/targets/Configuration File/Trace/NLog.config" />
    ///     <p>
    ///         This assumes just one target and a single rule. More configuration
    ///         options are described <a href="config.html">here</a>.
    ///     </p>
    ///     <p>
    ///         To set up the log target programmatically use code like this:
    ///     </p>
    ///     <code lang="C#" source="examples/targets/Configuration API/Trace/Simple/Example.cs" />
    /// </example>
    [Target("Trace")]
    public sealed class TraceTarget : TargetWithLayout
    {
        /// <summary>
        ///     Writes the specified logging event to the <see cref="System.Diagnostics.Trace" /> facility.
        ///     If the log level is greater than or equal to <see cref="LogLevel.Error" /> it uses the
        ///     <see cref="System.Diagnostics.Trace.Fail(string)" /> method, otherwise it uses
        ///     <see cref="System.Diagnostics.Trace.Write(string)" /> method.
        /// </summary>
        /// <param name="logEvent">The logging event.</param>
        protected override void Write(LogEventInfo logEvent)
        {
            if (logEvent.Level >= LogLevel.Error)
            {
                Trace.Fail(Layout.Render(logEvent));
            }
            else
            {
                Trace.WriteLine(Layout.Render(logEvent));
            }
        }
    }
}

#endif