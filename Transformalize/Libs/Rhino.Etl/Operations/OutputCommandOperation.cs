#region License

// /*
// Transformalize - Replicate, Transform, and Denormalize Your Data...
// Copyright (C) 2013 Dale Newman
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
// */

#endregion

using System.Collections.Generic;
using System.Data;
using Transformalize.Libs.Rhino.Etl.Enumerables;
using Transformalize.Libs.Rhino.Etl.Infrastructure;
using Transformalize.Main.Providers;

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
            using (var cn = Use.Connection(Connection))
            using (var transaction = BeginTransaction(cn))
            {
                foreach (Row row in new SingleRowEventRaisingEnumerator(this, rows))
                {
                    using (var cmd = cn.CreateCommand())
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