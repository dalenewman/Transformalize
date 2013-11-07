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
using System.Linq;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Main;

namespace Transformalize.Operations {
    public class ParametersExtract : InputCommandOperation {
        private readonly Process _process;
        private int[] _batchIds;
        private readonly string[] _keys;

        public ParametersExtract(Process process) : base(process.OutputConnection) {
            _process = process;
            UseTransaction = false;
            _keys = _process.Parameters.ToEnumerable().Where(p => !p.Value.HasValue()).Select(p => p.Key).ToArray();
        }

        private string PrepareSql() {
            string where = string.Empty;

            var fields = string.Join(", ", _keys);

            if (!_process.MasterEntity.IsFirstRun) {
                _batchIds = _process.Entities.Select(kv => kv.TflBatchId).Distinct().ToArray();
                @where = _batchIds.Length == 1 ? " WHERE [TflBatchId] = @TflBatchId" : string.Format(" WHERE TflBatchId BETWEEN {0} AND {1}", _batchIds.Min(), _batchIds.Max());
            }

            var sql = string.Format("SELECT [TflKey], {0} FROM {1}{2};", fields, _process.Star, where);
            Debug("SQL:\r\n{0}", sql);
            return sql;
        }

        protected override Row CreateRowFromReader(IDataReader reader) {
            var row = new Row();
            var index = 1;
            row["TflKey"] = reader.GetValue(0);
            foreach (var k in _keys) {
                row[k] = reader.GetValue(index);
                index++;
            }
            return row;
        }

        protected override void PrepareCommand(IDbCommand cmd) {
            cmd.CommandText = PrepareSql();
            cmd.CommandTimeout = 0;

            if (!_process.MasterEntity.IsFirstRun) {
                if (_batchIds.Length == 1) {
                    AddParameter(cmd, "@TflBatchId", _batchIds[0]);
                }
            }

            cmd.CommandTimeout = 0;
        }
    }
}