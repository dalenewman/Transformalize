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

using System.Data;
using Transformalize.Libs.Rhino.Etl;

namespace Transformalize.Main.Providers.SqlServer {
    public class SqlServerTflWriter : WithLoggingMixin, ITflWriter {

        public void Initialize(Process process) {

            if (!new SqlServerTableExists(process.OutputConnection).Exists("dbo", "TflBatch")) {
                Execute(process.OutputConnection, CreateTable());
                Execute(process.OutputConnection, CreateIndex());
                Debug("Created TflBatch.");
            }

            var sql = string.Format("DELETE FROM TflBatch WHERE ProcessName = '{0}';", process.Name);
            Debug(sql);
            Execute(process.OutputConnection, sql);

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

        public string CreateIndex()
        {
            return @"
                CREATE INDEX Ix_TflBatch_ProcessName_EntityName__TflBatchId ON TflBatch (
                    ProcessName ASC,
                    EntityName ASC
                ) INCLUDE (TflBatchId);
            ";
        }

        public string CreateTable() {
            return @"
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
            ";
        }
    }
}