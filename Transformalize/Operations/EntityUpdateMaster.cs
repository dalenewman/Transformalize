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

using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Transformalize.Core.Entity_;
using Transformalize.Core.Field_;
using Transformalize.Core.Process_;
using Transformalize.Libs.Rhino.Etl.Core;
using Transformalize.Libs.Rhino.Etl.Core.Operations;

namespace Transformalize.Operations
{
    public class EntityUpdateMaster : AbstractOperation {
        private readonly Process _process;
        private readonly Entity _entity;

        public EntityUpdateMaster(Process process, Entity entity) {
            _process = process;
            _entity = entity;
            UseTransaction = false;
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows)
        {
            if (_entity.IsMaster())
                return rows;

            if (_entity.RecordsAffected == 0)
                return rows;

            if (Process.OutputRecordsExist || _entity.HasForeignKeys()) {
                using (var cn = new SqlConnection(_process.MasterEntity.OutputConnection.ConnectionString)) {
                    cn.Open();
                    var cmd = new SqlCommand(PrepareSql(), cn) {CommandTimeout = 0};

                    Debug(cmd.CommandText);
                    
                    cmd.Parameters.Add(new SqlParameter("@TflBatchId", _entity.TflBatchId));
                    var records = cmd.ExecuteNonQuery();

                    Info("{0} | Processed {1} rows in EntityUpdateMaster", Process.Name, records);
                }
            }
            return rows;
        }

        private string PrepareSql() {
            var builder = new StringBuilder();
            var masterEntity = _process.MasterEntity;

            var master = string.Format("[{0}]", masterEntity.OutputName());
            var source = string.Format("[{0}]", _entity.OutputName());
            var sets = Process.OutputRecordsExist ?
                new FieldSqlWriter(_entity.Fields).FieldType(FieldType.ForeignKey).AddBatchId(false).Alias().Set(master, source).Write(",\r\n    ") :
                new FieldSqlWriter(_entity.Fields).FieldType(FieldType.ForeignKey).Alias().Set(master, source).Write(",\r\n    ");


            builder.AppendFormat("UPDATE {0}\r\n", master);
            builder.AppendFormat("SET {0}\r\n", sets);
            builder.AppendFormat("FROM {0}\r\n", source);

            foreach (var relationship in _entity.RelationshipToMaster) {
                var left = string.Format("[{0}]", relationship.LeftEntity.OutputName());
                var right = string.Format("[{0}]", relationship.RightEntity.OutputName());
                var join = string.Join(" AND ", relationship.Join.Select(j => string.Format("{0}.[{1}] = {2}.[{3}]", left, j.LeftField.Alias, right, j.RightField.Alias)));
                builder.AppendFormat("INNER JOIN {0} ON ({1})\r\n", left, join);
            }

            builder.AppendFormat("WHERE {0}.[TflBatchId] = @TflBatchId", source);
            return builder.ToString();
        }
    }
}