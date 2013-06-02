using System.Data;
using System.Data.SqlClient;
using Transformalize.Model;

namespace Transformalize.Readers {
    public class VersionReader : IVersionReader {
        private readonly Entity _entity;
        public bool HasRows { get; private set; }
        public bool IsRange { get; private set; }

        public VersionReader(Entity entity) {
            _entity = entity;
        }

        private SqlDataReader GetEndVersionReader() {
            var sql = string.Format("SELECT MAX([{0}]) AS [{0}] FROM [{1}].[{2}];", _entity.Version.Name, _entity.Schema, _entity.Name);
            var cn = new SqlConnection(_entity.InputConnection.ConnectionString);
            cn.Open();
            var command = new SqlCommand(sql, cn);
            return command.ExecuteReader(CommandBehavior.CloseConnection & CommandBehavior.SingleResult);
        }

        private SqlDataReader GetBeginVersionReader(string field) {

            var sql = string.Format(@"
                SELECT [{0}]
                FROM [EntityTracker]
                WHERE [EntityTrackerKey] = (
	                SELECT [EntityTrackerKey] = MAX([EntityTrackerKey])
	                FROM [EntityTracker]
	                WHERE [ProcessName] = @ProcessName 
                    AND [EntityName] = @EntityName
                );
            ", field);

            var cn = new SqlConnection(_entity.OutputConnection.ConnectionString);
            cn.Open();
            var command = new SqlCommand(sql, cn);
            command.Parameters.Add(new SqlParameter("@ProcessName", _entity.ProcessName));
            command.Parameters.Add(new SqlParameter("@EntityName", _entity.Name));
            return command.ExecuteReader(CommandBehavior.CloseConnection & CommandBehavior.SingleResult);
        }

        public object GetBeginVersion() {
            var field = _entity.Version.SimpleType().Replace("byte[]", "binary") + "Version";
            using (var reader = GetBeginVersionReader(field)) {
                IsRange = reader.HasRows;
                if (!IsRange)
                    return null;
                reader.Read();
                return reader.GetValue(0);
            }
        }

        public object GetEndVersion() {
            using (var reader = GetEndVersionReader()) {
                HasRows = reader.HasRows;
                if (!HasRows)
                    return null;
                reader.Read();
                return reader.GetValue(0);
            }
        }
    }
}