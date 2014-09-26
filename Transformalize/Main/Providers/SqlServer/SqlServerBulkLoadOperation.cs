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

using System.Data.SqlClient;
using Transformalize.Libs.Rhino.Etl.Operations;

namespace Transformalize.Main.Providers.SqlServer {
    public class SqlServerBulkLoadOperation : SqlBulkInsertOperation {
        private readonly Entity _entity;
        private readonly int _batchSize;

        public SqlServerBulkLoadOperation(AbstractConnection connection, Entity entity)
            : base(connection, connection.Enclose(entity.OutputName())) {
            _entity = entity;
            _batchSize = connection.BatchSize;
            UseTransaction = false;

            TurnOptionOn(SqlBulkCopyOptions.TableLock);
            TurnOptionOn(SqlBulkCopyOptions.UseInternalTransaction);
            TurnOptionOff(SqlBulkCopyOptions.CheckConstraints);
            TurnOptionOff(SqlBulkCopyOptions.FireTriggers);
            TurnOptionOn(SqlBulkCopyOptions.KeepNulls);
        }

        protected override void PrepareSchema() {
            NotifyBatchSize = 10000;
            BatchSize = _batchSize;

            var fromFields = new Fields(_entity.Fields, _entity.CalculatedFields).WithOutput().AddBatchId(_entity.Index, false);
            if (_entity.IsMaster())
                fromFields.AddDeleted(_entity.Index, false);

            var toFields = new SqlServerEntityAutoFieldReader().Read(Connection, _entity.ProcessName, _entity.Prefix, _entity.OutputName(), Connection.DefaultSchema, _entity.IsMaster());

            foreach (var field in fromFields) {
                Schema[field.Alias] = field.SystemType;
            }

            foreach (var from in fromFields) {
                if (toFields.HaveField(from.Alias)) {
                    var to = toFields.Find(from.Alias).First();
                    if (!to.SimpleType.Equals(from.SimpleType)) {
                        if (!to.SimpleType.Equals("byte[]") && from.SimpleType.Equals("rowversion")) {
                            throw new TransformalizeException("{0} has a matching {1} fields, but different types: {2} != {3}.", TargetTable, from.Alias, from.SimpleType, to.SimpleType);
                        }
                    }
                } else {
                    throw new TransformalizeException("{0} does not have a matching {1} field.", TargetTable, from.Alias);
                }
            }
        }

        protected override void OnSqlRowsCopied(object sender, SqlRowsCopiedEventArgs e) {
            TflLogger.Info(_entity.ProcessName, _entity.Name, "Processed {0} rows in EntityBulkInsert", e.RowsCopied);
        }
    }
}