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

namespace Transformalize.Main.Providers.SqlServer
{
    public class SqlServerTableQueryWriter : ITableQueryWriter
    {
        private const string CREATE_TABLE_TEMPLATE = @"
            CREATE TABLE [{0}].[{1}](
                {2},
                CONSTRAINT [Pk_{3}_{4}] PRIMARY KEY (
                    {5}
                ) {6}
            );
        ";

        public string Write(string name, IEnumerable<string> defs, IEnumerable<string> primaryKey, string schema = "dbo", bool ignoreDups = false)
        {
            string[] pk = primaryKey.ToArray();
            string defList = string.Join(",\r\n    ", defs);
            string keyName = string.Join("_", pk).Replace("`", string.Empty).Replace("]", string.Empty).Replace(" ", "_");
            string keyList = string.Join(", ", pk);
            return string.Format(
                CREATE_TABLE_TEMPLATE,
                schema,
                name.Length > 128 ? name.Substring(0, 128) : name,
                defList,
                name.Replace(" ", string.Empty),
                keyName.Length > 128 ? keyName.Substring(0, 128) : keyName,
                keyList,
                ignoreDups ? "WITH (IGNORE_DUP_KEY = ON)" : string.Empty
                );
        }

        public string WriteTemporary(string name, Field[] fields, AbstractProvider provider, bool useAlias = true)
        {
            string defs = useAlias ? new FieldSqlWriter(fields).Alias(provider).DataType().Write() : new FieldSqlWriter(fields).Name(provider).DataType().Write();
            return string.Format(@"DECLARE @{0} AS TABLE({1});", name.TrimStart("@".ToCharArray()), defs);
        }
    }
}