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
using Transformalize.Core;
using Transformalize.Core.Entity_;
using Transformalize.Core.Field_;
using Transformalize.Core.Fields_;
using Transformalize.Core.Process_;
using Transformalize.Libs.NLog;
using Transformalize.Libs.Rhino.Etl.Core;
using Transformalize.Libs.Rhino.Etl.Core.Operations;
using Transformalize.Providers;
using Transformalize.Providers.SqlServer;
using System.Linq;

namespace Transformalize.Operations {

    public class EntityCreate : AbstractOperation {
        
        private readonly Entity _entity;
        private readonly IEntityExists _entityExists;
        private readonly FieldSqlWriter _writer;

        public EntityCreate(Entity entity, Process process, IEntityExists entityExists = null) {
            _entity = entity;
           
            _writer = _entity.IsMaster() ?
                new FieldSqlWriter(entity.All, process.CalculatedFields, entity.CalculatedFields, GetRelationshipFields(process)) :
                new FieldSqlWriter(entity.All, entity.CalculatedFields);

            _entityExists = entityExists ?? new SqlServerEntityExists();
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            CreateEntity();
            return rows;
        }

        private void CreateEntity() {
            if (_entityExists.OutputExists(_entity)) return;

            var primaryKey = _entity.IsMaster()
                ? _writer.FieldType(FieldType.MasterKey).Alias().Asc().Values()
                : _writer.FieldType(FieldType.PrimaryKey).Alias().Asc().Values();
            var defs = _writer.Reload().ExpandXml().AddSurrogateKey().AddBatchId().Output().Alias().DataType().AppendIf(" NOT NULL", FieldType.MasterKey, FieldType.PrimaryKey).Values();
            var sql = SqlTemplates.CreateTable(_entity.OutputName(), defs, primaryKey, ignoreDups: true);

            Debug(sql);

            using (var cn = new SqlConnection(_entity.OutputConnection.ConnectionString)) {
                cn.Open();
                var cmd = new SqlCommand(sql, cn);
                cmd.ExecuteNonQuery();
                Info("Initialized {0} in {1} on {2}.", _entity.OutputName(), _entity.OutputConnection.Database, _entity.OutputConnection.Server);
            }
        }

        private IFields GetRelationshipFields(Process process)
        {
            var relationships = process.Relationships.Where(r => r.LeftEntity.Alias != _entity.Alias && r.RightEntity.Alias != _entity.Alias).ToArray();
            var fields = new Fields();
            if (relationships.Any())
            {
                foreach (var relationship in relationships)
                {
                    var leftSide = relationship.LeftEntity.RelationshipToMaster.Count();
                    var rightSide = relationship.RightEntity.RelationshipToMaster.Count();
                    if (leftSide <= rightSide)
                    {
                        foreach (var join in relationship.Join)
                        {
                            fields.Add(join.LeftField);
                        }
                    }
                    else
                    {
                        foreach (var join in relationship.Join)
                        {
                            fields.Add(join.RightField);
                        }
                    }
                }
            }
            return fields;
        }
    }
}