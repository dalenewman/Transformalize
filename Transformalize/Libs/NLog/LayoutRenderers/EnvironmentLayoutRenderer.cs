#region License
// /*
// See license included in this library folder.
// */
#endregion

using System.Text;
using Transformalize.Libs.NLog.Config;
using Transformalize.Libs.NLog.Internal;

#if !NET_CF && !SILVERLIGHT

namespace Transformalize.Libs.NLog.LayoutRenderers
{
    /// <summary>
    ///     The environment variable.
    /// </summary>
    [LayoutRenderer("environment")]
    public class EnvironmentLayoutRenderer : LayoutRenderer
    {
        /// <summary>
        ///     Gets or sets the name of the environment variable.
        /// </summary>
        /// <docgen category='Rendering Options' order='10' />
        [RequiredParameter]
        [DefaultParameter]
        public string Variable { get; set; }

        /// <summary>
        ///     Renders the specified environment variable and appends it to the specified <see cref="StringBuilder" />.
        /// </summary>
        /// <param name="builder">
        ///     The <see cref="StringBuilder" /> to append the rendered data to.
        /// </param>
        /// <param name="logEvent">Logging event.</param>
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            if (Variable != null)
            {
                builder.Append(EnvironmentHelper.GetSafeEnvironmentVariable(Variable));
            }
        }
    }
}

#endif