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

using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using Transformalize.Core.Entity_;

namespace Transformalize.Providers.SqlServer {

    public class SqlServerConnectionX : IConnection {

        private readonly IConnectionChecker _connectionChecker;
        private readonly SqlConnectionStringBuilder _builder;
        private ICompatibilityReader _compatibilityReader;

        public string Name { get; set; }
        public ProviderSetup Provider { get; set; }
        public int BatchSize { get; set; }
        public int CompatibilityLevel { get; set; }
        public ConnectionType ConnectionType { get; set; }
        public string Process { get; set; }
        public IScriptRunner ScriptRunner { get; private set; }
        public bool HasRows { get; set; }

        public IDbConnection GetConnection()
        {
            return new SqlConnection(ConnectionString);
        }

        private ICompatibilityReader CompatibilityReader
        {
            get { return _compatibilityReader ?? (_compatibilityReader = new SqlServerCompatibilityReader(this)); }
        }

        public SqlServerConnectionX(string connectionString)
        {
            Provider = new ProviderSetup { ProviderType = "System.Data.SqlClient.SqlConnection, System.Data, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089", L = "[", R = "]" };
            _builder = new SqlConnectionStringBuilder(connectionString);
            _connectionChecker = new DefaultConnectionChecker();
            ScriptRunner = new DefaultScriptRunner(this);
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
            return _connectionChecker.Check(this);
        }

        public bool CanInsertMultipleValues() {
            if (CompatibilityLevel > 0)
                return CompatibilityLevel > 90;
            return CompatibilityReader.CanInsertMultipleValues;
        }

        private SqlDataReader GetEndVersionReader(Entity entity)
        {
            var sql = string.Format("SELECT MAX([{0}]) AS [{0}] FROM [{1}].[{2}];", entity.Version.Name, entity.Schema, entity.Name);
            var cn = new SqlConnection(ConnectionString);
            cn.Open();
            var command = new SqlCommand(sql, cn);
            return command.ExecuteReader(CommandBehavior.CloseConnection & CommandBehavior.SingleResult);
        }

        public void LoadEndVersion(Entity entity)
        {
            using (var reader = GetEndVersionReader(entity))
            {
                entity.HasRows = reader.HasRows;
                if (!entity.HasRows)
                    entity.End = null;
                reader.Read();
                entity.End = reader.GetValue(0);
            }
        }

        private SqlDataReader GetBeginVersionReader(string field, Entity entity)
        {

            var sql = string.Format(@"
                SELECT [{0}]
                FROM [TflBatch]
                WHERE [TflBatchId] = (
	                SELECT [TflBatchId] = MAX([TflBatchId])
	                FROM [TflBatch]
	                WHERE [ProcessName] = @ProcessName 
                    AND [EntityName] = @EntityName
                );
            ", field);

            var cn = new SqlConnection(ConnectionString);
            cn.Open();
            var command = new SqlCommand(sql, cn);
            command.Parameters.Add(new SqlParameter("@ProcessName", entity.ProcessName));
            command.Parameters.Add(new SqlParameter("@EntityName", entity.Alias));
            return command.ExecuteReader(CommandBehavior.CloseConnection & CommandBehavior.SingleResult);
        }

        public void LoadBeginVersion(Entity entity)
        {
            using (var reader = GetBeginVersionReader(entity.GetVersionField(), entity))
            {
                entity.HasRange = reader.HasRows;
                if (!entity.HasRange)
                {
                    entity.Begin = null;
                }
                else
                {
                    reader.Read();
                    entity.Begin = reader.GetValue(0); 
                }
            }
        }

    }
}