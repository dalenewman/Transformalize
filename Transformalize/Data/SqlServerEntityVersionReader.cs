using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using Transformalize.Model;

namespace Transformalize.Data {
    public class SqlServerEntityVersionReader : IEntityVersionReader {
        private object _end;
        private object _begin;
        private readonly Entity _entity;
        public bool HasRows { get; private set; }
        public bool IsRange { get; private set; }

        public SqlServerEntityVersionReader(Entity entity) {
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
                FROM [TflBatch]
                WHERE [TflBatchId] = (
	                SELECT [TflBatchId] = MAX([TflBatchId])
	                FROM [TflBatch]
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
            var field = _entity.Version.SimpleType.Replace("byte[]", "binary") + "Version";
            using (var reader = GetBeginVersionReader(field)) {
                IsRange = reader.HasRows;
                if (!IsRange)
                    return null;
                reader.Read();
                _begin = reader.GetValue(0);
                return _begin;
            }
        }

        public object GetEndVersion() {
            using (var reader = GetEndVersionReader()) {
                HasRows = reader.HasRows;
                if (!HasRows)
                    return null;
                reader.Read();
                _end = reader.GetValue(0);
                return _end;
            }
        }

        private IEnumerable<byte> ObjectToByteArray(object obj) {
            if (obj == null)
                return null;
            var formatter = new BinaryFormatter();
            var memory = new MemoryStream();
            formatter.Serialize(memory, obj);
            return memory.ToArray();
        }

        public bool BeginAndEndAreEqual() {
            if (IsRange) {
                if (_entity.Version.SimpleType.Equals("byte[]")) {
                    var beginBytes = ObjectToByteArray(_begin);
                    var endBytes = ObjectToByteArray(_end);
                    return beginBytes.SequenceEqual(endBytes);
                }
                return _begin.Equals(_end);
            }
            return false;
        }
    }
}