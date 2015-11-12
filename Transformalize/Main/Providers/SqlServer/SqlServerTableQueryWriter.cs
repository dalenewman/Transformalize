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
using System.Linq;

namespace Transformalize.Main.Providers.SqlServer {

    public class SqlServerTableQueryWriter : QueryWriter, ITableQueryWriter {

        public string CreateTable(string name, IEnumerable<string> defs) {
            var defList = string.Join(",", defs);
            return $"CREATE TABLE [{SqlIdentifier(name)}]({defList});";
        }

        public string AddPrimaryKey(string name, IEnumerable<string> primaryKey) {
            var pk = primaryKey.ToArray();
            var keyList = string.Join(", ", pk);
            return string.Format(
                "ALTER TABLE [{0}] ADD CONSTRAINT [PK_{0}_{1}] PRIMARY KEY NONCLUSTERED ({2}) WITH (IGNORE_DUP_KEY = ON);",
                SqlIdentifier(name),
                KeyName(pk),
                keyList
            );
        }

        public string AddUniqueClusteredIndex(string name) {
            return string.Format(
                "CREATE UNIQUE CLUSTERED INDEX [UX_{0}_TflKey] ON [{0}] (TflKey ASC);",
                SqlIdentifier(name)
            );
        }

        public string WriteTemporary(AbstractConnection connection, string name, Fields fields, bool useAlias = true) {
            var defs = useAlias ? new FieldSqlWriter(fields).Alias(connection.L, connection.R).DataType(new SqlServerDataTypeService()).Write() : new FieldSqlWriter(fields).Name(connection.L, connection.R).DataType(new SqlServerDataTypeService()).Write();
            return string.Format(@"DECLARE @{0} AS TABLE({1});", name.TrimStart("@".ToCharArray()), defs);
        }

    }
}