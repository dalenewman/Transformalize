using System.Configuration;
using System.Data;
using Transformalize.Rhino.Etl.Core.Operations;

namespace Transformalize.Rhino.Etl.Core.ConventionOperations {
    /// <summary>
    /// A convention based version of <see cref="InputCommandOperation"/>. Will
    /// figure out as many things as it can on its own.
    /// </summary>
    public class ConventionInputCommandOperation : InputCommandOperation {
        /// <summary>
        /// Gets or sets the command to get the input from the database
        /// </summary>
        public string Command { get; set; }

        ///<summary>
        /// Gets or sets the timeout value for the database command
        ///</summary>
        public int Timeout { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConventionInputCommandOperation"/> class.
        /// </summary>
        /// <param name="connectionStringName">Name of the connection string.</param>
        public ConventionInputCommandOperation(string connectionStringName)
            : this(ConfigurationManager.ConnectionStrings[connectionStringName]) {
            Timeout = 0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConventionInputCommandOperation"/> class.
        /// </summary>
        /// <param name="connectionStringSettings">Name of the connection string.</param>
        public ConventionInputCommandOperation(ConnectionStringSettings connectionStringSettings)
            : base(connectionStringSettings) {
        }

        /// <summary>
        /// Creates a row from the reader.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns></returns>
        protected override Row CreateRowFromReader(IDataReader reader) {
            return Row.FromReader(reader);
        }

        /// <summary>
        /// Prepares the command for execution, set command text, parameters, etc
        /// </summary>
        /// <param name="cmd">The command.</param>
        protected override void PrepareCommand(IDbCommand cmd) {
            cmd.CommandText = Command;
            cmd.CommandTimeout = Timeout;
        }
    }
}