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

using System.Data.SqlClient;
using Transformalize.Libs.Rhino.Etl.Core;
using Transformalize.Libs.Rhino.Etl.Core.Operations;
using Transformalize.Model;

namespace Transformalize.Operations
{
    public class ResultsLoad : SqlBatchOperation {
        private readonly Process _process;

        public ResultsLoad(Process process) : base(process.MasterEntity.OutputConnection.ConnectionString) {
            _process = process;
            BatchSize = 50;
            UseTransaction = false;
        }

        protected override void PrepareCommand(Row row, SqlCommand command) {
            var sets = new FieldSqlWriter(_process.Results).Alias().SetParam().Write();
            command.CommandText = string.Format("UPDATE {0} SET {1} WHERE TflKey = @TflKey;", _process.MasterEntity.OutputName(), sets);
            foreach (var r in _process.Results) {
                AddParameter(command, r.Key, row[r.Key]);
            }
            AddParameter(command, "TflKey", row["TflKey"]);
        }
    }
}