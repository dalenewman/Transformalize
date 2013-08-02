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
using System.Linq;
using System.Text;
using Transformalize.Extensions;
using Transformalize.Libs.Rhino.Etl.Core;
using Transformalize.Model;

namespace Transformalize.Data.SqlServer {
    public class SqlServerViewWriter : WithLoggingMixin, IViewWriter {
        private readonly Process _process;
        private readonly Entity _masterEntity;

        public SqlServerViewWriter(ref Process process) {
            _process = process;
            _masterEntity = _process.MasterEntity;
        }

        public void Drop() {
            using (var cn = new SqlConnection(_masterEntity.OutputConnection.ConnectionString)) {
                cn.Open();
                var dropCommand = new SqlCommand(DropSql(), cn);
                dropCommand.ExecuteNonQuery();
            }
        }

        public void Create() {
            Drop();
            using (var cn = new SqlConnection(_masterEntity.OutputConnection.ConnectionString)) {
                cn.Open();

                var createCommand = new SqlCommand(CreateSql(), cn);
                createCommand.ExecuteNonQuery();

                Debug("{0} | Created Output {1}", _process.Name, _process.View);
            }
        }

        private string DropSql() {
            const string format = @"IF EXISTS (
	SELECT *
	FROM INFORMATION_SCHEMA.VIEWS
	WHERE TABLE_SCHEMA = 'dbo'
	AND TABLE_NAME = '{0}'
)
	DROP VIEW [{0}];";
            return string.Format(format, _process.View);
        }

        public string CreateSql() {
            var builder = new StringBuilder();
            builder.AppendFormat("CREATE VIEW [{0}] AS\r\n", _process.View);
            builder.AppendFormat("SELECT\r\n    [{0}].[TflKey],\r\n    [{0}].[TflBatchId],\r\n    b.[TflUpdate],\r\n", _masterEntity.OutputName());
            foreach (var entity in _process.Entities) {
                if (entity.IsMaster()) {
                    builder.AppendLine(string.Concat(new FieldSqlWriter(entity.PrimaryKey, entity.Fields, _process.Results).ExpandXml().Output().Alias().Prepend(string.Concat("    [", entity.OutputName(), "].")).Write(",\r\n"), ","));
                }
                else {
                    if (entity.Fields.Any(f => f.Value.FieldType.HasFlag(FieldType.ForeignKey))) {
                        builder.AppendLine(string.Concat(new FieldSqlWriter(entity.Fields).ExpandXml().Output().FieldType(FieldType.ForeignKey).Alias().Prepend(string.Concat("    [", _masterEntity.OutputName(), "].")).Write(",\r\n"), ","));
                    }
                    builder.AppendLine(string.Concat(new FieldSqlWriter(entity.Fields).ExpandXml().Output().FieldType(FieldType.Field, FieldType.Version, FieldType.Xml).Alias().Prepend(string.Concat("    [", entity.OutputName(), "].")).Write(",\r\n"), ","));
                }
            }
            builder.TrimEnd("\r\n,");
            builder.AppendLine();
            builder.AppendFormat("FROM [{0}]\r\n", _masterEntity.OutputName());
            builder.AppendFormat("INNER JOIN [TflBatch] b ON ([{0}].TflBatchId = b.TflBatchId)\r\n", _masterEntity.OutputName());

            foreach (var entity in _process.Entities.Where(e => !e.IsMaster())) {
                builder.AppendFormat("INNER JOIN [{0}] ON (", entity.OutputName());

                foreach (var join in entity.RelationshipToMaster.First().Join.ToArray()) {
                    builder.AppendFormat("[{0}].[{1}] = [{2}].[{3}] AND ", _masterEntity.OutputName(), join.LeftField.Alias, entity.OutputName(), join.RightField.Alias);
                }
                //foreach (var pk in entity.PrimaryKey) {
                //    builder.AppendFormat("[{0}].[{1}] = [{2}].[{1}] AND ", _masterEntity.OutputName(), pk.Value.Alias, entity.OutputName());
                //}
                builder.TrimEnd(" AND ");
                builder.AppendLine(")");
            }



            builder.Append(";");
            return builder.ToString();
        }
    }
}