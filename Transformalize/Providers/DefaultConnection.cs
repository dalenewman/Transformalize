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

namespace Transformalize.Providers
{
    public class DefaultConnection : IConnection
    {

        private readonly IConnectionChecker _connectionChecker;
        private readonly DbConnectionStringBuilder _builder;
        private Compatibility _compatibility;

        public string Name { get; set; }
        public ProviderSetup Provider { get; set; }
        public int BatchSize { get; set; }
        public int CompatibilityLevel { get; set; }
        public string Process { get; set; }
        public IScriptRunner ScriptRunner { get; private set; }

        public Compatibility Compatibility
        {
            get { return _compatibility ?? (_compatibility = Provider.CompatibilityReader.Read(this)); }
        }

        public string ConnectionString
        {
            get { return _builder.ConnectionString; }
        }

        public string Database
        {
            get
            {
                return _builder["Database"].ToString();
            }
        }

        public string Server
        {
            get
            {
                return _builder["Server"].ToString();
            }
        }

        public DefaultConnection(string connectionString, ProviderSetup providerSetup)
        {
            Provider = providerSetup;
            _builder = new DbConnectionStringBuilder { ConnectionString = connectionString };
            _connectionChecker = new DefaultConnectionChecker();
            ScriptRunner = new DefaultScriptRunner(this);
        }

        public IDbConnection GetConnection()
        {
            var type = Type.GetType(Provider.ProviderType, false, true);
            var connection = (IDbConnection)Activator.CreateInstance(type);
            connection.ConnectionString = ConnectionString;
            return connection;
        }

        public bool IsReady()
        {
            return _connectionChecker.Check(this);
        }

        public int NextBatchId(string processName)
        {
            using (var cn = GetConnection())
            {
                cn.Open();
                var cmd = cn.CreateCommand();
                cmd.CommandText = "SELECT ISNULL(MAX(TflBatchId),0)+1 FROM TflBatch WHERE ProcessName = @ProcessName;";

                var process = cmd.CreateParameter();
                process.ParameterName = "@ProcessName";
                process.Value = processName;

                cmd.Parameters.Add(process);
                return (int)cmd.ExecuteScalar();
            }
        }

        public void AddParameter(IDbCommand command, string name, object val)
        {
            var parameter = command.CreateParameter();
            parameter.ParameterName = name;
            parameter.Value = val ?? DBNull.Value;
            command.Parameters.Add(parameter);
        }

        private static IDbDataParameter CreateParameter(IDbCommand cmd, string name, object value)
        {
            var parameter = cmd.CreateParameter();
            parameter.ParameterName = name;
            parameter.Value = value;
            return parameter;
        }

        public void LoadBeginVersion(Entity entity)
        {
            var sql = string.Format(@"
                SELECT {0}
                FROM TflBatch
                WHERE TflBatchId = (
	                SELECT TflBatchId = MAX(TflBatchId)
	                FROM TflBatch
	                WHERE ProcessName = @ProcessName 
                    AND EntityName = @EntityName
                );
            ", entity.GetVersionField());

            using (var cn = GetConnection())
            {
                cn.Open();
                var cmd = cn.CreateCommand();
                cmd.CommandText = sql;
                cmd.Parameters.Add(CreateParameter(cmd, "@ProcessName", entity.ProcessName));
                cmd.Parameters.Add(CreateParameter(cmd, "@EntityName", entity.Alias));

                using (var reader = cmd.ExecuteReader(CommandBehavior.CloseConnection & CommandBehavior.SingleResult))
                {
                    if (reader == null) return;

                    entity.HasRange = reader.Read();
                    entity.Begin = entity.HasRange ? reader.GetValue(0) : null;
                }
            }
        }

        public void LoadEndVersion(Entity entity)
        {
            var sql = string.Format("SELECT MAX({0}) AS {0} FROM {1};", Provider.Enclose(entity.Version.Name), Provider.Enclose(entity.Name));

            using (var cn = GetConnection())
            {
                var command = cn.CreateCommand();
                command.CommandText = sql;
                cn.Open();
                using (var reader = command.ExecuteReader(CommandBehavior.CloseConnection & CommandBehavior.SingleResult))
                {
                    if (reader == null) return;

                    entity.HasRows = reader.Read();
                    entity.End = entity.HasRows ? reader.GetValue(0) : null;
                }
            }
        }


    }
}