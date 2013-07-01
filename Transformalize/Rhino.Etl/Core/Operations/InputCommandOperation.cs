using System.Configuration;
using System.Collections.Generic;
using System.Data;
using Transformalize.Rhino.Etl.Core.Infrastructure;

namespace Transformalize.Rhino.Etl.Core.Operations {
    /// <summary>
    /// Generic input command operation
    /// </summary>
    public abstract class InputCommandOperation : AbstractCommandOperation {
        
        private const string PROVIDER = "System.Data.SqlClient.SqlConnection, System.Data, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";

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
                ProviderName = PROVIDER,
            };
        }

        /// <summary>
        /// Executes this operation
        /// </summary>
        /// <param name="rows">The rows.</param>
        /// <returns></returns>
        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            using (IDbConnection connection = Use.Connection(ConnectionStringSettings))
            using (IDbTransaction transaction = BeginTransaction(connection)) {
                using (currentCommand = connection.CreateCommand()) {
                    currentCommand.Transaction = transaction;
                    PrepareCommand(currentCommand);
                    using (IDataReader reader = currentCommand.ExecuteReader()) {
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
