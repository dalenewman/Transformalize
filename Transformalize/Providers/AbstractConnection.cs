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
using Transformalize.Configuration;
using Transformalize.Core.Entity_;
using Transformalize.Core.Field_;

namespace Transformalize.Providers
{
    public abstract class AbstractConnection
    {

        private readonly AbstractConnectionChecker _connectionChecker;
        private readonly DbConnectionStringBuilder _builder;

        public string Name { get; set; }
        protected string TypeAndAssemblyName { get; set; }
        public AbstractProvider Provider { get; set; }
        public int BatchSize { get; set; }
        public string Process { get; set; }
        public IScriptRunner ScriptRunner { get; private set; }
        public IEntityQueryWriter EntityKeysQueryWriter { get; set; }
        public IEntityQueryWriter EntityKeysRangeQueryWriter { get; set; }
        public IEntityQueryWriter EntityKeysAllQueryWriter { get; set; }
        public ITableQueryWriter TableQueryWriter { get; set; }
        public IProviderSupportsModifier ProviderSupportsModifier { get; set; }

        public string ConnectionString
        {
            get { return _builder.ConnectionString; }
        }

        public string Database { get; set; }
        public string Server { get; set; }

        protected AbstractConnection(ConnectionConfigurationElement element, AbstractProvider provider, AbstractConnectionChecker connectionChecker, IScriptRunner scriptRunner, IProviderSupportsModifier providerSupportsModifier)
        {
            Provider = provider;
            BatchSize = element.BatchSize;
            Name = element.Name;

            _builder = new DbConnectionStringBuilder { ConnectionString = element.Value };
            Server = _builder.ContainsKey("Server") ? _builder["Server"].ToString() : string.Empty;
            Database = _builder.ContainsKey("Database") ? _builder["Database"].ToString() : string.Empty;

            _connectionChecker = connectionChecker;
            ScriptRunner = scriptRunner;
            ProviderSupportsModifier = providerSupportsModifier;
        }

        public IDbConnection GetConnection()
        {
            var type = Type.GetType(TypeAndAssemblyName, false, true);
            var connection = (IDbConnection)Activator.CreateInstance(type);
            connection.ConnectionString = ConnectionString;
            return connection;
        }

        public IScriptReponse ExecuteScript(string script)
        {
            return ScriptRunner.Execute(this, script);
        }

        public string WriteTemporaryTable(string name, Field[] fields, bool useAlias = true)
        {
            return TableQueryWriter.WriteTemporary(name, fields, Provider, useAlias);
        }

        public bool IsReady()
        {
            var isReady = _connectionChecker.Check(this);
            if (isReady)
            {
                ProviderSupportsModifier.Modify(this, Provider.Supports);
            }
            return isReady;
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
                AddParameter(cmd, "@ProcessName", entity.ProcessName);
                AddParameter(cmd, "@EntityName", entity.Alias);

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