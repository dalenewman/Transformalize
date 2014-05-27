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

namespace Transformalize.Main.Providers.SqlServer {
    public class SqlServerTableExists : ITableExists {

        private readonly AbstractConnection _connection;

        public SqlServerTableExists(AbstractConnection connection) {
            _connection = connection;
        }

        public bool Exists(string schema, string name) {
            var sql = string.Format(@"
	            SELECT TOP(1) TABLE_NAME
	            FROM INFORMATION_SCHEMA.TABLES 
	            WHERE TABLE_SCHEMA = '{0}' 
	            AND  TABLE_NAME = '{1}';
            ", schema.Equals(string.Empty) ? _connection.DefaultSchema : schema, name);

            using (var cn = _connection.GetConnection()) {
                cn.Open();
                var cmd = cn.CreateCommand();
                cmd.CommandText = sql;
                cmd.CommandType = CommandType.Text;
                using (var reader = cmd.ExecuteReader()) {
                    return reader.Read();
                }
            }
        }
    }
}