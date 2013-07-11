using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Transformalize.Model;
using Transformalize.Rhino.Etl.Core;

namespace Transformalize.Data
{
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

                Info("{0} | Created View {1}", _process.Name, _process.View);
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
            builder.AppendFormat("SELECT\r\n    [{0}].[TflKey],\r\n    [{0}].[TflBatchId],\r\n", _masterEntity.OutputName());
            foreach (var pair in _process.Entities) {
                var entity = pair.Value;
                if (entity.IsMaster()) {
                    builder.AppendLine(string.Concat(new FieldSqlWriter(entity.PrimaryKey, entity.Fields, _process.Results).ExpandXml().Output().Alias().Prepend(string.Concat("    [", entity.OutputName(), "].")).Write(",\r\n"), ","));
                }
                else {
                    if (entity.Fields.Any(f => f.Value.FieldType == FieldType.ForeignKey)) {
                        builder.AppendLine(string.Concat(new FieldSqlWriter(entity.Fields).ExpandXml().Output().FieldType(FieldType.ForeignKey).Alias().Prepend(string.Concat("    [", _masterEntity.OutputName(), "].")).Write(",\r\n"), ","));
                    }
                    builder.AppendLine(string.Concat(new FieldSqlWriter(entity.Fields).ExpandXml().Output().FieldType(FieldType.Field, FieldType.Version, FieldType.Xml).Alias().Prepend(string.Concat("    [", entity.OutputName(), "].")).Write(",\r\n"), ","));
                }
            }
            builder.TrimEnd("\r\n,");
            builder.AppendLine();
            builder.AppendFormat("FROM [{0}]\r\n", _masterEntity.OutputName());

            foreach (var pair in _process.Entities.Where(e => !e.Value.IsMaster())) {
                var entity = pair.Value;
                builder.AppendFormat("INNER JOIN [{0}] ON (", entity.OutputName());
                foreach (var pk in entity.PrimaryKey) {
                    builder.AppendFormat("[{0}].[{1}] = [{2}].[{1}] AND ", _masterEntity.OutputName(), pk.Value.Alias, entity.OutputName());
                }
                builder.TrimEnd(" AND ");
                builder.AppendLine(")");
            }
            builder.Append(";");
            return builder.ToString();
        }
    }
}