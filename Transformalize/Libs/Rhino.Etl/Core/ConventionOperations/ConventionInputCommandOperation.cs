using System.Configuration;
using System.Data;
using Transformalize.Libs.Rhino.Etl.Core.Operations;
using Transformalize.Providers;

namespace Transformalize.Libs.Rhino.Etl.Core.ConventionOperations {
    /// <summary>
    /// A convention based version of <see cref="InputCommandOperation"/>. Will
    /// figure out as many things as it can on its own.
    /// </summary>
    public class ConventionInputCommandOperation : InputCommandOperation {

        private const string PROVIDER = "System.Data.SqlClient.SqlConnection, System.Data, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
 
        /// <summary>
        /// Gets or sets the command to get the input from the database
        /// </summary>
        public string Command { get; set; }

        ///<summary>
        /// Gets or sets the timeout value for the database command
        ///</summary>
        public int Timeout { get; set; }

        public ConventionInputCommandOperation(IConnection connection) : base(connection) {
            UseTransaction = false;
            Timeout = 0;
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