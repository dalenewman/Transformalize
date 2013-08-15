using System.Collections.Generic;
using System.Configuration;
using System.Data;
using Transformalize.Libs.Rhino.Etl.Core.Infrastructure;

namespace Transformalize.Libs.Rhino.Etl.Core.Operations {
    /// <summary>
    /// Generic input command operation
    /// </summary>
    public abstract class InputCommandOperation : AbstractCommandOperation {
        
        /// <summary>
        /// Initializes a new instance of the <see cref="InputCommandOperation"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        protected InputCommandOperation(string connectionString) : this(GetConnectionStringSettings(connectionString) ) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InputCommandOperation"/> class.
        /// </summary>
        /// <param name="connectionStringSettings">Connection string settings to use.</param>
        protected InputCommandOperation(ConnectionStringSettings connectionStringSettings) : base(connectionStringSettings) {
            UseTransaction = false;
        }

        private static ConnectionStringSettings GetConnectionStringSettings(string connectionString) {
            return new ConnectionStringSettings {
                ConnectionString = connectionString,
            };
        }

        /// <summary>
        /// Executes this operation
        /// </summary>
        /// <param name="rows">The rows.</param>
        /// <returns></returns>
        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            using (var connection = Use.Connection(ConnectionStringSettings))
            using (var transaction = BeginTransaction(connection)) {
                using (currentCommand = connection.CreateCommand()) {
                    currentCommand.Transaction = transaction;
                    PrepareCommand(currentCommand);
                    using (var reader = currentCommand.ExecuteReader()) {
                        while (reader.Read()) {
                            yield return CreateRowFromReader(reader);
                        }
                    }
                }

                if (transaction != null) transaction.Commit();
            }
        }

        /// <summary>
        /// Creates a row from the reader.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns></returns>
        protected abstract Row CreateRowFromReader(IDataReader reader);

        /// <summary>
        /// Prepares the command for execution, set command text, parameters, etc
        /// </summary>
        /// <param name="cmd">The command.</param>
        protected abstract void PrepareCommand(IDbCommand cmd);
    }
}
