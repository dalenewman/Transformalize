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
using Transformalize.Data;
using Transformalize.Model;
using Transformalize.Rhino.Etl.Core;
using Transformalize.Rhino.Etl.Core.Operations;

namespace Transformalize.Operations {

    public class EntityCreate : AbstractOperation {
        
        private readonly Entity _entity;
        private readonly IEntityExists _entityExists;
        private readonly FieldSqlWriter _writer;

        public EntityCreate(Entity entity, Process process, IEntityExists entityExists = null) {
            _entity = entity;
            _writer = entity.IsMaster() ?
                new FieldSqlWriter(entity.All, process.Results, process.RelatedKeys.ToDictionary(k => k.Alias, v => v)) :
                new FieldSqlWriter(entity.All);
            _entityExists = entityExists ?? new SqlServerEntityExists();
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            CreateEntity();
            return rows;
        }

        private void CreateEntity() {
            if (_entityExists.OutputExists(_entity)) return;

            var primaryKey = _writer.FieldType(FieldType.MasterKey, FieldType.PrimaryKey).Alias().Asc().Values();
            var defs = _writer.Reload().ExpandXml().AddSurrogateKey().AddBatchId().Output().Alias().DataType().AppendIf(" NOT NULL", FieldType.MasterKey, FieldType.PrimaryKey).Values();
            var sql = SqlTemplates.CreateTable(_entity.OutputName(), defs, primaryKey, ignoreDups: true);

            Debug(sql);

            using (var cn = new SqlConnection(_entity.OutputConnection.ConnectionString)) {
                cn.Open();
                var cmd = new SqlCommand(sql, cn);
                cmd.ExecuteNonQuery();
                Info("{0} | Initialized {1} in {2} on {3}.", _entity.ProcessName, _entity.OutputName(), _entity.OutputConnection.Database, _entity.OutputConnection.Server);
            }
        }
    }
}