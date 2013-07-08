using System.Data.SqlClient;
using Transformalize.Model;
using Transformalize.Rhino.Etl.Core;

namespace Transformalize.Repositories {

    public class TflTrackerRepository : WithLoggingMixin {

        private readonly Process _process;

        public TflTrackerRepository(Process process) {
            _process = process;
        }

        private static void Execute(string sql, string connectionString) {
            using (var cn = new SqlConnection(connectionString)) {
                cn.Open();
                var command = new SqlCommand(sql, cn);
                command.ExecuteNonQuery();
            }
        }

        public string CreateTflTrackerSql() {
            return @"
                CREATE TABLE TflTracker(
                    TflTrackerKey INT NOT NULL IDENTITY(1,1),
					ProcessName NVARCHAR(100) NOT NULL,
	                EntityName NVARCHAR(100) NOT NULL,
	                BinaryVersion BINARY(8) NULL,
	                DateTimeVersion DATETIME NULL,
                    Int64Version BIGINT NULL,
                    Int32Version INT NULL,
                    Int16Version SMALLINT NULL,
                    ByteVersion TINYINT NULL,
	                LastProcessedDate DATETIME NOT NULL,
                    Rows INT NOT NULL,
					CONSTRAINT Pk_TflTracker_TflTrackerKey PRIMARY KEY (
						TflTrackerKey
					)
                );

                CREATE INDEX Ix_TflTracker_ProcessName_EntityName__TflTrackerKey ON TflTracker (
                    ProcessName ASC,
                    EntityName ASC
                ) INCLUDE (TflTrackerKey);
            ";
        }

        public void ResetProcess() {
            var sql = string.Format("DELETE FROM TflTracker WHERE ProcessName = '{0}';", _process.Name);
            Execute(sql, _process.MasterEntity.OutputConnection.ConnectionString);
        }

        public void ResetEntity(string entity) {
            var sql = string.Format("DELETE FROM TflTracker WHERE ProcessName = '{0}' AND EntityName = '{1}';", _process.Name, entity);
            Execute(sql, _process.MasterEntity.OutputConnection.ConnectionString);
        }

        public void Init() {
            var cs = _process.MasterEntity.OutputConnection.ConnectionString;

            Execute(SqlTemplates.TruncateTable("TflTracker"), cs);
            Info("{0} | Truncated TflTracker.", _process.Name);

            Execute(SqlTemplates.DropTable("TflTracker"), cs);
            Info("{0} | Dropped TflTracker.", _process.Name);

            Execute(CreateTflTrackerSql(), cs);
            Info("{0} | Created TflTracker.", _process.Name);
        }

    }
}
