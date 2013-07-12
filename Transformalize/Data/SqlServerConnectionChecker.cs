/*
Transformalize - Replicate, Transform, and Denormalize Your Data...
Copyright (C) 2013 Dale Newman

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Transformalize.Rhino.Etl.Core;

namespace Transformalize.Data {
    public class SqlServerConnectionChecker : WithLoggingMixin, IConnectionChecker {
        private readonly string _logPrefix;
        private readonly int _timeOut;
        private static readonly Dictionary<string, bool> CachedResults = new Dictionary<string, bool>();

        public SqlServerConnectionChecker(string logPrefix = "", int timeOut = 3) {
            _logPrefix = logPrefix;
            _timeOut = timeOut;
        }

        public bool Check(string connectionString) {
            if (CachedResults.ContainsKey(connectionString)) {
                return CachedResults[connectionString];
            }

            var builder = new SqlConnectionStringBuilder(connectionString) { ConnectTimeout = _timeOut };
            var result = false;
            using (var cn = new SqlConnection(builder.ConnectionString)) {
                try {
                    cn.Open();
                    result = cn.State == ConnectionState.Open;
                    if (result) {
                        Info("{0} | {1}.{2} is ready.", _logPrefix, builder.DataSource, builder.InitialCatalog);
                    }
                    else {
                        Warn("{0} | {1}.{2} wouldn't open.", _logPrefix, builder.DataSource, builder.InitialCatalog);
                    }
                }
                catch (Exception e) {
                    Warn("{0} | {1}.{2} threw error: {3}.", _logPrefix, builder.DataSource, builder.InitialCatalog, e.Message);
                }
            }
            CachedResults[connectionString] = result;
            return result;
        }

    }
}