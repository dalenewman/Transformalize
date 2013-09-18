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

namespace Transformalize.Main.Providers {
    public class SqlMapReader : IMapReader {
        private readonly string _sql;
        private readonly AbstractConnection _connection;

        public SqlMapReader(string sql, AbstractConnection connection) {
            _sql = sql;
            _connection = connection;
        }

        public Map Read() {
            var map = new Map();

            using (var cn = _connection.GetConnection()) {
                cn.Open();
                var cmd = cn.CreateCommand();
                cmd.CommandText = _sql;
                var reader = cmd.ExecuteReader();
                if (reader != null) {
                    while (reader.Read()) {
                        map[reader.GetValue(0).ToString()] = new Item(reader.GetValue(1));
                    }
                }
            }

            return map;
        }
    }
}