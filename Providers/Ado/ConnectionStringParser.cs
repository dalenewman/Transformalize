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
using System.Linq;

namespace Transformalize.Providers.Ado {
    public static class ConnectionStringParser {
        static readonly string[] ServerAliases = { "server", "host", "data source", "datasource", "address", "addr", "network address" };
        static readonly string[] DatabaseAliases = { "database", "initial catalog" };
        static readonly string[] UsernameAliases = { "user id", "uid", "username", "user name", "user" };
        static readonly string[] PasswordAliases = { "password", "pwd" };
        static readonly string[] TrustedAliases = { "trusted_connection", "integrated security" };
        static readonly string[] FileAliases = { "data source", "datasource", "file", "filename" };
        static readonly string[] PersistSecurityInfoAliases = { "persist security info" };

        public static string GetPassword(string connectionString) {
            return GetValue(connectionString, PasswordAliases);
        }

        public static string GetUsername(string connectionString) {
            return GetValue(connectionString, UsernameAliases);
        }

        public static string GetDatabaseName(string connectionString) {
            return GetValue(connectionString, DatabaseAliases);
        }

        public static string GetServerName(string connectionString) {
            return GetValue(connectionString, ServerAliases);
        }

        public static string GetFileName(string connectionString) {
            return GetValue(connectionString, FileAliases);
        }

        public static string GetPersistSecurityInfo(string connectionString) {
            return GetValue(connectionString, PersistSecurityInfoAliases);
        }

        public static bool GetTrustedConnection(string connectionString) {
            return (new[] { "true", "sspi" }).Contains(GetValue(connectionString, TrustedAliases).ToLower());
        }

        static string GetValue(string connectionString, params string[] keyAliases) {
            var keyValuePairs = connectionString.Split(';')
                                                .Where(kvp => kvp.Contains('='))
                                                .Select(kvp => kvp.Split(new[] { '=' }, 2))
                                                .ToDictionary(kvp => kvp[0].Trim(),
                                                              kvp => kvp[1].Trim(),
                                                              StringComparer.InvariantCultureIgnoreCase);
            foreach (var alias in keyAliases) {
                string value;
                if (keyValuePairs.TryGetValue(alias, out value))
                    return value;
            }
            return string.Empty;
        }
    }
}
