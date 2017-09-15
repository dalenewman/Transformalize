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
using System.Data.SqlClient;
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Providers.Ado;

namespace Transformalize.Providers.SqlServer {
    public class SqlServerConnectionFactory : IConnectionFactory {

        static Dictionary<string, string> _types;
        readonly Connection _c;

        static Dictionary<string, string> Types => _types ?? (_types = new Dictionary<string, string> {
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

        public AdoProvider AdoProvider { get; } = AdoProvider.SqlServer;
        public string Terminator { get; } = ";";

        public SqlServerConnectionFactory(Connection connection) {
            _c = connection;
        }

        public IDbConnection GetConnection(string appName = null) {
            return new SqlConnection(GetConnectionString(appName));
        }

        static char L { get; } = '[';
        static char R { get; } = ']';

        public string Enclose(string name) {
            return L + name + R;
        }

        public string SqlDataType(Field f) {

            var length = (new[] { "string", "char", "binary", "byte[]", "rowversion", "varbinary" }).Any(t => t == f.Type) ? string.Concat("(", f.Length, ")") : string.Empty;
            var dimensions = (new[] { "decimal" }).Any(s => s.Equals(f.Type)) ?
                $"({f.Precision},{f.Scale})" :
                string.Empty;

            var sqlDataType = Types[f.Type];

            if (!f.Unicode && sqlDataType.StartsWith("N", StringComparison.Ordinal)) {
                sqlDataType = sqlDataType.TrimStart("N".ToCharArray());
            }

            if (!f.VariableLength && (sqlDataType.EndsWith("VARCHAR", StringComparison.Ordinal) || sqlDataType == "VARBINARY")) {
                sqlDataType = sqlDataType.Replace("VAR", string.Empty);
            }

            return string.Concat(sqlDataType, length, dimensions);
        }

        public string GetConnectionString(string appName = null) {
            if (!string.IsNullOrEmpty(_c.ConnectionString)) {
                return _c.ConnectionString;
            }

            _c.ConnectionString = (new SqlConnectionStringBuilder {
                ApplicationName = appName ?? Constants.ApplicationName,
                ConnectTimeout = _c.RequestTimeout,
                DataSource = _c.Server,
                InitialCatalog = _c.Database,
                IntegratedSecurity = _c.User == string.Empty,
                UserID = _c.User,
                Password = _c.Password
            }).ConnectionString;

            return _c.ConnectionString;
        }

    }
}
