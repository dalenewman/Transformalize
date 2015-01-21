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

using System.Collections.Generic;

namespace Transformalize.Main.Providers.MySql {
    public class MySqlTableQueryWriter : QueryWriter, ITableQueryWriter {

        public string CreateTable(string name, IEnumerable<string> defs) {
            var defList = string.Join(",\r\n    ", defs);
            return string.Format(
                "CREATE TABLE `{0}`({1});",
                SqlIdentifier(name),
                defList
            );
        }

        public string AddPrimaryKey(string name, IEnumerable<string> primaryKey) {
            throw new System.NotImplementedException();
        }

        public string AddUniqueClusteredIndex(string name) {
            throw new System.NotImplementedException();
        }

        public string WriteTemporary(AbstractConnection connection, string name, Fields fields, bool useAlias = true) {
            var safeName = connection.Enclose(name.TrimStart("@".ToCharArray()));
            var defs = useAlias ? new FieldSqlWriter(fields).Alias(connection.L, connection.R).DataType(new MySqlDataTypeService()).Write() : new FieldSqlWriter(fields).Name(connection.L, connection.R).DataType(new MySqlDataTypeService()).Write();
            return string.Format(@"CREATE TEMPORARY TABLE IF NOT EXISTS {0}({1}) ENGINE = MEMORY; TRUNCATE TABLE {0};", safeName, defs);
        }
    }
}