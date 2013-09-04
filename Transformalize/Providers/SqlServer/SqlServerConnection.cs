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

using System.Data.SqlClient;

namespace Transformalize.Providers.SqlServer {

    public class SqlServerConnection : IConnection {

        private readonly IConnectionChecker _connectionChecker;
        private readonly SqlConnectionStringBuilder _builder;
        private ICompatibilityReader _compatibilityReader;

        public string Provider { get; set; }
        public int BatchSize { get; set; }
        public int CompatibilityLevel { get; set; }
        public ConnectionType ConnectionType { get; set; }
        public string Process { get; set; }
        public IScriptRunner ScriptRunner { get; private set; }

        private ICompatibilityReader CompatibilityReader
        {
            get { return _compatibilityReader ?? (_compatibilityReader = new SqlServerCompatibilityReader(this)); }
        }

        public SqlServerConnection(string connectionString) {
            _builder = new SqlConnectionStringBuilder(connectionString);
            _connectionChecker = new SqlServerConnectionChecker();
            ScriptRunner = new SqlServerScriptRunner(this);
        }

        public string ConnectionString {
            get { return _builder.ConnectionString; }
        }

        public string Database {
            get { return _builder.InitialCatalog; }
        }

        public string Server {
            get { return _builder.DataSource; }
        }

        public bool IsReady() {
            return _connectionChecker.Check(ConnectionString);
        }

        public bool InsertMultipleValues() {
            if (CompatibilityLevel > 0)
                return CompatibilityLevel > 90;
            return CompatibilityReader.InsertMultipleValues;
        }
    }
}