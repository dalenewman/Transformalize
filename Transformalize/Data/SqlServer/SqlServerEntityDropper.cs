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
using Transformalize.Model;

namespace Transformalize.Data.SqlServer {
    public class SqlServerEntityDropper : WithLoggingMixin, IEntityDropper {

        private const string FORMAT = "DROP TABLE [{0}].[{1}];";

        public void DropOutput(Entity entity) {
            if (!new SqlServerEntityExists().OutputExists(entity)) return;

            using (var cn = new SqlConnection(entity.OutputConnection.ConnectionString)) {
                cn.Open();
                new SqlCommand(string.Format(FORMAT, entity.Schema, entity.OutputName()), cn).ExecuteNonQuery();
                Debug("{0} | Dropped Output {1}.{2}", entity.ProcessName, entity.Schema, entity.OutputName());
            }
        }
    }
}