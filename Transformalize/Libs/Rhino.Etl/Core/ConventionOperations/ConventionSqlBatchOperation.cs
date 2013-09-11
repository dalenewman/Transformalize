using System.Data.SqlClient;
using Transformalize.Libs.Rhino.Etl.Core.Operations;
using Transformalize.Providers;

namespace Transformalize.Libs.Rhino.Etl.Core.ConventionOperations
{
    /// <summary>
    /// Convention class for sql batching.
    /// </summary>
    public class ConventionSqlBatchOperation : SqlBatchOperation
    {
        private string command;

        /// <summary>
        /// Gets or sets the command text to execute
        /// </summary>
        /// <value>The command.</value>
        public string Command
        {
            get { return command; }
            set { command = value; }
        }

        public ConventionSqlBatchOperation(AbstractConnection connection) : base(connection)
        {
        }

        /// <summary>
        /// Prepares the command.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="sqlCommand">The SQL command.</param>
        protected override void PrepareCommand(Row row, SqlCommand sqlCommand)
        {
            sqlCommand.CommandText = Command;    
            CopyRowValuesToCommandParameters(sqlCommand, row);
        }
    }
}