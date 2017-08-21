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
using System.Data.Odbc;
using System.IO;
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Providers.Ado;

namespace Transformalize.Providers.Access {
    public class AccessConnectionFactory : IConnectionFactory {
        static Dictionary<string, string> _types;
        readonly Connection _c;

        public AdoProvider AdoProvider { get; } = AdoProvider.Access;

        static Dictionary<string, string> Types => _types ?? (_types = new Dictionary<string, string> {
            {"int64", "LONG"},
            {"int", "INTEGER"},
            {"long", "LONG"},
            {"boolean", "YESNO"},
            {"bool", "YESNO"},
            {"string", "CHAR"},
            {"datetime", "DATE"},
            {"date", "DATE"},
            {"time", "DATE"},
            {"decimal", "CURRENCY"},
            {"numeric", "CURRENCY"},
            {"double", "DOUBLE"},
            {"int32", "INTEGER"},
            {"short", "INTEGER" },
            {"char", "CHAR"},
            {"single", "SINGLE"},
            {"int16", "INTEGER"},
            {"byte", "BYTE"},
            {"byte[]", "OLEOBJECT"},
            {"guid", "CHAR"},
            {"rowversion", "OLEOBJECT"},
            {"xml", "MEMO"}
        });

        public AccessConnectionFactory(Connection connection) {
            _c = connection;
            if (_c.User == string.Empty) {
                _c.User = "Admin";
            }
        }

        public IDbConnection GetConnection(string appName = null) {
            return new OdbcConnection(GetConnectionString(appName));
        }

        public string GetConnectionString(string appName = null) {
            if (_c.ConnectionString != string.Empty)
                return _c.ConnectionString;

            var file = new FileInfo(_c.File == string.Empty ? _c.Database : _c.File);
            var cs = _c.ConnectionString = new OdbcConnectionStringBuilder { ConnectionString = $@"Driver={{Microsoft Access Driver (*.mdb)}};Dbq={file.FullName};Uid={_c.User};Pwd={_c.Password};" }.ConnectionString;

            return cs;
        }

        static char L { get; } = '`';
        static char R { get; } = '`';

        public string Enclose(string name) {
            return L + name + R;
        }

        public string SqlDataType(Field f) {

            var length = (new[] { "string", "char", "binary", "byte[]", "rowversion", "varbinary" }).Any(t => t == f.Type) ? string.Concat("(", f.Length, ")") : string.Empty;

            var sqlDataType = Types[f.Type];

            if (length.ToUpper() == "(MAX)") {
                length = string.Empty;
                sqlDataType = sqlDataType.Contains("BINARY") ? "OLE OBJECT" : "MEMO";
            } else {
                if (sqlDataType == "CHAR" && int.TryParse(f.Length, out int intOut) && intOut > 255) {
                    sqlDataType = "MEMO";
                }
            }

            return string.Concat(sqlDataType, length);
        }

    }
}
