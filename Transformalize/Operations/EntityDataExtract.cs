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
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Main.Providers;

namespace Transformalize.Operations {
    public class EntityDataExtract : InputCommandOperation {
        private readonly string[] _fields;
        private readonly string _sql;
        //private readonly ObjectPool<Row> _objectPool = new ObjectPool<Row>(() => new Row());

        public EntityDataExtract(string[] fields, string sql, AbstractConnection connection)
            : base(connection) {
            _fields = fields;
            _sql = sql;
            UseTransaction = false;
            Name = "EntityDataExtract";
        }

        protected override Row CreateRowFromReader(IDataReader reader) {
            //var row = _objectPool.GetObject();
            var row = new Row();
            foreach (var field in _fields) {
                row[field] = reader[field];
            }
            return row;
        }

        protected override void PrepareCommand(IDbCommand cmd) {
            cmd.CommandText = _sql;
            cmd.CommandTimeout = 0;
            cmd.CommandType = CommandType.Text;
        }
    }
}