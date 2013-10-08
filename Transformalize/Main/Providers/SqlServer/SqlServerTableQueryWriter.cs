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
    public class SqlServerTableQueryWriter : ITableQueryWriter {

        private const string CREATE_TABLE_TEMPLATE = "CREATE TABLE [{0}].[{1}]({2});";
        public string CreateTable(string name, IEnumerable<string> defs, string schema) {
            var defList = string.Join(",", defs);
            return string.Format(
                CREATE_TABLE_TEMPLATE,
                schema,
                Name128(name),
                defList
            );
        }

        private const string ADD_PRIMARY_KEY = "ALTER TABLE [{0}].[{1}] ADD CONSTRAINT [PK_{1}_{2}] PRIMARY KEY NONCLUSTERED ({3}) WITH (IGNORE_DUP_KEY = ON);";
        public string AddPrimaryKey(string name, string schema, IEnumerable<string> primaryKey) {
            var pk = primaryKey.ToArray();
            var keyList = string.Join(", ", pk);
            return string.Format(
                ADD_PRIMARY_KEY,
                schema,
                Name128(name),
                KeyName(pk),
                keyList
            );
        }

        private const string DRP_PRIMARY_KEY = "ALTER TABLE [{0}].[{1}] DROP CONSTRAINT [PK_{1}_{2}];";
        public string DropPrimaryKey(string name, string schema, IEnumerable<string> primaryKey) {
            var pk = primaryKey.ToArray();
            return string.Format(
                DRP_PRIMARY_KEY,
                schema,
                Name128(name),
                KeyName(pk)
            );
        }

        private const string ADD_UNIQUE_CLUSTERED_INDEX = "CREATE UNIQUE CLUSTERED INDEX [UX_{0}_TflKey] ON [{1}].[{0}] (TflKey ASC);";
        public string AddUniqueClusteredIndex(string name, string schema) {
            return string.Format(
                ADD_UNIQUE_CLUSTERED_INDEX,
                Name128(name),
                schema
            );
        }

        private const string DRP_UNIQUE_CLUSTERED_INDEX = "DROP INDEX [UX_{0}_TflKey] ON [{1}].[{0}];";
        public string DropUniqueClusteredIndex(string name, string schema) {
            return string.Format(
                DRP_UNIQUE_CLUSTERED_INDEX,
                Name128(name),
                schema
            );
        }

        public string WriteTemporary(string name, Field[] fields, AbstractProvider provider, bool useAlias = true) {
            var defs = useAlias ? new FieldSqlWriter(fields).Alias(provider).DataType().Write() : new FieldSqlWriter(fields).Name(provider).DataType().Write();
            return string.Format(@"DECLARE @{0} AS TABLE({1});", name.TrimStart("@".ToCharArray()), defs);
        }

        private static string Name128(string name) {
            return name.Length > 128 ? name.Substring(0, 128) : name;
        }

        private static string KeyName(string[] pk) {
            return Name128(string.Join("_", pk).Replace("[", string.Empty).Replace("]", string.Empty).Replace(" ", "_"));
        }

    }
}