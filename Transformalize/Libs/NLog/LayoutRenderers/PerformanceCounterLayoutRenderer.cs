#region License
// /*
// See license included in this library folder.
// */
#endregion

using System.Diagnostics;
using System.Globalization;
using System.Text;
using Transformalize.Libs.NLog.Config;

#if !NET_CF && !SILVERLIGHT

namespace Transformalize.Libs.NLog.LayoutRenderers
{
    /// <summary>
    ///     The performance counter.
    /// </summary>
    [LayoutRenderer("performancecounter")]
    public class PerformanceCounterLayoutRenderer : LayoutRenderer
    {
        private PerformanceCounter perfCounter;

        /// <summary>
        ///     Gets or sets the name of the counter category.
        /// </summary>
        /// <docgen category='Performance Counter Options' order='10' />
        [RequiredParameter]
        public string Category { get; set; }

        /// <summary>
        ///     Gets or sets the name of the performance counter.
        /// </summary>
        /// <docgen category='Performance Counter Options' order='10' />
        [RequiredParameter]
        public string Counter { get; set; }

        /// <summary>
        ///     Gets or sets the name of the performance counter instance (e.g. this.Global_).
        /// </summary>
        /// <docgen category='Performance Counter Options' order='10' />
        public string Instance { get; set; }

        /// <summary>
        ///     Gets or sets the name of the machine to read the performance counter from.
        /// </summary>
        /// <docgen category='Performance Counter Options' order='10' />
        public string MachineName { get; set; }

        /// <summary>
        ///     Initializes the layout renderer.
        /// </summary>
        protected override void InitializeLayoutRenderer()
        {
            base.InitializeLayoutRenderer();

            if (MachineName != null)
            {
                perfCounter = new PerformanceCounter(Category, Counter, Instance, MachineName);
            }
            else
            {
                perfCounter = new PerformanceCounter(Category, Counter, Instance, true);
            }
        }

        /// <summary>
        ///     Closes the layout renderer.
        /// </summary>
        protected override void CloseLayoutRenderer()
        {
            base.CloseLayoutRenderer();
            if (perfCounter != null)
            {
                perfCounter.Close();
                perfCounter = null;
            }
        }

        /// <summary>
        ///     Renders the specified environment variable and appends it to the specified <see cref="StringBuilder" />.
        /// </summary>
        /// <param name="builder">
        ///     The <see cref="StringBuilder" /> to append the rendered data to.
        /// </param>
        /// <param name="logEvent">Logging event.</param>
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            builder.Append(perfCounter.NextValue().ToString(CultureInfo.InvariantCulture));
        }
    }
}

#endif