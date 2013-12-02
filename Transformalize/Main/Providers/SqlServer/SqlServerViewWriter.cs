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

using System.Linq;
using System.Text;
using Transformalize.Extensions;
using Transformalize.Libs.NLog;
using Transformalize.Libs.Dapper;

namespace Transformalize.Main.Providers.SqlServer {
    public class SqlServerViewWriter : IViewWriter {
        private readonly Logger _log = LogManager.GetCurrentClassLogger();
        private readonly Entity _masterEntity;
        private readonly Process _process;

        public SqlServerViewWriter(Process process) {
            _process = process;
            _masterEntity = _process.MasterEntity;
        }

        public void Drop() {
            using (var cn = _process.OutputConnection.GetConnection()) {
                cn.Open();

                var sql = DropSql();
                _log.Debug(sql);
                cn.Execute(sql);
                _log.Debug("Dropped Output {0}.", _process.Star);
            }
        }

        public void Create() {
            Drop();
            using (var cn = _process.OutputConnection.GetConnection()) {
                cn.Open();
                var sql = CreateSql();

                _log.Debug(sql);
                cn.Execute(sql);
                _log.Debug("Created Output {0}.", _process.Star);
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
            return string.Format(format, _process.Star);
        }

        public string CreateSql() {
            var provider = _process.OutputConnection.Provider;
            var builder = new StringBuilder();
            builder.AppendFormat("CREATE VIEW {0} AS\r\n", provider.Enclose(_process.Star));
            builder.AppendFormat("SELECT\r\n    d.TflKey,\r\n    d.TflBatchId,\r\n    b.TflUpdate,\r\n");

            var typedFields = new StarFields(_process).TypedFields();
            builder.AppendLine(string.Concat(new FieldSqlWriter(typedFields[StarFieldType.Master]).Alias(provider).PrependEntityOutput(provider, "d").Prepend("    ").Write(",\r\n"), ","));

            if (typedFields[StarFieldType.Foreign].Any())
                builder.AppendLine(string.Concat(new FieldSqlWriter(typedFields[StarFieldType.Foreign]).Alias(provider).PrependEntityOutput(provider, "d").IsNull().ToAlias(provider).Prepend("    ").Write(",\r\n"), ","));

            if (typedFields[StarFieldType.Other].Any())
                builder.AppendLine(string.Concat(new FieldSqlWriter(typedFields[StarFieldType.Other]).Alias(provider).PrependEntityOutput(provider).IsNull().ToAlias(provider).Prepend("    ").Write(",\r\n"), ","));

            builder.TrimEnd("\r\n,");
            builder.AppendLine();
            builder.AppendFormat("FROM {0} d\r\n", provider.Enclose(_masterEntity.OutputName()));
            builder.AppendFormat("INNER JOIN TflBatch b ON (d.TflBatchId = b.TflBatchId AND b.ProcessName = '{0}')\r\n", _process.Name);

            foreach (var entity in _process.Entities.Where(e => !e.IsMaster())) {
                builder.AppendFormat("LEFT OUTER JOIN {0} ON (", entity.OutputName());

                foreach (var join in entity.RelationshipToMaster.First().Join.ToArray()) {
                    builder.AppendFormat("{0}.{1} = {2}.{3} AND ", _masterEntity.OutputName(), provider.Enclose(join.LeftField.Alias), entity.OutputName(), provider.Enclose(join.RightField.Alias));
                }

                builder.TrimEnd(" AND ");
                builder.AppendLine(")");
            }

            builder.Append(";");
            return builder.ToString();
        }
    }
}