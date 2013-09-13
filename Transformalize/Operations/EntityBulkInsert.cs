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
using Transformalize.Main;
using Transformalize.Libs.Rhino.Etl.Operations;

namespace Transformalize.Operations
{
    public class EntityBulkInsert : SqlBulkInsertOperation
    {
        private readonly Entity _entity;

        public EntityBulkInsert(Entity entity) : base(entity.OutputConnection, entity.OutputName())
        {
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
            BatchSize = _entity.OutputConnection.BatchSize;

            Field[] fields = new FieldSqlWriter(_entity.All, _entity.CalculatedFields).ExpandXml().Output().AddBatchId(false).ToArray();
            foreach (Field field in fields)
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