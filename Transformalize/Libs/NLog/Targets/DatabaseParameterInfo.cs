#region License
// /*
// See license included in this library folder.
// */
#endregion

using System.ComponentModel;
using Transformalize.Libs.NLog.Config;
using Transformalize.Libs.NLog.Layouts;

#if !SILVERLIGHT

namespace Transformalize.Libs.NLog.Targets
{
    /// <summary>
    ///     Represents a parameter to a Database target.
    /// </summary>
    [NLogConfigurationItem]
    public class DatabaseParameterInfo
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="DatabaseParameterInfo" /> class.
        /// </summary>
        public DatabaseParameterInfo()
            : this(null, null)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="DatabaseParameterInfo" /> class.
        /// </summary>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <param name="parameterLayout">The parameter layout.</param>
        public DatabaseParameterInfo(string parameterName, Layout parameterLayout)
        {
            Name = parameterName;
            Layout = parameterLayout;
        }

        /// <summary>
        ///     Gets or sets the database parameter name.
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

        /// <summary>
        ///     Gets or sets the database parameter size.
        /// </summary>
        /// <docgen category='Parameter Options' order='10' />
        [DefaultValue(0)]
        public int Size { get; set; }

        /// <summary>
        ///     Gets or sets the database parameter precision.
        /// </summary>
        /// <docgen category='Parameter Options' order='10' />
        [DefaultValue(0)]
        public byte Precision { get; set; }

        /// <summary>
        ///     Gets or sets the database parameter scale.
        /// </summary>
        /// <docgen category='Parameter Options' order='10' />
        [DefaultValue(0)]
        public byte Scale { get; set; }
    }
}

#endif