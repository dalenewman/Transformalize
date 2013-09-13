using System.Collections.Generic;
using System.Data;
using Transformalize.Main.Providers;
using Transformalize.Libs.Rhino.Etl.Enumerables;
using Transformalize.Libs.Rhino.Etl.Infrastructure;

namespace Transformalize.Libs.Rhino.Etl.Operations
{
    /// <summary>
    ///     Generic output command operation
    /// </summary>
    public abstract class OutputCommandOperation : AbstractCommandOperation
    {
        protected OutputCommandOperation(AbstractConnection connection) : base(connection)
        {
        }

        /// <summary>
        ///     Executes this operation
        /// </summary>
        /// <param name="rows">The rows.</param>
        /// <returns></returns>
        public override IEnumerable<Row> Execute(IEnumerable<Row> rows)
        {
            using (IDbConnection cn = Use.Connection(Connection))
            using (IDbTransaction transaction = BeginTransaction(cn))
            {
                foreach (Row row in new SingleRowEventRaisingEnumerator(this, rows))
                {
                    using (IDbCommand cmd = cn.CreateCommand())
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
        ///     Prepares the command for execution, set command text, parameters, etc
        /// </summary>
        /// <param name="cmd">The command.</param>
        /// <param name="row">The row.</param>
        protected abstract void PrepareCommand(IDbCommand cmd, Row row);
    }
}