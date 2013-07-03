using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Transformalize.Rhino.Etl.Core;

namespace Transformalize.Repositories {

    public class ConnectionChecker : WithLoggingMixin {

        private readonly List<SqlConnectionStringBuilder> _builders = new List<SqlConnectionStringBuilder>();
        private readonly string _logPrefix;

        public ConnectionChecker(IEnumerable<string> connectionStrings, string logPrefix = "", int timeOut = 7) {
            _logPrefix = logPrefix;
            foreach (var connectionString in connectionStrings) {
                _builders.Add(new SqlConnectionStringBuilder(connectionString) { ConnectTimeout = timeOut });
            }
        }

        public bool Check() {
            var results = new List<bool>();

            foreach (var builder in _builders) {
                SqlConnection sqlConnection;
                using (sqlConnection = new SqlConnection(builder.ConnectionString)) {
                    try {
                        sqlConnection.Open();
                        results.Add(sqlConnection.State == ConnectionState.Open);
                    }
                    catch (Exception e) {
                        results.Add(false);
                        Error(e, String.Format("{0} | {1}.{2} did not respond.", _logPrefix, builder.DataSource, builder.InitialCatalog));
                    }
                }
            }

            var result = results.All(r => r);
            if (result)
                Info("{0} | All Systems: Online.", _logPrefix);
            return result;
        }

    }
}