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
using System.Data;
using Transformalize.Libs.NLog;
using Transformalize.Extensions;

namespace Transformalize.Main.Providers {
    public class DatabaseEntityVersionWriter : IEntityVersionWriter {
        private readonly Process _process;
        private readonly Entity _entity;
        private readonly Logger _log = LogManager.GetLogger(string.Empty);

        public DatabaseEntityVersionWriter(Process process, Entity entity) {
            _process = process;
            _entity = entity;
        }

        public void WriteEndVersion(AbstractConnection input) {

            if (_entity.Inserts + _entity.Updates > 0) {
                using (var cn = _process.OutputConnection.GetConnection()) {
                    cn.Open();

                    var cmd = cn.CreateCommand();
                    cmd.CommandText = PrepareSql(input);
                    cmd.CommandType = CommandType.Text;

                    _process.OutputConnection.AddParameter(cmd, "@TflBatchId", _entity.TflBatchId);
                    _process.OutputConnection.AddParameter(cmd, "@ProcessName", _process.Name);
                    _process.OutputConnection.AddParameter(cmd, "@EntityName", _entity.Alias);
                    _process.OutputConnection.AddParameter(cmd, "@TflUpdate", DateTime.Now);
                    _process.OutputConnection.AddParameter(cmd, "@Inserts", _entity.Inserts);
                    _process.OutputConnection.AddParameter(cmd, "@Updates", _entity.Updates);
                    _process.OutputConnection.AddParameter(cmd, "@Deletes", _entity.Deletes);

                    if (input.CanDetectChanges(_entity)) {
                        var end = new DefaultFactory().Convert(_entity.End, _entity.Version.SimpleType);
                        _process.OutputConnection.AddParameter(cmd, "@End", end);
                    }

                    _log.Debug(cmd.CommandText);
                    cmd.ExecuteNonQuery();
                }
            }
            if (_entity.Delete) {
                _log.Info("Processed {0} insert{1}, {2} update{3}, and {4} delete{5} in {6}.", _entity.Inserts, _entity.Inserts.Plural(), _entity.Updates, _entity.Updates.Plural(), _entity.Deletes, _entity.Deletes.Plural(), _entity.Alias);
            } else {
                _log.Info("Processed {0} insert{1}, and {2} update{3} in {4}.", _entity.Inserts, _entity.Inserts.Plural(), _entity.Updates, _entity.Updates.Plural(), _entity.Alias);
            }

        }

        private string PrepareSql(AbstractConnection input) {
            if (!input.CanDetectChanges(_entity)) {
                return @"
                    INSERT INTO TflBatch(TflBatchId, ProcessName, EntityName, TflUpdate, Inserts, Updates, Deletes)
                    VALUES(@TflBatchId, @ProcessName, @EntityName, @TflUpdate, @Inserts, @Updates, @Deletes);
                ";
            }

            var field = _entity.Version.SimpleType.Replace("rowversion", "Binary").Replace("byte[]", "Binary") + "Version";
            return string.Format(@"
                INSERT INTO TflBatch(TflBatchId, ProcessName, EntityName, {0}, TflUpdate, Inserts, Updates, Deletes)
                VALUES(@TflBatchId, @ProcessName, @EntityName, @End, @TflUpdate, @Inserts, @Updates, @Deletes);
            ", field);
        }
    }
}