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

using System;
using System.Data.SqlClient;
using Transformalize.Libs.NLog;

namespace Transformalize.Main.Providers.SqlServer {
    public class SqlServerEntityVersionWriter : IEntityVersionWriter {
        private readonly Entity _entity;
        private readonly Logger _log = LogManager.GetCurrentClassLogger();

        public SqlServerEntityVersionWriter(Entity entity) {
            _entity = entity;
        }

        public void WriteEndVersion() {

            if (_entity.Inserts > 0) {
                using (var cn = new SqlConnection(_entity.OutputConnection.ConnectionString)) {
                    cn.Open();
                    var command = new SqlCommand(PrepareSql(), cn);

                    command.Parameters.Add(new SqlParameter("@TflBatchId", _entity.TflBatchId));
                    command.Parameters.Add(new SqlParameter("@ProcessName", _entity.ProcessName));
                    command.Parameters.Add(new SqlParameter("@EntityName", _entity.Alias));
                    command.Parameters.Add(new SqlParameter("@TflUpdate", DateTime.Now));
                    command.Parameters.Add(new SqlParameter("@Inserts", _entity.Inserts));
                    command.Parameters.Add(new SqlParameter("@Updates", _entity.Updates));
                    command.Parameters.Add(new SqlParameter("@Deletes", _entity.Deletes));

                    if (_entity.Version != null) {
                        var end = new ConversionFactory().Convert(_entity.End, _entity.Version.SimpleType);
                        command.Parameters.Add(new SqlParameter("@End", end));
                    }

                    _log.Debug(command.CommandText);

                    command.ExecuteNonQuery();
                }
            }

            _log.Info("Processed {0} rows in {1}", _entity.Inserts, _entity.Alias);
        }

        private string PrepareSql() {
            if (_entity.Version == null) {
                return @"
                    INSERT INTO [TflBatch](TflBatchId, ProcessName, EntityName, TflUpdate, Inserts, Updates, Deletes)
                    VALUES(@TflBatchId, @ProcessName, @EntityName, @TflUpdate, @Inserts, @Updates, @Deletes);
                ";
            }

            var field = _entity.Version.SimpleType.Replace("rowversion", "Binary").Replace("byte[]", "Binary") + "Version";
            return string.Format(@"
                INSERT INTO [TflBatch](TflBatchId, ProcessName, EntityName, [{0}], TflUpdate, Inserts, Updates, Deletes)
                VALUES(@TflBatchId, @ProcessName, @EntityName, @End, @TflUpdate, @Inserts, @Updates, @Deletes);
            ", field);
        }
    }
}