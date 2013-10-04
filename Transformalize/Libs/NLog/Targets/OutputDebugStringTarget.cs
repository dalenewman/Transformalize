#region License
// /*
// See license included in this library folder.
// */
#endregion

using Transformalize.Libs.NLog.Internal;

#if !NET_CF && !SILVERLIGHT

namespace Transformalize.Libs.NLog.Targets
{
    /// <summary>
    ///     Outputs log messages through the <c>OutputDebugString()</c> Win32 API.
    /// </summary>
    /// <seealso href="http://nlog-project.org/wiki/OutputDebugString_target">Documentation on NLog Wiki</seealso>
    /// <example>
    ///     <p>
    ///         To set up the target in the <a href="config.html">configuration file</a>,
    ///         use the following syntax:
    ///     </p>
    ///     <code lang="XML" source="examples/targets/Configuration File/OutputDebugString/NLog.config" />
    ///     <p>
    ///         This assumes just one target and a single rule. More configuration
    ///         options are described <a href="config.html">here</a>.
    ///     </p>
    ///     <p>
    ///         To set up the log target programmatically use code like this:
    ///     </p>
    ///     <code lang="C#" source="examples/targets/Configuration API/OutputDebugString/Simple/Example.cs" />
    /// </example>
    [Target("OutputDebugString")]
    public sealed class OutputDebugStringTarget : TargetWithLayout
    {
        /// <summary>
        ///     Outputs the rendered logging event through the <c>OutputDebugString()</c> Win32 API.
        /// </summary>
        /// <param name="logEvent">The logging event.</param>
        protected override void Write(LogEventInfo logEvent)
        {
            NativeMethods.OutputDebugString(Layout.Render(logEvent));
        }
    }
}

#endif