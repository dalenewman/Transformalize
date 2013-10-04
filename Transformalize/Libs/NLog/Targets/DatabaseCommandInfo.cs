#region License
// /*
// See license included in this library folder.
// */
#endregion

using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using Transformalize.Libs.NLog.Config;
using Transformalize.Libs.NLog.Layouts;

#if !SILVERLIGHT

namespace Transformalize.Libs.NLog.Targets
{
    /// <summary>
    ///     Information about database command + parameters.
    /// </summary>
    [NLogConfigurationItem]
    public class DatabaseCommandInfo
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="DatabaseCommandInfo" /> class.
        /// </summary>
        public DatabaseCommandInfo()
        {
            Parameters = new List<DatabaseParameterInfo>();
            CommandType = CommandType.Text;
        }

        /// <summary>
        ///     Gets or sets the type of the command.
        /// </summary>
        /// <value>The type of the command.</value>
        /// <docgen category='Command Options' order='10' />
        [RequiredParameter]
        [DefaultValue(CommandType.Text)]
        public CommandType CommandType { get; set; }

        /// <summary>
        ///     Gets or sets the connection string to run the command against. If not provided, connection string from the target is used.
        /// </summary>
        /// <docgen category='Command Options' order='10' />
        public Layout ConnectionString { get; set; }

        /// <summary>
        ///     Gets or sets the command text.
        /// </summary>
        /// <docgen category='Command Options' order='10' />
        [RequiredParameter]
        public Layout Text { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether to ignore failures.
        /// </summary>
        /// <docgen category='Command Options' order='10' />
        public bool IgnoreFailures { get; set; }

        /// <summary>
        ///     Gets the collection of parameters. Each parameter contains a mapping
        ///     between NLog layout and a database named or positional parameter.
        /// </summary>
        /// <docgen category='Command Options' order='10' />
        [ArrayParameter(typeof (DatabaseParameterInfo), "parameter")]
        public IList<DatabaseParameterInfo> Parameters { get; private set; }
    }
}

#endif