#region License
// /*
// See license included in this library folder.
// */
#endregion

using System.Data.SqlClient;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Main.Providers;

namespace Transformalize.Libs.Rhino.Etl.ConventionOperations
{
    /// <summary>
    ///     Convention class for sql batching.
    /// </summary>
    public class ConventionSqlBatchOperation : SqlBatchOperation
    {
        public ConventionSqlBatchOperation(AbstractConnection connection) : base(connection)
        {
        }

        /// <summary>
        ///     Gets or sets the command text to execute
        /// </summary>
        /// <value>The command.</value>
        public string Command { get; set; }

        /// <summary>
        ///     Prepares the command.
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