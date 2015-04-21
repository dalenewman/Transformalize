using System.Linq;
using System.Text;
using Transformalize.Extensions;
using Transformalize.Libs.Dapper;
using Transformalize.Logging;

namespace Transformalize.Main.Providers.SqlServer {

    public class SqlServerViewWriter : IViewWriter {

        public void Create(Process process) {

            if (!process.ViewEnabled || process.Relationships.Count == 0)
                return;

            var outputSql = string.Format("CREATE VIEW {0} AS {1}", process.OutputConnection.Enclose(process.View), ViewSql(process));
            TflLogger.Debug(process.Name, string.Empty, outputSql);
            using (var cn = process.OutputConnection.GetConnection()) {
                cn.Open();
                cn.Execute(outputSql);
            }
        }

        public void Drop(Process process) {

            if (!process.ViewEnabled || process.Relationships.Count == 0)
                return;

            using (var cn = process.OutputConnection.GetConnection()) {
                cn.Open();
                if (cn.Query("SELECT TOP(1) TABLE_NAME FROM INFORMATION_SCHEMA.VIEWS WHERE TABLE_NAME = @View;", new { process.View }).Any()) {
                    cn.Execute(string.Format("DROP VIEW {0};", process.OutputConnection.Enclose(process.View)));
                }
            }
        }

        public string ViewSql(Process process) {

            const string fieldSpacer = ",\r\n    ";
            var builder = new StringBuilder();
            var master = process.MasterEntity;
            var l = process.OutputConnection.L;
            var r = process.OutputConnection.R;

            builder.AppendLine("SELECT");
            builder.Append("    ");
            builder.Append(new FieldSqlWriter(master.OutputFields()).Alias(l, r).Prepend("m.").Write(fieldSpacer));

            foreach (var rel in process.Relationships) {
                var joinFields = rel.Fields();
                foreach (var field in rel.RightEntity.OutputFields()) {
                    if (!joinFields.HaveField(field.Alias)) {
                        builder.Append(fieldSpacer);
                        builder.Append(new FieldSqlWriter(new Fields(field)).Alias(l, r).Prepend("r" + rel.RightEntity.Index + ".").Write(fieldSpacer));
                    }
                }
            }

            builder.AppendLine();
            builder.Append("FROM ");
            builder.Append(l);
            builder.Append(master.OutputName());
            builder.Append(r);
            builder.Append(" m");

            foreach (var rel in process.Relationships) {
                builder.AppendLine();
                builder.Append("LEFT OUTER JOIN ");
                if (rel.RightEntity.IsMaster()) {
                    builder.Append("m");
                } else {
                    builder.Append(l);
                    builder.Append(rel.RightEntity.OutputName());
                    builder.Append(r);
                }
                builder.Append(" ");
                builder.Append("r");
                builder.Append(rel.RightEntity.Index);
                builder.Append(" ON (");
                foreach (var j in rel.Join) {
                    if (rel.LeftEntity.IsMaster()) {
                        builder.Append("m");
                    } else {
                        builder.Append("r");
                        builder.Append(rel.LeftEntity.Index);
                    }
                    builder.Append(".");
                    builder.Append(l);
                    builder.Append(j.LeftField.Alias);
                    builder.Append(r);
                    builder.Append(" = ");
                    builder.Append("r");
                    builder.Append(rel.RightEntity.Index);
                    builder.Append(".");
                    builder.Append(l);
                    builder.Append(j.RightField.Alias);
                    builder.Append(r);
                    builder.Append(" AND ");
                }
                builder.TrimEnd(" AND ");
                builder.Append(")");
            }

            builder.Append(";");
            return builder.ToString();

        }

    }
}