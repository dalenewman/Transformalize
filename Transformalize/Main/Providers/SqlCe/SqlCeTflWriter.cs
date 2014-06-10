using System.Data;
using Transformalize.Libs.Rhino.Etl;

namespace Transformalize.Main.Providers.SqlCe {

    public class SqlCeTflWriter : WithLoggingMixin, ITflWriter {

        public void Initialize(Process process) {

            if (!new SqlCeTableExists(process.OutputConnection).Exists("dbo", "TflBatch")) {
                Execute(process.OutputConnection, @"
                    CREATE TABLE [TflBatch](
                        [TflBatchId] INT NOT NULL,
					    [ProcessName] NVARCHAR(100) NOT NULL,
	                    [EntityName] NVARCHAR(100) NOT NULL,
	                    [BinaryVersion] BINARY(8) NULL,
                        [StringVersion] NVARCHAR(64) NULL,
	                    [DateTimeVersion] DATETIME NULL,
                        [Int64Version] BIGINT NULL,
                        [Int32Version] INT NULL,
                        [Int16Version] SMALLINT NULL,
                        [ByteVersion] TINYINT NULL,
	                    [TflUpdate] DATETIME NOT NULL,
                        [Inserts] BIGINT NOT NULL,
                        [Updates] BIGINT NOT NULL,
                        [Deletes] BIGINT NOT NULL,
					    CONSTRAINT Pk_TflBatch_TflBatchId PRIMARY KEY (
						    TflBatchId,
                            ProcessName
					    )
                    );
                ");
                Execute(process.OutputConnection, @"
                    CREATE INDEX Ix_TflBatch_ProcessName_EntityName_TflBatchId ON TflBatch (
                        ProcessName ASC,
                        EntityName ASC,
                        TflBatchId ASC
                    );
                ");
                Debug("Created TflBatch.");
            }

            Execute(process.OutputConnection, "DELETE FROM TflBatch WHERE ProcessName = '{0}';", process.Name);

            Info("Initialized TrAnSfOrMaLiZeR {0} connection.", process.OutputConnection.Name);
        }

        private static void Execute(AbstractConnection connection, string sqlFormat, params object[] values) {
            var sql = values.Length > 0 ? string.Format(sqlFormat, values) : sqlFormat;

            using (var cn = connection.GetConnection()) {
                cn.Open();
                var cmd = cn.CreateCommand();
                cmd.CommandText = sql;
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
            }
        }
    }
}