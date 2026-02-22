#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2023 Dale Newman
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
using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Providers.Ado;

namespace Transformalize.Providers.SQLite {
   public class SqliteConnectionFactory : IConnectionFactory {
      private static Dictionary<string, string> _types;
      private readonly Connection _c;

      public AdoProvider AdoProvider { get; } = AdoProvider.SqLite;
      public string Terminator { get; } = ";";
      public bool SupportsLimit { get; } = true;

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

      public SqliteConnectionFactory(Connection connection) {
         _c = connection;
      }

      public IDbConnection GetConnection(string appName = null) {
         return new SqliteConnection(GetConnectionString(appName));
      }

      public string GetConnectionString(string appName = null) {
         if (_c.ConnectionString != string.Empty)
            return _c.ConnectionString;

         _c.ConnectionString = new SqliteConnectionStringBuilder {
            DataSource = _c.File == string.Empty ? _c.Database : _c.File,
            DefaultTimeout = _c.Timeout,
            Password = _c.Password
         }.ConnectionString;

         return _c.ConnectionString;
      }

      private static char L { get; } = '"';
      private static char R { get; } = '"';

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
