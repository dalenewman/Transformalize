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
using System.Data;
using System.Data.Common;
using System.IO;
using Transformalize.Configuration;
using Transformalize.Extensions;
using Transformalize.Libs.Dapper;
using Transformalize.Libs.FileHelpers.Enums;
using Transformalize.Libs.NLog;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Main.Providers.SqlServer;

namespace Transformalize.Main.Providers {

    public abstract class AbstractConnection {

        private readonly Logger _log = LogManager.GetLogger(string.Empty);
        private string _l = string.Empty;
        private string _r = string.Empty;

        private const StringComparison IC = StringComparison.OrdinalIgnoreCase;

        public string Name { get; set; }
        protected string TypeAndAssemblyName { get; set; }
        public int BatchSize { get; set; }
        public string Process { get; set; }

        public IConnectionChecker ConnectionChecker { get; set; }
        public IEntityRecordsExist EntityRecordsExist { get; set; }
        public IEntityDropper EntityDropper { get; set; }
        public IEntityCreator EntityCreator { get; set; }
        public IScriptRunner ScriptRunner { get; set; }
        public ITableQueryWriter TableQueryWriter { get; set; }
        public ITflWriter TflWriter { get; set; }
        public IViewWriter ViewWriter { get; set; }

        public string Database { get; set; }
        public string Server { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public int Port { get; set; }
        public string PersistSecurityInfo { get; set; }

        public string File { get; set; }
        public string Folder { get; set; }
        public ErrorMode ErrorMode { get; set; }
        public string Delimiter { get; set; }
        public string LineDelimiter { get; set; }
        public string SearchPattern { get; set; }
        public SearchOption SearchOption { get; set; }

        public int Start { get; set; }
        public int End { get; set; }

        public abstract string UserProperty { get; }
        public abstract string PasswordProperty { get; }
        public abstract string PortProperty { get; }
        public abstract string DatabaseProperty { get; }
        public abstract string ServerProperty { get; }
        public abstract string TrustedProperty { get; }
        public abstract string PersistSecurityInfoProperty { get; }

        public ProviderType Type { get; set; }

        public string L {
            get { return _l; }
            set { _l = value; }
        }

        public string R {
            get { return _r; }
            set { _r = value; }
        }

        public bool IsDatabase { get; set; }
        public bool InsertMultipleRows { get; set; }
        public bool Top { get; set; }
        public bool NoLock { get; set; }
        public bool TableVariable { get; set; }
        public bool NoCount { get; set; }
        public bool MaxDop { get; set; }
        public bool IndexInclude { get; set; }
        public bool Views { get; set; }
        public bool Schemas { get; set; }

        protected AbstractConnection(
            ConnectionConfigurationElement element,
            AbstractConnectionDependencies dependencies
        ) {
            BatchSize = element.BatchSize;
            Name = element.Name;
            Start = element.Start;
            End = element.End;
            File = element.File;
            Folder = element.Folder;
            Delimiter = element.Delimiter;
            LineDelimiter = element.LineDelimiter;
            ErrorMode = (ErrorMode)Enum.Parse(typeof(ErrorMode), element.ErrorMode, true);
            SearchOption = (SearchOption)Enum.Parse(typeof(SearchOption), element.SearchOption, true);
            SearchPattern = element.SearchPattern;

            TableQueryWriter = dependencies.TableQueryWriter;
            ConnectionChecker = dependencies.ConnectionChecker;
            EntityRecordsExist = dependencies.EntityRecordsExist;
            EntityDropper = dependencies.EntityDropper;
            EntityCreator = dependencies.EntityCreator;
            ViewWriter = dependencies.ViewWriter;
            TflWriter = dependencies.TflWriter;
            ScriptRunner = dependencies.ScriptRunner;

            ProcessConnectionString(element);
        }

        private void ProcessConnectionString(ConnectionConfigurationElement element) {
            if (element.ConnectionString != string.Empty) {
                ProcessConnectionString(element.ConnectionString);
            } else {
                Server = element.Server;
                Database = element.Database;
                User = element.User;
                Password = element.Password;
                Port = element.Port;
            }
        }

        private void ProcessConnectionString(string connectionString) {
            Database = ConnectionStringParser.GetDatabaseName(connectionString);
            Server = ConnectionStringParser.GetServerName(connectionString);
            User = ConnectionStringParser.GetUsername(connectionString);
            Password = ConnectionStringParser.GetPassword(connectionString);
            PersistSecurityInfo = ConnectionStringParser.GetPersistSecurityInfo(connectionString);
        }


        public string GetConnectionString() {

            var builder = new DbConnectionStringBuilder { { ServerProperty, Server } };

            if (!string.IsNullOrEmpty(Database)) {
                builder.Add(DatabaseProperty, Database);
            }

            if (!String.IsNullOrEmpty(User)) {
                builder.Add(UserProperty, User);
                builder.Add(PasswordProperty, Password);
            } else {
                if (!String.IsNullOrEmpty(TrustedProperty)) {
                    builder.Add(TrustedProperty, true);
                }
            }

            if (PersistSecurityInfoProperty != string.Empty && PersistSecurityInfo != string.Empty) {
                builder.Add(PersistSecurityInfoProperty, PersistSecurityInfo);
            }

            if (Port <= 0)
                return builder.ConnectionString;

            if (PortProperty == string.Empty) {
                builder[ServerProperty] += "," + Port;
            } else {
                builder.Add("Port", Port);
            }
            return builder.ConnectionString;
        }

        public IDbConnection GetConnection() {
            var type = System.Type.GetType(TypeAndAssemblyName, false, true);
            var connection = (IDbConnection)Activator.CreateInstance(type);
            connection.ConnectionString = GetConnectionString();
            return connection;
        }

        public IScriptReponse ExecuteScript(string script) {
            return ScriptRunner.Execute(this, script);
        }

        public string WriteTemporaryTable(string name, Field[] fields, bool useAlias = true) {
            return TableQueryWriter.WriteTemporary(name, fields, this, useAlias);
        }

        public bool IsReady() {
            var isReady = ConnectionChecker.Check(this);
            return isReady;
        }

        public abstract int NextBatchId(string processName);

        public void AddParameter(IDbCommand command, string name, object val) {
            var parameter = command.CreateParameter();
            parameter.ParameterName = name;
            parameter.Value = val ?? DBNull.Value;
            command.Parameters.Add(parameter);
        }

        public void LoadBeginVersion(Entity entity) {
            var sql = string.Format(@"
                SELECT {0}
                FROM TflBatch b
                INNER JOIN (
                    SELECT @ProcessName AS ProcessName, TflBatchId = MAX(TflBatchId)
                    FROM TflBatch
                    WHERE ProcessName = @ProcessName
                    AND EntityName = @EntityName
                ) m ON (b.ProcessName = m.ProcessName AND b.TflBatchId = m.TflBatchId);
            ", entity.GetVersionField());

            using (var cn = GetConnection()) {
                cn.Open();
                var cmd = cn.CreateCommand();
                cmd.CommandText = sql;
                AddParameter(cmd, "@ProcessName", entity.ProcessName);
                AddParameter(cmd, "@EntityName", entity.Alias);

                using (var reader = cmd.ExecuteReader(CommandBehavior.CloseConnection & CommandBehavior.SingleResult)) {
                    entity.HasRange = reader.Read();
                    entity.Begin = entity.HasRange ? reader.GetValue(0) : null;
                }
            }
        }

        public void LoadEndVersion(Entity entity) {
            var sql = string.Format("SELECT MAX({0}) AS {0} FROM {1};", Enclose(entity.Version.Name), Enclose(entity.Name));

            using (var cn = GetConnection()) {
                var command = cn.CreateCommand();
                command.CommandText = sql;
                command.CommandTimeout = 0;
                cn.Open();
                using (var reader = command.ExecuteReader(CommandBehavior.CloseConnection & CommandBehavior.SingleResult)) {
                    entity.HasRows = reader.Read();
                    entity.End = entity.HasRows ? reader.GetValue(0) : null;
                }
            }
        }

        public bool RecordsExist(Entity entity) {
            return EntityRecordsExist.RecordsExist(this, entity);
        }

        public void Drop(Entity entity) {
            EntityDropper.Drop(this, entity);
        }

        public bool Exists(Entity entity) {
            return EntityDropper.EntityExists.Exists(this, entity);
        }

        public bool IsDelimited() {
            return !string.IsNullOrEmpty(Delimiter);
        }

        public bool IsTrusted() {
            return string.IsNullOrEmpty(User);
        }

        public void DropPrimaryKey(Entity entity) {
            var primaryKey = new FieldSqlWriter(entity.Fields, entity.CalculatedFields).FieldType(entity.IsMaster() ? FieldType.MasterKey : FieldType.PrimaryKey).Alias(L, R).Asc().Values();
            using (var cn = GetConnection()) {
                cn.Open();
                cn.Execute(TableQueryWriter.DropPrimaryKey(entity.OutputName(), primaryKey, entity.Schema));
            }
        }

        public void AddPrimaryKey(Entity entity) {
            var primaryKey = new FieldSqlWriter(entity.Fields, entity.CalculatedFields).FieldType(entity.IsMaster() ? FieldType.MasterKey : FieldType.PrimaryKey).Alias(L, R).Asc().Values();
            using (var cn = GetConnection()) {
                cn.Open();
                cn.Execute(TableQueryWriter.AddPrimaryKey(entity.OutputName(), primaryKey, entity.Schema));
            }
        }

        public void AddUniqueClusteredIndex(Entity entity) {
            using (var cn = GetConnection()) {
                cn.Open();
                cn.Execute(TableQueryWriter.AddUniqueClusteredIndex(entity.OutputName(), entity.Schema));
            }
        }

        public void DropUniqueClusteredIndex(Entity entity) {
            using (var cn = GetConnection()) {
                cn.Open();
                cn.Execute(TableQueryWriter.DropUniqueClusteredIndex(entity.OutputName(), entity.Schema));
            }
        }

        public bool IsExcel() {
            return Type == ProviderType.File && (File.EndsWith(".xlsx", IC) || File.Equals(".xls", IC));
        }

        public bool IsFile() {
            return Type == ProviderType.File;
        }

        public bool IsFolder() {
            return Type == ProviderType.Folder;
        }

        public void Create(Process process, Entity entity) {
            EntityCreator.Create(this, process, entity);
        }

        public Entity TflBatchEntity(string processName) {
            return new Entity(1) { Name = "TflBatch", ProcessName = processName, Alias = "TflBatch", Schema = "dbo", PrimaryKey = new Fields() { new Field(FieldType.PrimaryKey) { Name = "TflBatchId" } } };
        }

        public bool TflBatchRecordsExist(string processName) {
            return RecordsExist(TflBatchEntity(processName));
        }

        public bool TflBatchExists(string processName) {
            return EntityRecordsExist.EntityExists.Exists(this, TflBatchEntity(processName));
        }

        //concrete class should override these
        public virtual string KeyRangeQuery(Entity entity) { throw new NotImplementedException(); }
        public virtual string KeyTopQuery(Entity entity, int top) { throw new NotImplementedException(); }
        public virtual string KeyQuery(Entity entity) { throw new NotImplementedException(); }
        public virtual string KeyAllQuery(Entity entity) { throw new NotImplementedException(); }

        public abstract void WriteEndVersion(AbstractConnection input, Entity entity);

        public string Enclose(string field) {
            return L + field + R;
        }

        public abstract IOperation EntityOutputKeysExtract(Entity entity);

        public abstract IOperation EntityBulkLoad(Entity entity);

        public abstract IOperation EntityBatchUpdate(Entity entity);
    }
}