#region License
// /*
// See license included in this library folder.
// */
#endregion

using System.Diagnostics;

#if !NET_CF

namespace Transformalize.Libs.NLog.Targets
{
    /// <summary>
    ///     Writes log messages to the attached managed debugger.
    /// </summary>
    /// <example>
    ///     <p>
    ///         To set up the target in the <a href="config.html">configuration file</a>,
    ///         use the following syntax:
    ///     </p>
    ///     <code lang="XML" source="examples/targets/Configuration File/Debugger/NLog.config" />
    ///     <p>
    ///         This assumes just one target and a single rule. More configuration
    ///         options are described <a href="config.html">here</a>.
    ///     </p>
    ///     <p>
    ///         To set up the log target programmatically use code like this:
    ///     </p>
    ///     <code lang="C#" source="examples/targets/Configuration API/Debugger/Simple/Example.cs" />
    /// </example>
    [Target("Debugger")]
    public sealed class DebuggerTarget : TargetWithLayoutHeaderAndFooter
    {
        /// <summary>
        ///     Initializes the target.
        /// </summary>
        protected override void InitializeTarget()
        {
            base.InitializeTarget();
            if (Header != null)
            {
                Debugger.Log(LogLevel.Off.Ordinal, string.Empty, Header.Render(LogEventInfo.CreateNullEvent()) + "\n");
            }
        }

        /// <summary>
        ///     Closes the target and releases any unmanaged resources.
        /// </summary>
        protected override void CloseTarget()
        {
            if (Footer != null)
            {
                Debugger.Log(LogLevel.Off.Ordinal, string.Empty, Footer.Render(LogEventInfo.CreateNullEvent()) + "\n");
            }

            base.CloseTarget();
        }

        /// <summary>
        ///     Writes the specified logging event to the attached debugger.
        /// </summary>
        /// <param name="logEvent">The logging event.</param>
        protected override void Write(LogEventInfo logEvent)
        {
            if (Debugger.IsLogging())
            {
                Debugger.Log(logEvent.Level.Ordinal, logEvent.LoggerName, Layout.Render(logEvent) + "\n");
            }
        }
    }
}

#endif