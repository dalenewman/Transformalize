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