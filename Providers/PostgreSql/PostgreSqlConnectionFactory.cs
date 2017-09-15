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
using Npgsql;
using Transformalize.Configuration;
using Transformalize.Providers.Ado;

namespace Transformalize.Providers.PostgreSql {
    public class PostgreSqlConnectionFactory : IConnectionFactory {
        static Dictionary<string, string> _types;
        private static HashSet<string> _reserved;
        readonly Connection _c;

        public AdoProvider AdoProvider { get; } = AdoProvider.PostgreSql;
        public string Terminator { get; } = ";";

        //select * from pg_get_keywords() where catcode = 'R'
        public HashSet<string> Reserved => _reserved ??
        (_reserved =
            new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "all",
                "analyse",
                "analyze",
                "and",
                "any",
                "array",
                "as",
                "asc",
                "asymmetric",
                "authorization",
                "binary",
                "both",
                "case",
                "cast",
                "check",
                "collate",
                "collation",
                "column",
                "concurrently",
                "constraint",
                "create",
                "cross",
                "current_catalog",
                "current_date",
                "current_role",
                "current_schema",
                "current_time",
                "current_timestamp",
                "current_user",
                "default",
                "deferrable",
                "desc",
                "distinct",
                "do",
                "else",
                "end",
                "except",
                "fetch",
                "for",
                "foreign",
                "freeze",
                "from",
                "full",
                "grant",
                "group",
                "ilike",
                "in",
                "initially",
                "intersect",
                "into",
                "is",
                "isnull",
                "join",
                "lateral",
                "leading",
                "left",
                "localtime",
                "localtimestamp",
                "natural",
                "not",
                "notnull",
                "null",
                "offset",
                "on",
                "only",
                "or",
                "order",
                "outer",
                "overlaps",
                "placing",
                "primary",
                "references",
                "returning",
                "right",
                "select",
                "session_user",
                "similar",
                "some",
                "symmetric",
                "table",
                "tablesample",
                "then",
                "to",
                "trailing",
                "true",
                "union",
                "unique",
                "user",
                "using",
                "variadic",
                "verbose",
                "when",
                "where",
                "window",
                "with"
            });

        public Dictionary<string, string> Types => _types ?? (_types = new Dictionary<string, string> {
            {"int64", "BIGINT"},
            {"int", "INTEGER"},
            {"real", "REAL" },
            {"long", "BIGINT"},
            {"boolean", "BOOLEAN"},
            {"bool", "BOOLEAN"},
            {"string", "VARCHAR"},
            {"date", "DATE" },
            {"datetime", "TIMESTAMP"},
            {"decimal", "NUMERIC"},
            {"double", "DOUBLE PRECISION"},
            {"float", "DOUBLE PRECISION" },
            {"int32", "INTEGER"},
            {"char", "VARCHAR"},
            {"single", "REAL"},
            {"int16", "SMALLINT"},
            {"short","SMALLINT" },
            {"byte", "SMALLINT"},
            {"byte[]", "BYTEA"},
            {"guid", "UUID"},
            {"rowversion", "BYTEA"},
            {"xml", "XML"}
        });

        public PostgreSqlConnectionFactory(Connection connection) {
            _c = connection;
        }

        public IDbConnection GetConnection(string appName = null) {
            return new NpgsqlConnection(GetConnectionString(appName));
        }

        public string GetConnectionString(string appName = null) {
            if (_c.ConnectionString != string.Empty)
                return _c.ConnectionString;

            _c.ConnectionString = new NpgsqlConnectionStringBuilder {
                ApplicationName = appName ?? Constants.ApplicationName,
                Database = _c.Database,
                Host = _c.Server,
                IntegratedSecurity = _c.User == string.Empty && _c.Password == string.Empty,
                Password = _c.Password,
                Username = _c.User,
                Port = _c.Port == 0 ? 5432 : _c.Port,
                Timeout = _c.RequestTimeout
            }.ConnectionString;

            return _c.ConnectionString;
        }

        static char L { get; } = '"';
        static char R { get; } = '"';

        /// <summary>
        /// The Postgres Server requires case sensativity if you enclose identifiers in double-quotes.  
        /// So, this is only done when necessary.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string Enclose(string name) {
            return name.Contains(" ") || Reserved.Contains(name) ? L + name + R : name;
        }

        public string SqlDataType(Field f) {

            var length = (new[] { "string", "char" }).Any(t => t == f.Type) ? $"({(f.Length)})" : string.Empty;
            var dimensions = (new[] { "decimal" }).Any(s => s.Equals(f.Type)) ?
                $"({f.Precision},{f.Scale})" :
                string.Empty;

            var sqlDataType = Types[f.Type];

            var type = string.Concat(sqlDataType, length, dimensions);
            switch (type.ToLower()) {
                case "varchar(max)":
                    return "TEXT";
                default:
                    return type;
            }

        }

    }
}
