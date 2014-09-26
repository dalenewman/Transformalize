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
using System.Linq;
using System.Text;
using Transformalize.Libs.NLog;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Main;
using Transformalize.Libs.Dapper;
using Transformalize.Main.Providers;

namespace Transformalize.Operations {
    public class EntityUpdateMaster : AbstractOperation {

        private readonly Entity _entity;
        private readonly Process _process;

        public EntityUpdateMaster(Process process, Entity entity) {
            _process = process;
            _entity = entity;
            EntityName = entity.Name;
            UseTransaction = false;
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {

            //escape 1
            if (_entity.IsMaster() || (!_entity.HasForeignKeys() && _process.IsFirstRun))
                return rows;

            //escape 2
            var entityChanged = _entity.Inserts + _entity.Updates > 0;
            var masterChanged = _process.MasterEntity.Inserts + _process.MasterEntity.Updates > 0;
            if (!entityChanged && !masterChanged)
                return rows;

            var master = _process.OutputConnection.Enclose(_process.MasterEntity.OutputName());
            var entity = _process.OutputConnection.Enclose(_entity.OutputName());

            using (var cn = _process.OutputConnection.GetConnection()) {
                cn.Open();

                int records;
                string where;
                if (entityChanged && masterChanged) {
                    where = string.Format("WHERE {0}.TflBatchId = @TflBatchId OR {1}.TflBatchId = @MasterTflBatchId;", entity, master);
                    var sql = PrepareSql(master, entity, _process.OutputConnection) + where;
                    Debug(sql);
                    records = cn.Execute(sql, new { _entity.TflBatchId, MasterTflBatchId = _process.MasterEntity.TflBatchId }, commandTimeout: 0);
                } else {
                    where = string.Format("WHERE {0}.TflBatchId = @TflBatchId;", entityChanged ? entity : master);
                    var sql = PrepareSql(master, entity, _process.OutputConnection) + where;
                    Debug(sql);
                    records = cn.Execute(sql, new { TflBatchId = entityChanged ? _entity.TflBatchId : _process.MasterEntity.TflBatchId }, commandTimeout: 0);
                }

                Debug("TflBatchId = {0}.", _entity.TflBatchId);
                Info("Processed {0} rows. Updated {1} with {2}.", records, _process.MasterEntity.Alias, _entity.Alias);
            }
            return rows;
        }

        private string PrepareSql(string master, string entity, AbstractConnection connection) {
            //note: TflBatchId is updated and next process depends it.

            var builder = new StringBuilder();

            var sets = _entity.HasForeignKeys() ? new FieldSqlWriter(_entity.Fields).FieldType(FieldType.ForeignKey).Alias(connection.L, connection.R).Set(master, entity).Write(",\r\n    ") + "," : string.Empty;

            builder.AppendFormat("UPDATE {0}\r\n", master);
            builder.AppendFormat("SET {0} {1}.TflBatchId = @TflBatchId\r\n", sets, master);
            builder.AppendFormat("FROM {0}\r\n", entity);

            foreach (var relationship in _entity.RelationshipToMaster) {
                var left = connection.Enclose(relationship.LeftEntity.OutputName());
                var right = connection.Enclose(relationship.RightEntity.OutputName());
                var join = string.Join(" AND ", relationship.Join.Select(j => string.Format("{0}.{1} = {2}.{3}", left, connection.Enclose(j.LeftField.Alias), right, connection.Enclose(j.RightField.Alias))));
                builder.AppendFormat("INNER JOIN {0} ON ({1})\r\n", left, join);
            }

            return builder.ToString();
        }
    }
}