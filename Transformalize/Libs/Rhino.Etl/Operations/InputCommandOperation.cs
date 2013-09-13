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
using Transformalize.Libs.Rhino.Etl.Infrastructure;
using Transformalize.Main.Providers;

namespace Transformalize.Libs.Rhino.Etl.Operations
{
    /// <summary>
    ///     Generic input command operation
    /// </summary>
    public abstract class InputCommandOperation : AbstractCommandOperation
    {
        protected InputCommandOperation(AbstractConnection connection) : base(connection)
        {
            UseTransaction = false;
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
                using (currentCommand = cn.CreateCommand())
                {
                    currentCommand.Transaction = transaction;
                    PrepareCommand(currentCommand);
                    using (var reader = currentCommand.ExecuteReader(CommandBehavior.SequentialAccess))
                    {
                        while (reader.Read())
                        {
                            yield return CreateRowFromReader(reader);
                        }
                    }
                }

                if (transaction != null) transaction.Commit();
            }
        }

        /// <summary>
        ///     Creates a row from the reader.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns></returns>
        protected abstract Row CreateRowFromReader(IDataReader reader);

        /// <summary>
        ///     Prepares the command for execution, set command text, parameters, etc
        /// </summary>
        /// <param name="cmd">The command.</param>
        protected abstract void PrepareCommand(IDbCommand cmd);
    }
}