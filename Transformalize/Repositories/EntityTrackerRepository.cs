using System.Data.SqlClient;
using Transformalize.Model;
using Transformalize.Rhino.Etl.Core;

namespace Transformalize.Repositories {

    public class EntityTrackerRepository : WithLoggingMixin {

        private readonly Process _process;

        public EntityTrackerRepository(Process process) {
            _process = process;
        }

        private static void Execute(string sql, string connectionString) {
            using (var cn = new SqlConnection(connectionString)) {
                cn.Open();
                var command = new SqlCommand(sql, cn);
                command.ExecuteNonQuery();
            }
        }

        public string CreateEntityTrackerSql() {
            return @"
                CREATE TABLE EntityTracker(
                    EntityTrackerKey INT NOT NULL IDENTITY(1,1),
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
					CONSTRAINT Pk_EntityTracker_EntityTrackerKey PRIMARY KEY (
						EntityTrackerKey
					)
                );

                CREATE INDEX Ix_EntityTracker_ProcessName_EntityName__EntityTrackerKey ON EntityTracker (
                    ProcessName ASC,
                    EntityName ASC
                ) INCLUDE (EntityTrackerKey);
            ";
        }

        public string TruncateEntityTrackerSql() {
            return SqlTemplates.TruncateTable("EntityTracker");
        }

        public string DropEntityTrackerSql() {
            return SqlTemplates.DropTable("EntityTracker");
        }

        public void InitializeEntityTracker() {
            Execute(TruncateEntityTrackerSql(), _process.OutputConnection.ConnectionString);
            Info("{0} | Truncated EntityTracker.", _process.Name);
            Execute(DropEntityTrackerSql(), _process.OutputConnection.ConnectionString);
            Info("{0} | Dropped EntityTracker.", _process.Name);
            Execute(CreateEntityTrackerSql(), _process.OutputConnection.ConnectionString);
            Info("{0} | Created EntityTracker.", _process.Name);
        }
    }
}
