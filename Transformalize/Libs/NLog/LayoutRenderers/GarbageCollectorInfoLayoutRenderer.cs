#region License
// /*
// See license included in this library folder.
// */
#endregion

using System;
using System.ComponentModel;
using System.Globalization;
using System.Text;

#if !NET_CF && !MONO

namespace Transformalize.Libs.NLog.LayoutRenderers
{
    /// <summary>
    ///     The information about the garbage collector.
    /// </summary>
    [LayoutRenderer("gc")]
    public class GarbageCollectorInfoLayoutRenderer : LayoutRenderer
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="GarbageCollectorInfoLayoutRenderer" /> class.
        /// </summary>
        public GarbageCollectorInfoLayoutRenderer()
        {
            Property = GarbageCollectorProperty.TotalMemory;
        }

        /// <summary>
        ///     Gets or sets the property to retrieve.
        /// </summary>
        /// <docgen category='Rendering Options' order='10' />
        [DefaultValue("TotalMemory")]
        public GarbageCollectorProperty Property { get; set; }

        /// <summary>
        ///     Renders the selected process information.
        /// </summary>
        /// <param name="builder">
        ///     The <see cref="StringBuilder" /> to append the rendered data to.
        /// </param>
        /// <param name="logEvent">Logging event.</param>
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            object value = null;

            switch (Property)
            {
                case GarbageCollectorProperty.TotalMemory:
                    value = GC.GetTotalMemory(false);
                    break;

                case GarbageCollectorProperty.TotalMemoryForceCollection:
                    value = GC.GetTotalMemory(true);
                    break;

#if !SILVERLIGHT
                case GarbageCollectorProperty.CollectionCount0:
                    value = GC.CollectionCount(0);
                    break;

                case GarbageCollectorProperty.CollectionCount1:
                    value = GC.CollectionCount(1);
                    break;

                case GarbageCollectorProperty.CollectionCount2:
                    value = GC.CollectionCount(2);
                    break;

#endif

                case GarbageCollectorProperty.MaxGeneration:
                    value = GC.MaxGeneration;
                    break;
            }

            builder.Append(Convert.ToString(value, CultureInfo.InvariantCulture));
        }
    }
}

#endif