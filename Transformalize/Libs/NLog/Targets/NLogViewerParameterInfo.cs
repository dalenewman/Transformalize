#region License
// /*
// See license included in this library folder.
// */
#endregion

using Transformalize.Libs.NLog.Config;
using Transformalize.Libs.NLog.Layouts;

namespace Transformalize.Libs.NLog.Targets
{
    /// <summary>
    ///     Represents a parameter to a NLogViewer target.
    /// </summary>
    [NLogConfigurationItem]
    public class NLogViewerParameterInfo
    {
        /// <summary>
        ///     Gets or sets viewer parameter name.
        /// </summary>
        /// <docgen category='Parameter Options' order='10' />
        [RequiredParameter]
        public string Name { get; set; }

        /// <summary>
        ///     Gets or sets the layout that should be use to calcuate the value for the parameter.
        /// </summary>
        /// <docgen category='Parameter Options' order='10' />
        [RequiredParameter]
        public Layout Layout { get; set; }
    }
}