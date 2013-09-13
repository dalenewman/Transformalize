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

namespace Transformalize.Operations
{
    public class ParametersExtract : InputCommandOperation
    {
        private readonly string _sql;
        private IParameters _parameters;

        public ParametersExtract(Process process)
            : base(process.MasterEntity.OutputConnection)
        {
            UseTransaction = false;
            _sql = BuildSql(process);
        }

        private string BuildSql(Process process)
        {
            _parameters = process.Parameters();
            var fields = string.Join(", ", _parameters.Keys);
            var tflWhereClause = string.Format(" WHERE [TflBatchId] IN ({0})", string.Join(", ", process.Entities.Select(kv => kv.TflBatchId).Distinct()));
            var sql = string.Format("SELECT [TflKey], {0} FROM {1}{2};", fields, process.View, tflWhereClause);
            Debug("SQL:\r\n{0}", sql);
            return sql;
        }

        protected override Row CreateRowFromReader(IDataReader reader)
        {
            var row = new Row();
            var index = 1;
            row["TflKey"] = reader.GetValue(0);
            foreach (var p in _parameters)
            {
                row[p.Key] = reader.GetValue(index);
                index++;
            }
            return row;
        }

        protected override void PrepareCommand(IDbCommand cmd)
        {
            cmd.CommandText = _sql;
            cmd.CommandTimeout = 0;
        }
    }
}