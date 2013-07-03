using System.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Transformalize.Rhino.Etl.Core.Infrastructure;

namespace Transformalize.Rhino.Etl.Core.Operations {
    /// <summary>
    /// Perform a batch command against SQL server
    /// </summary>
    public abstract class SqlBatchOperation : AbstractDatabaseOperation {

        private const string PROVIDER = "System.Data.SqlClient.SqlConnection, System.Data, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
        private int _batchSize = 50;
        private int _timeout;

        /// <summary>
        /// Gets or sets the size of the batch.
        /// </summary>
        /// <value>The size of the batch.</value>
        public int BatchSize {
            get { return _batchSize; }
            set { _batchSize = value; }
        }

        /// <summary>
        /// The timeout of the command set
        /// </summary>
        public int Timeout {
            get { return _timeout; }
            set { _timeout = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlBatchOperation"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        protected SqlBatchOperation(string connectionString)
            : this(GetConnectionStringSettings(connectionString)) {
        }

        private static ConnectionStringSettings GetConnectionStringSettings(string connectionString) {
            return new ConnectionStringSettings {
                ConnectionString = connectionString,
                ProviderName = PROVIDER,
            };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlBatchOperation"/> class.
        /// </summary>
        /// <param name="connectionStringSettings">The connection string settings to use.</param>
        protected SqlBatchOperation(ConnectionStringSettings connectionStringSettings)
            : base(connectionStringSettings) {
            base.ParamPrefix = "@";
        }

        /// <summary>
        /// Executes this operation
        /// </summary>
        /// <param name="rows">The rows.</param>
        /// <returns></returns>
        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            Guard.Against<ArgumentException>(rows == null, "SqlBatchOperation cannot accept a null enumerator");
            using (var connection = (SqlConnection)Use.Connection(ConnectionStringSettings))
            using (var transaction = connection.BeginTransaction()) {
                SqlCommandSet commandSet = null;
                CreateCommandSet(connection, transaction, ref commandSet, _timeout);
                foreach (var row in rows) {
                    var command = new SqlCommand();
                    PrepareCommand(row, command);
                    if (command.Parameters.Count == 0) //workaround around a framework bug
                    {
                        var guid = Guid.NewGuid();
                        command.Parameters.AddWithValue(guid.ToString(), guid);
                    }
                    commandSet.Append(command);
                    if (commandSet.CountOfCommands >= _batchSize) {
                        Debug("Executing batch of {0} commands", commandSet.CountOfCommands);
                        commandSet.ExecuteNonQuery();
                        CreateCommandSet(connection, transaction, ref commandSet, _timeout);
                    }
                }
                Debug("Executing final batch of {0} commands", commandSet.CountOfCommands);
                commandSet.ExecuteNonQuery();

                if (PipelineExecuter.HasErrors) {
                    Warn(null, "Rolling back transaction in {0}", Name);
                    transaction.Rollback();
                    Warn(null, "Rolled back transaction in {0}", Name);
                } else {
                    Debug("Committing {0}", Name);
                    transaction.Commit();
                    Debug("Committed {0}", Name);
                }
            }
            yield break;
        }

        /// <summary>
        /// Prepares the command from the given row
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="command">The command.</param>
        protected abstract void PrepareCommand(Row row, SqlCommand command);

        private static void CreateCommandSet(SqlConnection connection, SqlTransaction transaction, ref SqlCommandSet commandSet, int timeout) {
            if (commandSet != null)
                commandSet.Dispose();
            commandSet = new SqlCommandSet {
                Connection = connection,
                Transaction = transaction,
                CommandTimeout = timeout
            };
        }
    }
}