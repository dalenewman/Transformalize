using System.Data.SqlClient;
using Transformalize.Model;
using Transformalize.Rhino.Etl.Core;

namespace Transformalize.Data {
    public class SqlServerTflWriter : WithLoggingMixin, ITflWriter {

        private readonly Process _process;

        public SqlServerTflWriter(ref Process process) {
            _process = process;
        }

        private static void Execute(string sql, string connectionString) {
            using (var cn = new SqlConnection(connectionString)) {
                cn.Open();
                var command = new SqlCommand(sql, cn);
                command.ExecuteNonQuery();
            }
        }

        public string CreateSql() {
            return @"
                CREATE TABLE TflBatch(
                    TflBatchId INT NOT NULL,
					ProcessName NVARCHAR(100) NOT NULL,
	                EntityName NVARCHAR(100) NOT NULL,
	                BinaryVersion BINARY(8) NULL,
	                DateTimeVersion DATETIME NULL,
                    Int64Version BIGINT NULL,
                    Int32Version INT NULL,
                    Int16Version SMALLINT NULL,
                    ByteVersion TINYINT NULL,
	                LastProcessedDate DATETIME NOT NULL,
                    Rows BIGINT NOT NULL,
					CONSTRAINT Pk_TflBatch_TflBatchId PRIMARY KEY (
						TflBatchId
					)
                );

                CREATE INDEX Ix_TflBatch_ProcessName_EntityName__TflBatchId ON TflBatch (
                    ProcessName ASC,
                    EntityName ASC
                ) INCLUDE (TflBatchId);
            ";
        }

        public void Reset() {
            var sql = string.Format("DELETE FROM TflBatch WHERE ProcessName = '{0}';", _process.Name);
            Execute(sql, _process.MasterEntity.OutputConnection.ConnectionString);
        }

        public void Initialize() {
            var cs = _process.MasterEntity.OutputConnection.ConnectionString;

            Execute(SqlTemplates.TruncateTable("TflBatch"), cs);
            Info("{0} | Truncated TflBatch.", _process.Name);

            Execute(SqlTemplates.DropTable("TflBatch"), cs);
            Info("{0} | Dropped TflBatch.", _process.Name);

            Execute(CreateSql(), cs);
            Info("{0} | Created TflBatch.", _process.Name);
        }

    }
}
