#region License
// /*
// See license included in this library folder.
// */
#endregion

using System.Runtime.InteropServices;
using Transformalize.Libs.NLog.Internal;

#if !NET_CF && !SILVERLIGHT

namespace Transformalize.Libs.NLog.Targets
{
    /// <summary>
    ///     Outputs log messages through the ASP Response object.
    /// </summary>
    /// <seealso href="http://nlog-project.org/wiki/AspResponse_target">Documentation on NLog Wiki</seealso>
    [Target("AspResponse")]
    public sealed class AspResponseTarget : TargetWithLayout
    {
        /// <summary>
        ///     Gets or sets a value indicating whether to add &lt;!-- --&gt; comments around all written texts.
        /// </summary>
        /// <docgen category='Layout Options' order='100' />
        public bool AddComments { get; set; }

        /// <summary>
        ///     Outputs the rendered logging event through the <c>OutputDebugString()</c> Win32 API.
        /// </summary>
        /// <param name="logEvent">The logging event.</param>
        protected override void Write(LogEventInfo logEvent)
        {
            var response = AspHelper.GetResponseObject();
            if (response != null)
            {
                if (AddComments)
                {
                    response.Write("<!-- " + Layout.Render(logEvent) + "-->");
                }
                else
                {
                    response.Write(Layout.Render(logEvent));
                }

                Marshal.ReleaseComObject(response);
            }
        }
    }
}

#endif