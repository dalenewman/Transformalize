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
using Transformalize.Main;

namespace Transformalize.Operations
{
    public class EntityBulkInsert : SqlBulkInsertOperation
    {
        private readonly Process _process;
        private readonly Entity _entity;

        public EntityBulkInsert(Process process, Entity entity) : base(process.OutputConnection, entity.OutputName())
        {
            _process = process;
            _entity = entity;
            UseTransaction = false;

            TurnOptionOn(SqlBulkCopyOptions.TableLock);
            TurnOptionOff(SqlBulkCopyOptions.UseInternalTransaction);
            TurnOptionOff(SqlBulkCopyOptions.CheckConstraints);
            TurnOptionOff(SqlBulkCopyOptions.FireTriggers);
        }

        protected override void PrepareSchema()
        {
            NotifyBatchSize = 10000;
            BatchSize = _process.OutputConnection.BatchSize;

            var fields = new FieldSqlWriter(_entity.Fields, _entity.CalculatedFields).Output().AddBatchId(false).ToArray();
            foreach (var field in fields)
            {
                Schema[field.Alias] = field.SystemType;
            }
        }

        protected override void OnSqlRowsCopied(object sender, SqlRowsCopiedEventArgs e)
        {
            Info("Processed {0} rows in EntityBulkInsert", e.RowsCopied);
        }
    }
}