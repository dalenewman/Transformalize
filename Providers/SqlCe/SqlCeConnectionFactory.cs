#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2017 Dale Newman
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//   
//       http://www.apache.org/licenses/LICENSE-2.0
//   
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlServerCe;
using System.IO;
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Providers.Ado;

namespace Transformalize.Providers.SqlCe {
    public class SqlCeConnectionFactory : IConnectionFactory {
        private static Dictionary<string, string> _types;
        private readonly Connection _c;

        public AdoProvider AdoProvider { get; } = AdoProvider.SqlCe;
        public string Terminator { get; } = ";";

        private static Dictionary<string, string> Types => _types ?? (_types = new Dictionary<string, string> {
            {"int64", "BIGINT"},
            {"int", "INT"},
            {"long", "BIGINT"},
            {"boolean", "BIT"},
            {"bool", "BIT"},
            {"string", "NVARCHAR"},
            {"datetime", "DATETIME"},
            {"date", "DATETIME"},
            {"time", "DATETIME"},
            {"decimal", "DECIMAL"},
            {"numeric", "DECIMAL"},
            {"double", "FLOAT"},
            {"int32", "INT"},
            {"short", "SMALLINT" },
            {"char", "NCHAR"},
            {"single", "REAL"},
            {"int16", "SMALLINT"},
            {"byte", "TINYINT"},
            {"byte[]", "VARBINARY"},
            {"guid", "UNIQUEIDENTIFIER"},
            {"rowversion", "BINARY"},
            {"xml", "XML"}
        });

        public SqlCeConnectionFactory(Connection connection) {
            _c = connection;
        }

        public IDbConnection GetConnection(string appName = null) {
            return new SqlCeConnection(GetConnectionString(appName));
        }

        public string GetConnectionString(string appName = null) {
            if (_c.ConnectionString != string.Empty)
                return _c.ConnectionString;

            var file = new FileInfo(_c.File == string.Empty ? _c.Database : _c.File);

            var cs = _c.ConnectionString = new SqlCeConnectionStringBuilder {
                DataSource = file.FullName,
                Password = _c.Password
            }.ConnectionString;

            // not sure if this is the right place to do this, probably isn't...
            if (file.Exists)
                return cs;

            using (var engine = new SqlCeEngine(cs)) {
                engine.CreateDatabase();
            }

            return cs;
        }

        private static char L { get; } = '[';
        private static char R { get; } = ']';

        public string Enclose(string name) {
            return L + name + R;
        }

        public string SqlDataType(Field f) {

            var length = (new[] { "string", "char", "binary", "byte[]", "rowversion", "varbinary" }).Any(t => t == f.Type) ? string.Concat("(", f.Length, ")") : string.Empty;
            var dimensions = (new[] { "decimal" }).Any(s => s.Equals(f.Type)) ?
                $"({f.Precision},{f.Scale})" :
                string.Empty;

            var sqlDataType = Types[f.Type];

            if (!f.VariableLength && (sqlDataType.EndsWith("VARCHAR", StringComparison.Ordinal) || sqlDataType == "VARBINARY")) {
                sqlDataType = sqlDataType.Replace("VAR", string.Empty);
            }

            if (length.ToUpper() == "(MAX)") {
                length = string.Empty;
                sqlDataType = sqlDataType.Contains("BINARY") ? "IMAGE" : "NTEXT";
            }

            return string.Concat(sqlDataType, length, dimensions);
        }

    }
}
