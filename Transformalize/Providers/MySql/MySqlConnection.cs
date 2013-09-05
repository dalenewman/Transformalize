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
using System.Data;
using System.Data.Common;
using Transformalize.Core.Entity_;

namespace Transformalize.Providers.MySql {

    public class MySqlConnection : IConnection {

        private readonly IConnectionChecker _connectionChecker;
        private ICompatibilityReader _compatibilityReader;
        private readonly DbConnectionStringBuilder _builder;

        public string Name { get; set; }
        public string Provider { get; set; }
        public int BatchSize { get; set; }
        public int CompatibilityLevel { get; set; }
        public ConnectionType ConnectionType { get; set; }
        public string Process { get; set; }
        public IScriptRunner ScriptRunner { get; private set; }
        
        public IDbConnection GetConnection()
        {
            var type = Type.GetType(Provider, false, true);
            var connection = (IDbConnection) Activator.CreateInstance(type);
            connection.ConnectionString = ConnectionString;
            return connection;
        }

        public void LoadEndVersion(Entity entity)
        {
            throw new NotImplementedException();
        }

        public void LoadBeginVersion(Entity entity)
        {
            throw new NotImplementedException();
        }

        private ICompatibilityReader CompatibilityReader
        {
            get { return _compatibilityReader ?? (_compatibilityReader = new MySqlCompatibilityReader()); }
        }

        public MySqlConnection(string connectionString)
        {
            Provider = "MySql.Data.MySqlClient.MySqlConnection, MySql.Data";

            _builder = new DbConnectionStringBuilder {ConnectionString = connectionString};

            _connectionChecker = new MySqlConnectionChecker(this);
            ScriptRunner = new MySqlScriptRunner(this);
        }

        public string ConnectionString {
            get { return _builder.ConnectionString; }
        }

        public string Database {
            get { return _builder["Database"].ToString() ; }
        }

        public string Server {
            get { return _builder["Server"].ToString(); }
        }

        public bool IsReady() {
            return _connectionChecker.Check(ConnectionString);
        }

        public bool InsertMultipleValues() {
            return CompatibilityReader.InsertMultipleValues;
        }
    }
}