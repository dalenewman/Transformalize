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
using Transformalize.Libs.Dapper;

namespace Transformalize.Main.Providers.SqlServer {

    public class SqlServerStarViewWriter : IViewWriter {

        public void Drop(Process process) {

            if (!process.StarEnabled)
                return;

            using (var cn = process.OutputConnection.GetConnection()) {
                cn.Open();

                var sql = DropSql(process);
                process.Logger.Debug(sql);
                cn.Execute(sql);
                process.Logger.Debug("Dropped Output {0}.", process.Name);
            }
        }

        public void Create(Process process) {

            if (!process.StarEnabled)
                return;

            Drop(process);
            using (var cn = process.OutputConnection.GetConnection()) {
                cn.Open();
                var sql = CreateSql(process);

                process.Logger.Debug(sql);
                cn.Execute(sql);
                process.Logger.Debug("Created Output {0}.", process.Star);
            }
        }

        private static string DropSql(Process process) {
            const string format = @"
            IF EXISTS (
	            SELECT *
	            FROM INFORMATION_SCHEMA.VIEWS
	            WHERE TABLE_NAME = '{0}'
            )
	            DROP VIEW [{0}];";
            return string.Format(format, process.Star);
        }

        public string CreateSql(Process process) {
            var builder = new StringBuilder();
            builder.AppendFormat("CREATE VIEW {0} AS\r\n", process.OutputConnection.Enclose(process.Star));
            builder.AppendFormat("SELECT\r\n    d.TflKey,\r\n    d.TflBatchId,\r\n    b.TflUpdate,\r\n");

            var l = process.OutputConnection.L;
            var r = process.OutputConnection.R;

            var typedFields = new StarFields(process).TypedFields();
            builder.AppendLine(string.Concat(new FieldSqlWriter(typedFields[StarFieldType.Master]).Alias(l, r).PrependEntityOutput(process.OutputConnection, "d").Prepend("    ").Write(",\r\n"), ","));

            if (typedFields[StarFieldType.Foreign].Any())
                builder.AppendLine(string.Concat(new FieldSqlWriter(typedFields[StarFieldType.Foreign]).Alias(l, r).PrependEntityOutput(process.OutputConnection, "d").IsNull().ToAlias(l, r).Prepend("    ").Write(",\r\n"), ","));

            if (typedFields[StarFieldType.Other].Any())
                builder.AppendLine(string.Concat(new FieldSqlWriter(typedFields[StarFieldType.Other]).Alias(l, r).PrependEntityOutput(process.OutputConnection).IsNull().ToAlias(l, r).Prepend("    ").Write(",\r\n"), ","));

            builder.TrimEnd("\r\n,");
            builder.AppendLine();
            builder.AppendFormat("FROM {0} d\r\n", process.OutputConnection.Enclose(process.MasterEntity.OutputName()));
            builder.AppendFormat("INNER JOIN TflBatch b ON (d.TflBatchId = b.TflBatchId AND b.ProcessName = '{0}')\r\n", process.Name);

            foreach (var entity in process.Entities.Where(e => !e.IsMaster())) {
                builder.AppendFormat("LEFT OUTER JOIN {0} ON (", entity.OutputName());

                foreach (var join in entity.RelationshipToMaster.First().Join.ToArray()) {
                    builder.AppendFormat(
                        "d.{0} = {1}.{2} AND ",
                        process.OutputConnection.Enclose(join.LeftField.Alias),
                        entity.OutputName(),
                        process.OutputConnection.Enclose(join.RightField.Alias));
                }

                builder.TrimEnd(" AND ");
                builder.AppendLine(")");
            }

            builder.Append(";");
            return builder.ToString();
        }
    }
}