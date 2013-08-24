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
using Transformalize.Core.Entity_;

namespace Transformalize.Providers.SqlServer {
    public class SqlServerEntityRecordsExist : IEntityRecordsExist {
        private readonly SqlServerEntityExists _entityExists;

        private const string FORMAT = @"
IF EXISTS(
	SELECT *
	FROM [{0}].[{1}]
)	SELECT 1
ELSE
	SELECT 0;";

        public SqlServerEntityRecordsExist() {
            _entityExists = new SqlServerEntityExists();
        }

        public bool OutputRecordsExist(Entity entity) {
            if (_entityExists.OutputExists(entity)) {
                using (var cn = new SqlConnection(entity.OutputConnection.ConnectionString)) {
                    cn.Open();
                    var sql = string.Format(FORMAT, entity.Schema, entity.OutputName());
                    var cmd = new SqlCommand(sql, cn);
                    return (int)cmd.ExecuteScalar() == 1;
                }
            }
            return false;
        }

        public bool InputRecordsExist(Entity entity) {
            if (_entityExists.InputExists(entity)) {
                using (var cn = new SqlConnection(entity.InputConnection.ConnectionString)) {
                    cn.Open();
                    var sql = string.Format(FORMAT, entity.Schema, entity.Alias);
                    var cmd = new SqlCommand(sql, cn);
                    return (int)cmd.ExecuteScalar() == 1;
                }
            }
            return false;
        }
    }
}