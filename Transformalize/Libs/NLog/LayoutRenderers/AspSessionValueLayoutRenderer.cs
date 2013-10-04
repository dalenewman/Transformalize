#region License
// /*
// See license included in this library folder.
// */
#endregion

using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using Transformalize.Libs.NLog.Config;
using Transformalize.Libs.NLog.Internal;

#if !NET_CF && !SILVERLIGHT

namespace Transformalize.Libs.NLog.LayoutRenderers
{
    /// <summary>
    ///     ASP Session variable.
    /// </summary>
    [LayoutRenderer("asp-session")]
    public class AspSessionValueLayoutRenderer : LayoutRenderer
    {
        /// <summary>
        ///     Gets or sets the session variable name.
        /// </summary>
        /// <docgen category='Rendering Options' order='10' />
        [RequiredParameter]
        [DefaultParameter]
        public string Variable { get; set; }

        /// <summary>
        ///     Renders the specified ASP Session variable and appends it to the specified <see cref="StringBuilder" />.
        /// </summary>
        /// <param name="builder">
        ///     The <see cref="StringBuilder" /> to append the rendered data to.
        /// </param>
        /// <param name="logEvent">Logging event.</param>
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            var session = AspHelper.GetSessionObject();
            if (session != null)
            {
                if (Variable != null)
                {
                    var variableValue = session.GetValue(Variable);
                    builder.Append(Convert.ToString(variableValue, CultureInfo.InvariantCulture));
                }

                Marshal.ReleaseComObject(session);
            }
        }
    }
}

#endif