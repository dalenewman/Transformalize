/*
Transformalize - Replicate, Transform, and Denormalize Your Data...
Copyright (C) 2013 Dale Newman

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System.Data.SqlClient;
using Transformalize.Libs.Rhino.Etl.Core.Operations;
using Transformalize.Model;

namespace Transformalize.Operations {
    public class EntityBulkInsert : SqlBulkInsertOperation {
        private readonly Entity _entity;

        public EntityBulkInsert(Entity entity) : base(entity.OutputConnection.ConnectionString, entity.OutputName()) {
            
            _entity = entity;
            UseTransaction = false;

            TurnOptionOn(SqlBulkCopyOptions.TableLock);
            TurnOptionOff(SqlBulkCopyOptions.UseInternalTransaction);
            TurnOptionOff(SqlBulkCopyOptions.CheckConstraints);
            TurnOptionOff(SqlBulkCopyOptions.FireTriggers);
        }

        protected override void PrepareSchema() {
            NotifyBatchSize = 10000;
            BatchSize = _entity.OutputConnection.BatchSize;

            var fields = new FieldSqlWriter(_entity.All).ExpandXml().Output().AddBatchId(false).Context();
            foreach (var pair in fields) {
                Schema[pair.Key] = pair.Value.SystemType;
            }
        }

        protected override void OnSqlRowsCopied(object sender, SqlRowsCopiedEventArgs e) {
           Info("{0} | Processed {1} rows in EntityBulkInsert", _entity.ProcessName, e.RowsCopied);
        }

    }
}