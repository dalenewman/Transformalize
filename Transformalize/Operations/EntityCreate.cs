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
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Main;
using Transformalize.Main.Providers.SqlServer;
using Transformalize.Libs.Dapper;

namespace Transformalize.Operations {
    public class EntityCreate : AbstractOperation {
        private readonly Entity _entity;
        private readonly Process _process;
        private readonly FieldSqlWriter _writer;

        public EntityCreate(Entity entity, Process process) {
            Name = "Entity Create";
            _entity = entity;
            _process = process;

            _writer = _entity.IsMaster() ?
                new FieldSqlWriter(entity.Fields, process.CalculatedFields, entity.CalculatedFields, GetRelationshipFields(process)) :
                new FieldSqlWriter(entity.Fields, entity.CalculatedFields);

        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            CreateEntity();
            return rows;
        }

        private void CreateEntity() {
            var provider = _process.OutputConnection.Provider;

            if (_process.OutputConnection.Exists(_entity))
                return;

            var primaryKey = _writer.FieldType(_entity.IsMaster() ? FieldType.MasterKey : FieldType.PrimaryKey).Alias(provider).Asc().Values();
            var defs = _writer.Reload().AddSurrogateKey().AddBatchId().Output().Alias(provider).DataType().AppendIf(" NOT NULL", _entity.IsMaster() ? FieldType.MasterKey : FieldType.PrimaryKey).Values();

            var createSql = _process.OutputConnection.TableQueryWriter.CreateTable(_entity.OutputName(), defs, _entity.Schema);
            Debug(createSql);

            var indexSql = _process.OutputConnection.TableQueryWriter.AddUniqueClusteredIndex(_entity.OutputName(), _entity.Schema);
            Debug(indexSql);

            var keySql = _process.OutputConnection.TableQueryWriter.AddPrimaryKey(_entity.OutputName(), _entity.Schema, primaryKey);
            Debug(keySql);

            using (var cn = _process.OutputConnection.GetConnection()) {
                cn.Open();
                cn.Execute(createSql);
                cn.Execute(indexSql);
                cn.Execute(keySql);
                Info("Initialized {0} in {1} on {2}.", _entity.OutputName(), _process.OutputConnection.Database, _process.OutputConnection.Server);
            }
        }

        private Fields GetRelationshipFields(Process process) {
            var relationships = process.Relationships.Where(r => r.LeftEntity.Alias != _entity.Alias && r.RightEntity.Alias != _entity.Alias).ToArray();
            var fields = new Fields();
            if (relationships.Any()) {
                foreach (var relationship in relationships) {
                    var leftSide = relationship.LeftEntity.RelationshipToMaster.Count();
                    var rightSide = relationship.RightEntity.RelationshipToMaster.Count();
                    if (leftSide <= rightSide) {
                        foreach (var join in relationship.Join) {
                            fields.Add(join.LeftField);
                        }
                    } else {
                        foreach (var join in relationship.Join) {
                            fields.Add(join.RightField);
                        }
                    }
                }
            }
            return fields;
        }
    }
}