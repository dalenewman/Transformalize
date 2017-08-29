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
using System.Linq;
using MySql.Data.MySqlClient;
using Transformalize.Configuration;
using Transformalize.Providers.Ado;

namespace Transformalize.Providers.MySql {
    public class MySqlConnectionFactory : IConnectionFactory {

        static Dictionary<string, string> _types;
        readonly Connection _c;
        public static Dictionary<string, string> Types => _types ?? (_types = new Dictionary<string, string> {
            {"int64", "BIGINT"},
            {"int", "INTEGER"},
            {"long", "BIGINT"},
            {"boolean", "BIT"},
            {"bool", "BIT"},
            {"string", "VARCHAR"},
            {"datetime", "DATETIME"},
            {"date", "DATETIME"},
            {"time", "DATETIME"},
            {"decimal", "NUMERIC"},
            {"numeric", "DECIMAL"},
            {"double", "DOUBLE"},
            {"int32", "INTEGER"},
            {"char", "VARCHAR"},
            {"single", "REAL"},
            {"int16", "SMALLINT"},
            {"short", "SMALLINT" },
            {"byte", "SMALLINT"},
            {"byte[]", "VARBINARY"},
            {"guid", "CHAR"},
            {"rowversion", "BINARY"},
            {"xml", "TEXT"}
        });

        public AdoProvider AdoProvider { get; } = AdoProvider.MySql;
        public string Terminator { get; } = ";";

        public MySqlConnectionFactory(Connection connection) {
            _c = connection;
        }

        public IDbConnection GetConnection(string appName = null) {
            return new MySqlConnection(GetConnectionString(appName));
        }

        public string GetConnectionString(string appName = null) {
            if (_c.ConnectionString != string.Empty) {
                return _c.ConnectionString;
            }

            _c.ConnectionString = (new MySqlConnectionStringBuilder {
                ConnectionTimeout = Convert.ToUInt32(_c.RequestTimeout),
                // CharacterSet = "utf8mb4",
                PersistSecurityInfo = true,
                UsePerformanceMonitor = false,
                Server = _c.Server,
                Database = _c.Database,
                Port = Convert.ToUInt32(_c.Port == 0 ? 3306 : _c.Port),
                IntegratedSecurity = _c.User == string.Empty,
                UserID = _c.User,
                Password = _c.Password
            }).ConnectionString;

            return _c.ConnectionString;

        }

        static char L { get; } = '`';
        static char R { get; } = '`';

        public string Enclose(string name) {
            return L + name + R;
        }

        public string SqlDataType(Field f) {

            var length = (new[] { "string", "char", "byte[]", "guid" }).Any(t => t == f.Type) ? $"({(f.Length)})" : string.Empty;
            var dimensions = (new[] { "decimal" }).Any(s => s.Equals(f.Type)) ?
                $"({f.Precision},{f.Scale})" :
                string.Empty;

            var sqlDataType = Types[f.Type];

            var type = string.Concat(sqlDataType, length, dimensions);
            switch (type.ToLower()) {
                case "varbinary(max)":
                    return "BLOB";
                case "varchar(max)":
                    return "TEXT";
                default:
                    return type;
            }
        }

    }
}
