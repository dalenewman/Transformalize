/*
Transformalize - Replicate, Transform, and Denormalize Your Data...
Copyright (C) 2013 Dale Newman

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Data.SqlClient;
using Transformalize.Core;
using Transformalize.Core.Entity_;
using Transformalize.Libs.NLog;

namespace Transformalize.Providers.SqlServer {

    public class SqlServerEntityVersionWriter : IEntityVersionWriter {

        private readonly Entity _entity;
        private readonly Logger _log = LogManager.GetCurrentClassLogger();

        public SqlServerEntityVersionWriter(Entity entity) {
            _entity = entity;
        }

        public void WriteEndVersion(object end, long count) {
            if (count > 0) {
                using (var cn = new SqlConnection(_entity.OutputConnection.ConnectionString)) {
                    cn.Open();
                    var command = new SqlCommand(PrepareSql(), cn);

                    command.Parameters.Add(new SqlParameter("@TflBatchId", _entity.TflBatchId));
                    command.Parameters.Add(new SqlParameter("@ProcessName", _entity.ProcessName));
                    command.Parameters.Add(new SqlParameter("@EntityName", _entity.Alias));
                    command.Parameters.Add(new SqlParameter("@TflUpdate", DateTime.Now));
                    command.Parameters.Add(new SqlParameter("@Count", count));

                    if(_entity.Version != null)
                        command.Parameters.Add(new SqlParameter("@End", new ConversionFactory().Convert(end, _entity.Version.SimpleType)));

                    _log.Debug(command.CommandText);

                    command.ExecuteNonQuery();
                }
            }

            _log.Info("Processed {0} rows in {1}", count, _entity.Alias);
        }

        private string PrepareSql() {

            if (_entity.Version == null)
            {
                return @"
                    INSERT INTO [TflBatch](TflBatchId, ProcessName, EntityName, TflUpdate, Rows)
                    VALUES(@TflBatchId, @ProcessName, @EntityName, @TflUpdate, @Count);
                ";
            }

            var field = _entity.Version.SimpleType.Replace("rowversion", "Binary").Replace("byte[]","Binary") + "Version";
            return string.Format(@"
                INSERT INTO [TflBatch](TflBatchId, ProcessName, EntityName, [{0}], TflUpdate, Rows)
                VALUES(@TflBatchId, @ProcessName, @EntityName, @End, @TflUpdate, @Count);
            ", field);
        }
    }
}