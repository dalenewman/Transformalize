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
using Transformalize.Core.Process_;
using Transformalize.Libs.NLog;

namespace Transformalize.Providers.SqlServer {
    public class SqlServerConnectionChecker : IConnectionChecker
    {
        private readonly Logger _log = LogManager.GetCurrentClassLogger();
        private readonly int _timeOut;
        private static readonly Dictionary<string, bool> CachedResults = new Dictionary<string, bool>();

        public SqlServerConnectionChecker(int timeOut = 3)
        {
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
                        _log.Debug("{0}.{1} is ready.", builder.DataSource, builder.InitialCatalog);
                    }
                    else {
                        _log.Warn("{0}.{1} wouldn't open.", builder.DataSource, builder.InitialCatalog);
                    }
                }
                catch (Exception e) {
                    _log.Warn("{0}.{1} threw error: {2}.", builder.DataSource, builder.InitialCatalog, e.Message);
                }
            }
            CachedResults[connectionString] = result;
            return result;
        }

    }
}