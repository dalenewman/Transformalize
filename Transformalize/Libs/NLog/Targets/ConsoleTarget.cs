#region License
// /*
// See license included in this library folder.
// */
#endregion

using System;
using System.ComponentModel;

namespace Transformalize.Libs.NLog.Targets
{
    /// <summary>
    ///     Writes log messages to the console.
    /// </summary>
    /// <seealso href="http://nlog-project.org/wiki/Console_target">Documentation on NLog Wiki</seealso>
    /// <example>
    ///     <p>
    ///         To set up the target in the <a href="config.html">configuration file</a>,
    ///         use the following syntax:
    ///     </p>
    ///     <code lang="XML" source="examples/targets/Configuration File/Console/NLog.config" />
    ///     <p>
    ///         This assumes just one target and a single rule. More configuration
    ///         options are described <a href="config.html">here</a>.
    ///     </p>
    ///     <p>
    ///         To set up the log target programmatically use code like this:
    ///     </p>
    ///     <code lang="C#" source="examples/targets/Configuration API/Console/Simple/Example.cs" />
    /// </example>
    [Target("Console")]
    public sealed class ConsoleTarget : TargetWithLayoutHeaderAndFooter
    {
#if !NET_CF
        /// <summary>
        ///     Gets or sets a value indicating whether to send the log messages to the standard error instead of the standard output.
        /// </summary>
        /// <docgen category='Console Options' order='10' />
        [DefaultValue(false)]
        public bool Error { get; set; }
#endif

        /// <summary>
        ///     Initializes the target.
        /// </summary>
        protected override void InitializeTarget()
        {
            base.InitializeTarget();
            if (Header != null)
            {
                Output(Header.Render(LogEventInfo.CreateNullEvent()));
            }
        }

        /// <summary>
        ///     Closes the target and releases any unmanaged resources.
        /// </summary>
        protected override void CloseTarget()
        {
            if (Footer != null)
            {
                Output(Footer.Render(LogEventInfo.CreateNullEvent()));
            }

            base.CloseTarget();
        }

        /// <summary>
        ///     Writes the specified logging event to the Console.Out or
        ///     Console.Error depending on the value of the Error flag.
        /// </summary>
        /// <param name="logEvent">The logging event.</param>
        /// <remarks>
        ///     Note that the Error option is not supported on .NET Compact Framework.
        /// </remarks>
        protected override void Write(LogEventInfo logEvent)
        {
            Output(Layout.Render(logEvent));
        }

        private void Output(string s)
        {
#if !NET_CF
            if (Error)
            {
                Console.Error.WriteLine(s);
            }
            else
            {
                Console.Out.WriteLine(s);
            }
#else
            Console.WriteLine(s);
#endif
        }
    }
}