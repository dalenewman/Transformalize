using System;
using System.Data.SqlClient;
using Transformalize.Model;
using Transformalize.Rhino.Etl.Core;

namespace Transformalize.Writers {
    public class VersionWriter : WithLoggingMixin, IVersionWriter {

        private readonly Entity _entity;

        public VersionWriter(Entity entity) {
            _entity = entity;
        }

        public void WriteEndVersion(object end, int count) {

            var field = _entity.Version.SimpleType.Replace("byte[]", "binary") + "Version";
            var sql = string.Format(@"
                INSERT INTO [EntityTracker](ProcessName, EntityName, [{0}], LastProcessedDate, Rows)
                VALUES(@ProcessName, @EntityName, @End, @Date, @Count);
            ", field);

            using (var cn = new SqlConnection(_entity.OutputConnection.ConnectionString)) {
                cn.Open();
                var command = new SqlCommand(sql, cn);
                command.Parameters.Add(new SqlParameter("@ProcessName", _entity.ProcessName));
                command.Parameters.Add(new SqlParameter("@EntityName", _entity.Name));
                command.Parameters.Add(new SqlParameter("@End", end));
                command.Parameters.Add(new SqlParameter("@Date", DateTime.Now));
                command.Parameters.Add(new SqlParameter("@Count", count));
                command.ExecuteNonQuery();
            }

            Info("{0} | Closing {1} (using {2}).", _entity.ProcessName, _entity.Name, field);
        }
    }
}