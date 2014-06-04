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
            
            return string.Format(
                "CREATE TABLE [{0}]({1});",
                SqlIdentifier(name),
                defList
            );
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

        public string DropPrimaryKey(string name, IEnumerable<string> primaryKey) {
            var pk = primaryKey.ToArray();
            return string.Format(
                "ALTER TABLE [{0}] DROP CONSTRAINT [PK_{0}_{1}];",
                SqlIdentifier(name),
                KeyName(pk)
            );
        }

        public string AddUniqueClusteredIndex(string name) {
            return string.Format(
                "CREATE UNIQUE CLUSTERED INDEX [UX_{0}_TflKey] ON [{0}] (TflKey ASC);",
                SqlIdentifier(name)
            );
        }

        public string DropUniqueClusteredIndex(string name) {
            return string.Format(
                @"
                    IF EXISTS(
	                    SELECT i.*
	                    FROM sys.indexes i WITH (NOLOCK)
	                    INNER JOIN sys.tables t WITH (NOLOCK) ON (i.object_id = t.object_id)
	                    WHERE i.[name] = 'UX_{0}_TflKey'
	                    AND t.[name] = '{0}'
                    )	DROP INDEX [UX_{0}_TflKey] ON [{0}];
                ",
                SqlIdentifier(name)
            );
        }

        public string WriteTemporary(string name, Fields fields, AbstractConnection connection, bool useAlias = true) {
            var defs = useAlias ? new FieldSqlWriter(fields).Alias(connection.L, connection.R).DataType(new SqlServerDataTypeService()).Write() : new FieldSqlWriter(fields).Name(connection.L, connection.R).DataType(new SqlServerDataTypeService()).Write();
            return string.Format(@"DECLARE @{0} AS TABLE({1});", name.TrimStart("@".ToCharArray()), defs);
        }

    }
}