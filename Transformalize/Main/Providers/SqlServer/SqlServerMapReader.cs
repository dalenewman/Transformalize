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

using System.Data.SqlClient;

namespace Transformalize.Main.Providers.SqlServer
{
    public class SqlServerMapReader : IMapReader
    {
        private readonly string _connectionString;
        private readonly string _sql;

        public SqlServerMapReader(string sql, string connectionString)
        {
            _sql = sql;
            _connectionString = connectionString;
        }

        public Map Read()
        {
            var map = new Map();

            using (var cn = new SqlConnection(_connectionString))
            {
                cn.Open();
                var cmd = new SqlCommand(_sql, cn);
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        map[reader.GetValue(0).ToString()] = new Item(reader.GetValue(1));
                    }
                }
            }

            return map;
        }
    }
}