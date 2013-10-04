#region License
// /*
// See license included in this library folder.
// */
#endregion

using System;
using System.ComponentModel;
using System.Text;

namespace Transformalize.Libs.NLog.LayoutRenderers
{
    /// <summary>
    ///     Globally-unique identifier (GUID).
    /// </summary>
    [LayoutRenderer("guid")]
    public class GuidLayoutRenderer : LayoutRenderer
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="GuidLayoutRenderer" /> class.
        /// </summary>
        public GuidLayoutRenderer()
        {
            Format = "N";
        }

        /// <summary>
        ///     Gets or sets the GUID format as accepted by Guid.ToString() method.
        /// </summary>
        /// <docgen category='Rendering Options' order='10' />
        [DefaultValue("N")]
        public string Format { get; set; }

        /// <summary>
        ///     Renders a newly generated GUID string and appends it to the specified <see cref="StringBuilder" />.
        /// </summary>
        /// <param name="builder">
        ///     The <see cref="StringBuilder" /> to append the rendered data to.
        /// </param>
        /// <param name="logEvent">Logging event.</param>
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            builder.Append(Guid.NewGuid().ToString(Format));
        }
    }
}