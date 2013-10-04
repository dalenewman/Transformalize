#region License
// /*
// See license included in this library folder.
// */
#endregion

using System;
using System.Text;
using Transformalize.Libs.NLog.Common;
using Transformalize.Libs.NLog.Config;
using Transformalize.Libs.NLog.Internal;

#if !NET_CF && !SILVERLIGHT

namespace Transformalize.Libs.NLog.LayoutRenderers
{
    /// <summary>
    ///     The machine name that the process is running on.
    /// </summary>
    [LayoutRenderer("machinename")]
    [AppDomainFixedOutput]
    [ThreadAgnostic]
    public class MachineNameLayoutRenderer : LayoutRenderer
    {
        internal string MachineName { get; private set; }

        /// <summary>
        ///     Initializes the layout renderer.
        /// </summary>
        protected override void InitializeLayoutRenderer()
        {
            base.InitializeLayoutRenderer();
            try
            {
                MachineName = Environment.MachineName;
            }
            catch (Exception exception)
            {
                if (exception.MustBeRethrown())
                {
                    throw;
                }

                InternalLogger.Error("Error getting machine name {0}", exception);
                MachineName = string.Empty;
            }
        }

        /// <summary>
        ///     Renders the machine name and appends it to the specified <see cref="StringBuilder" />.
        /// </summary>
        /// <param name="builder">
        ///     The <see cref="StringBuilder" /> to append the rendered data to.
        /// </param>
        /// <param name="logEvent">Logging event.</param>
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            builder.Append(MachineName);
        }
    }
}

#endif