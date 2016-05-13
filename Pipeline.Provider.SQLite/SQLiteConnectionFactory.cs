#region license
// Transformalize
// A Configurable ETL Solution Specializing in Incremental Denormalization.
// Copyright 2013 Dale Newman
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
using System.Data.SQLite;
using System.Linq;
using Pipeline.Configuration;
using Pipeline.Provider.Ado;

namespace Pipeline.Provider.SQLite {
    public class SqLiteConnectionFactory : IConnectionFactory {
        static Dictionary<string, string> _types;
        readonly Connection _c;

        public AdoProvider AdoProvider { get; } = AdoProvider.SqLite;

        public Dictionary<string, string> Types => _types ?? (_types = new Dictionary<string, string> {
            {"int64", "BIGINT"},
            {"int", "INTEGER"},
            {"real", "REAL" },
            {"long", "BIGINT"},
            {"boolean", "BOOLEAN"},
            {"bool", "BOOLEAN"},
            {"string", "NVARCHAR"},
            {"date", "DATE" },
            {"datetime", "DATETIME"},
            {"decimal", "DECIMAL"},
            {"double", "DOUBLE"},
            {"float", "FLOAT" },
            {"int32", "INTEGER"},
            {"char", "TEXT"},
            {"single", "REAL"},
            {"int16", "SMALLINT"},
            {"short", "SMALLINT" },
            {"byte", "TINYINT"},
            {"byte[]", "BLOB"},
            {"guid", "TEXT"}
        });

        public SqLiteConnectionFactory(Connection connection) {
            _c = connection;
        }

        public IDbConnection GetConnection() {
            return new SQLiteConnection(GetConnectionString());
        }

        public string GetConnectionString() {
            if (_c.ConnectionString != string.Empty)
                return _c.ConnectionString;

            _c.ConnectionString = new SQLiteConnectionStringBuilder {
                DataSource = _c.File == string.Empty ? _c.Database : _c.File,
                Version = 3,
                FailIfMissing = false,
                Password = _c.Password,
            }.ConnectionString;

            return _c.ConnectionString;
        }

        static char L { get; } = '"';
        static char R { get; } = '"';

        public string Enclose(string name) {
            return L + name + R;
        }

        public string SqlDataType(Field f) {

            var length = (new[] { "string", "char" }).Any(t => t == f.Type) ? $"({(f.Length)})" : string.Empty;
            var dimensions = (new[] { "decimal" }).Any(s => s.Equals(f.Type)) ?
                $"({f.Precision},{f.Scale})" :
                string.Empty;

            var sqlDataType = Types[f.Type];

            var type = string.Concat(sqlDataType, length, dimensions);
            switch (type.ToLower()) {
                case "nvarchar(max)":
                case "varchar(max)":
                    return "TEXT";
                default:
                    return type;
            }

        }

    }
}
