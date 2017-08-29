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

using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Providers.Ado;

namespace Transformalize.Providers.Access {

    public class AccessConnectionFactory : IConnectionFactory {

        private static Dictionary<string, string> _types;
        private readonly Connection _c;
        public AdoProvider AdoProvider { get; } = AdoProvider.Access;
        public string Terminator { get; } = string.Empty;

        private static Dictionary<string, string> Types => _types ?? (_types = new Dictionary<string, string> {
            {"int64", "LONG"},
            {"int", "INTEGER"},
            {"long", "LONG"},
            {"boolean", "YESNO"},
            {"bool", "YESNO"},
            {"string", "TEXT"},
            {"datetime", "DATETIME"},
            {"date", "DATE"},
            {"time", "DATE"},
            {"decimal", "DECIMAL"},
            {"numeric", "DECIMAL"},
            {"double", "DOUBLE"},
            {"int32", "INTEGER"},
            {"short", "INTEGER" },
            {"char", "CHAR"},
            {"single", "SINGLE"},
            {"int16", "INTEGER"},
            {"byte", "BYTE"},
            {"byte[]", "BINARY"},
            {"guid", "GUID"},
            {"xml", "MEMO"}
        });

        public AccessConnectionFactory(Connection connection) {
            _c = connection;
            if (_c.User == string.Empty) {
                _c.User = "Admin";
            }
        }

        public IDbConnection GetConnection(string appName = null) {
            return new OleDbConnection(GetConnectionString(appName));
        }

        public string GetConnectionString(string appName = null) {
            if (_c.ConnectionString != string.Empty)
                return _c.ConnectionString;

            var file = new FileInfo(_c.File == string.Empty ? _c.Database : _c.File);
            var cs = _c.ConnectionString = new OleDbConnectionStringBuilder {
                ConnectionString = $@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={file.FullName};User Id={_c.User};Password={_c.Password};",
            }.ConnectionString;

            return cs;
        }

        private static char L { get; } = '`';
        private static char R { get; } = '`';

        public string Enclose(string name) {
            return L + name + R;
        }

        public string SqlDataType(Field f) {

            var length = (new[] { "string", "char", "byte[]" }).Any(t => t == f.Type) ? string.Concat("(", f.Length, ")") : string.Empty;
            var dimensions = f.Type == "decimal" ? $"({f.Precision},{f.Scale})" : string.Empty;
            var sqlDataType = Types[f.Type];

            if (length.ToUpper() == "(MAX)") {
                length = string.Empty;
                sqlDataType = f.Type == "byte[]" ? "OLEOBJECT" : "MEMO";
            } else {
                if (sqlDataType == "TEXT" && int.TryParse(f.Length, out int intOut) && intOut > 255) {
                    length = string.Empty;
                    sqlDataType = "MEMO";
                }
            }

            if (sqlDataType == "TEXT" && !f.VariableLength) {
                sqlDataType = "CHAR";
            }

            return sqlDataType == "OLEOBJECT" ? "OLEOBJECT" : string.Concat(sqlDataType, length, dimensions);
        }

    }
}
