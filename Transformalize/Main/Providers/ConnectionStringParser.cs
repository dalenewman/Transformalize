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

using System;
using System.Linq;

namespace Transformalize.Main.Providers
{
    public static class ConnectionStringParser
    {
        private static readonly string[] ServerAliases = {"server", "host", "data source", "datasource", "address", "addr", "network address"};
        private static readonly string[] DatabaseAliases = {"database", "initial catalog"};
        private static readonly string[] UsernameAliases = {"user id", "uid", "username", "user name", "user"};
        private static readonly string[] PasswordAliases = {"password", "pwd"};
        private static readonly string[] TrustedAliases = {"trusted_connection", "integrated security"};

        public static string GetPassword(string connectionString)
        {
            return GetValue(connectionString, PasswordAliases);
        }

        public static string GetUsername(string connectionString)
        {
            return GetValue(connectionString, UsernameAliases);
        }

        public static string GetDatabaseName(string connectionString)
        {
            return GetValue(connectionString, DatabaseAliases);
        }

        public static string GetServerName(string connectionString)
        {
            return GetValue(connectionString, ServerAliases);
        }

        public static bool GetTrustedConnection(string connectionString)
        {
            return (new[] {"true", "sspi"}).Contains(GetValue(connectionString, TrustedAliases).ToLower());
        }

        private static string GetValue(string connectionString, params string[] keyAliases)
        {
            var keyValuePairs = connectionString.Split(';')
                                                .Where(kvp => kvp.Contains('='))
                                                .Select(kvp => kvp.Split(new[] {'='}, 2))
                                                .ToDictionary(kvp => kvp[0].Trim(),
                                                              kvp => kvp[1].Trim(),
                                                              StringComparer.InvariantCultureIgnoreCase);
            foreach (var alias in keyAliases)
            {
                string value;
                if (keyValuePairs.TryGetValue(alias, out value))
                    return value;
            }
            return string.Empty;
        }
    }
}