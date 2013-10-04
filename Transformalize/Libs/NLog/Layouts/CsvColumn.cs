#region License
// /*
// See license included in this library folder.
// */
#endregion

using Transformalize.Libs.NLog.Config;

namespace Transformalize.Libs.NLog.Layouts
{
    /// <summary>
    ///     A column in the CSV.
    /// </summary>
    [NLogConfigurationItem]
    [ThreadAgnostic]
    public class CsvColumn
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="CsvColumn" /> class.
        /// </summary>
        public CsvColumn()
            : this(null, null)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="CsvColumn" /> class.
        /// </summary>
        /// <param name="name">The name of the column.</param>
        /// <param name="layout">The layout of the column.</param>
        public CsvColumn(string name, Layout layout)
        {
            Name = name;
            Layout = layout;
        }

        /// <summary>
        ///     Gets or sets the name of the column.
        /// </summary>
        /// <docgen category='CSV Column Options' order='10' />
        public string Name { get; set; }

        /// <summary>
        ///     Gets or sets the layout of the column.
        /// </summary>
        /// <docgen category='CSV Column Options' order='10' />
        [RequiredParameter]
        public Layout Layout { get; set; }
    }
}