using System.Collections.Generic;
using System.Configuration;
using System.Data;
using Transformalize.Libs.Rhino.Etl.Core.Enumerables;
using Transformalize.Libs.Rhino.Etl.Core.Infrastructure;

namespace Transformalize.Libs.Rhino.Etl.Core.Operations
{
    /// <summary>
    /// Generic output command operation
    /// </summary>
    public abstract class OutputCommandOperation : AbstractCommandOperation
    {
        private const string PROVIDER = "System.Data.SqlClient.SqlConnection, System.Data, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";

        /// <summary>
        /// Initializes a new instance of the <see cref="OutputCommandOperation"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        protected OutputCommandOperation(string connectionString) : this(GetConnectionStringSettings(connectionString))
        {
        }

        private static ConnectionStringSettings GetConnectionStringSettings(string connectionString) {
            return new ConnectionStringSettings {
                ConnectionString = connectionString,
                ProviderName = PROVIDER,
            };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OutputCommandOperation"/> class.
        /// </summary>
        /// <param name="connectionStringSettings">Connection string settings to use.</param>
        protected OutputCommandOperation(ConnectionStringSettings connectionStringSettings)
            : base(connectionStringSettings)
        {
        }

        /// <summary>
        /// Executes this operation
        /// </summary>
        /// <param name="rows">The rows.</param>
        /// <returns></returns>
        public override IEnumerable<Row> Execute(IEnumerable<Row> rows)
        {
            using (IDbConnection connection = Use.Connection(ConnectionStringSettings))
            using (IDbTransaction transaction = BeginTransaction(connection))
            {
                foreach (Row row in new SingleRowEventRaisingEnumerator(this, rows))
                {
                    using (IDbCommand cmd = connection.CreateCommand())
                    {
                        currentCommand = cmd;
                        currentCommand.Transaction = transaction;
                        PrepareCommand(currentCommand, row);
                        currentCommand.ExecuteNonQuery();
                    }
                }
                if (PipelineExecuter.HasErrors)
                {
                    Warn("Rolling back transaction in {0}", Name);
                    transaction.Rollback();
                    Warn("Rolled back transaction in {0}", Name);
                }
                else
                {
                    Debug("Committing {0}", Name);
                    if (transaction != null) transaction.Commit();
                    Debug("Committed {0}", Name);
                }
            }
            yield break;
        }

        /// <summary>
        /// Prepares the command for execution, set command text, parameters, etc
        /// </summary>
        /// <param name="cmd">The command.</param>
        /// <param name="row">The row.</param>
        protected abstract void PrepareCommand(IDbCommand cmd, Row row);
    }
}
