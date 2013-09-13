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

namespace Transformalize.Main.Providers.MySql
{
    public class MySqlTableQueryWriter : ITableQueryWriter
    {
        private const string CREATE_TABLE_TEMPLATE = @"
            CREATE TABLE `{0}`(
                {1},
                CONSTRAINT `Pk_{2}_{3}` PRIMARY KEY (
                    {4}
                )
            );
        ";

        public string Write(string name, IEnumerable<string> defs, IEnumerable<string> primaryKey, string schema = "dbo", bool ignoreDups = false)
        {
            var pk = primaryKey.ToArray();
            var defList = string.Join(",\r\n    ", defs);
            var keyName = string.Join("_", pk).Replace("`", string.Empty).Replace(" ", "_");
            var keyList = string.Join(", ", pk);
            return string.Format(
                CREATE_TABLE_TEMPLATE,
                name.Length > 128 ? name.Substring(0, 128) : name,
                defList,
                name.Replace(" ", string.Empty),
                keyName.Length > 128 ? keyName.Substring(0, 128) : keyName,
                keyList
                );
        }

        public string WriteTemporary(string name, Field[] fields, AbstractProvider provider, bool useAlias = true)
        {
            var safeName = provider.Enclose(name.TrimStart("@".ToCharArray()));
            var defs = useAlias ? new FieldSqlWriter(fields).Alias(provider).DataType().Write() : new FieldSqlWriter(fields).Name(provider).DataType().Write();
            return string.Format(@"CREATE TEMPORARY TABLE {0}({1}) ENGINE = MEMORY;", safeName, defs);
        }
    }
}