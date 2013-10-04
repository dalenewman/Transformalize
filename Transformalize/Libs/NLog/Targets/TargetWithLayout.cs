#region License
// /*
// See license included in this library folder.
// */
#endregion

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Transformalize.Libs.NLog.Config;
using Transformalize.Libs.NLog.Layouts;

namespace Transformalize.Libs.NLog.Targets
{
    /// <summary>
    ///     Represents target that supports string formatting using layouts.
    /// </summary>
    public abstract class TargetWithLayout : Target
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="TargetWithLayout" /> class.
        /// </summary>
        /// <remarks>
        ///     The default value of the layout is: <code>${longdate}|${level:uppercase=true}|${logger}|${message}</code>
        /// </remarks>
        [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification = "This one is safe.")]
        protected TargetWithLayout()
        {
            Layout = "${longdate}|${level:uppercase=true}|${logger}|${message}";
        }

        /// <summary>
        ///     Gets or sets the layout used to format log messages.
        /// </summary>
        /// <docgen category='Layout Options' order='1' />
        [RequiredParameter]
        [DefaultValue("${longdate}|${level:uppercase=true}|${logger}|${message}")]
        public virtual Layout Layout { get; set; }
    }
}