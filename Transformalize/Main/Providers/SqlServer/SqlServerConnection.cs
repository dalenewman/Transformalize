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
using Transformalize.Configuration;
using Transformalize.Libs.NLog;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Main.Providers.Sql;
using Transformalize.Operations;
using Transformalize.Processes;

namespace Transformalize.Main.Providers.SqlServer {

    public class SqlServerConnection : AbstractConnection {

        private readonly Logger _log = LogManager.GetLogger("tfl");

        public SqlServerConnection(ConnectionConfigurationElement element, AbstractConnectionDependencies dependencies)
            : base(element, dependencies) {

            Type = ProviderType.SqlServer;
            L = "[";
            R = "]";
            IsDatabase = true;
            InsertMultipleRows = true;
            Top = true;
            NoLock = true;
            TableVariable = true;
            NoCount = true;
            IndexInclude = true;
            Views = true;
            Schemas = true;
            MaxDop = true;
            TableSample = true;
            DefaultSchema = "dbo";
            ConnectionStringProperties.UserProperty = "User Id";
            ConnectionStringProperties.PasswordProperty = "Password";
            ConnectionStringProperties.DatabaseProperty = "Database";
            ConnectionStringProperties.ServerProperty = "Server";
            ConnectionStringProperties.TrustedProperty = "Trusted_Connection";
            ConnectionStringProperties.PersistSecurityInfoProperty = "Persist Security Info";

        }

        public override int NextBatchId(string processName) {
            var tflEntity = new Entity() { TflBatchId = 1, Name = "TflBatch", Alias = "TflBatch", Schema = "dbo", PrimaryKey = new Fields() { new Field(FieldType.PrimaryKey) { Name = "TflBatchId" } } };
            if (!RecordsExist(tflEntity)) {
                return 1;
            }

            using (var cn = GetConnection()) {
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

        public override string KeyRangeQuery(Entity entity) {

            const string sql = @"
                SELECT {0}
                FROM [{1}].[{2}] WITH (NOLOCK)
                WHERE [{3}] BETWEEN @Begin AND @End
            ";

            return string.Format(
                sql,
                string.Join(", ", entity.SelectKeys(this)),
                entity.Schema,
                entity.Name,
                entity.Version.Name
                );
        }

        public override string KeyQuery(Entity entity) {

            const string sql = @"
                SELECT {0}
                FROM [{1}].[{2}] WITH (NOLOCK)
                WHERE [{3}] <= @End
            ";

            return string.Format(
                sql,
                string.Join(", ", entity.SelectKeys(this)),
                entity.Schema,
                entity.Name,
                entity.Version.Name
            );
        }

        public override string KeyAllQuery(Entity entity) {
            const string format = @"
                SELECT {0} FROM [{1}].[{2}]
            ";

            var sql = string.Format(
                format,
                string.Join(", ", entity.SelectKeys(this)),
                entity.Schema,
                entity.Name
            );

            if (entity.NoLock) {
                sql += " WITH (NOLOCK)";
            }

            if (entity.Sample > 0m && entity.Sample < 100m && TableSample && !entity.Sampled) {
                entity.Sampled = true;
                sql += string.Format(" TABLESAMPLE ({0:##} PERCENT)", entity.Sample);
            }
            return sql;
        }

        public override void WriteEndVersion(AbstractConnection input, Entity entity, bool force = false) {
            //default implementation for relational database
            if (entity.Inserts + entity.Updates > 0 || force) {
                using (var cn = GetConnection()) {
                    cn.Open();

                    var cmd = cn.CreateCommand();

                    if (!entity.CanDetectChanges(input.IsDatabase)) {
                        cmd.CommandText = @"
                            INSERT INTO TflBatch(TflBatchId, ProcessName, EntityName, TflUpdate, Inserts, Updates, Deletes)
                            VALUES(@TflBatchId, @ProcessName, @EntityName, @TflUpdate, @Inserts, @Updates, @Deletes);
                        ";
                    } else {
                        var field = entity.Version.SimpleType.Replace("rowversion", "Binary").Replace("byte[]", "Binary") + "Version";
                        cmd.CommandText = string.Format(@"
                            INSERT INTO TflBatch(TflBatchId, ProcessName, EntityName, {0}, TflUpdate, Inserts, Updates, Deletes)
                            VALUES(@TflBatchId, @ProcessName, @EntityName, @End, @TflUpdate, @Inserts, @Updates, @Deletes);
                        ", field);
                    }

                    cmd.CommandType = CommandType.Text;

                    AddParameter(cmd, "@TflBatchId", entity.TflBatchId);
                    AddParameter(cmd, "@ProcessName", entity.ProcessName);
                    AddParameter(cmd, "@EntityName", entity.Alias);
                    AddParameter(cmd, "@TflUpdate", DateTime.Now);
                    AddParameter(cmd, "@Inserts", entity.Inserts);
                    AddParameter(cmd, "@Updates", entity.Updates);
                    AddParameter(cmd, "@Deletes", entity.Deletes);

                    if (entity.CanDetectChanges(input.IsDatabase)) {
                        var end = new DefaultFactory().Convert(entity.End, entity.Version.SimpleType);
                        AddParameter(cmd, "@End", end);
                    }

                    _log.Debug(cmd.CommandText);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public override IOperation ExtractCorrespondingKeysFromOutput(Entity entity) {
            return new SqlServerEntityKeysExtractFromOutput(this, entity);
        }

        public override IOperation ExtractAllKeysFromOutput(Entity entity) {
            return new SqlEntityKeysExtractAllFromOutput(this, entity);
        }

        public override IOperation ExtractAllKeysFromInput(Entity entity) {
            return new SqlEntityKeysExtractAllFromInput(this, entity);
        }

        public override IOperation Insert(ref Process process, Entity entity) {
            return new SqlServerBulkLoadOperation(this, entity);
        }

        public override IOperation Update(Entity entity) {
            //return new DapperBulkUpdateOperation(this, entity);
            return new SqlServerEntityBatchUpdate(this, entity);
        }

        public override void LoadBeginVersion(Entity entity) {
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

        public override void LoadEndVersion(Entity entity) {
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

        public override Fields GetEntitySchema(Process process, string name, string schema = "", bool isMaster = false) {
            return new SqlServerEntityAutoFieldReader().Read(this, name, string.Empty, name, schema, isMaster);
        }

        public override IOperation Delete(Entity entity) {
            return new SqlEntityDelete(this, entity);
        }

        public override IOperation Extract(ref Process process, Entity entity, bool firstRun) {
            if (Schemas && entity.Schema.Equals(string.Empty)) {
                entity.Schema = DefaultSchema;
            }
            var p = new PartialProcessOperation(ref process);
            if (entity.HasSqlOverride()) {
                p.Register(new SqlOverrideOperation(entity, this));
            } else {
                if (entity.PrimaryKey.WithInput().Any()) {
                    p.Register(new EntityKeysSaveOperation(entity));
                    p.Register(new EntityKeysToOperations(entity, this, firstRun));
                    p.Register(new SerialUnionAllOperation());
                } else {
                    entity.SqlOverride = SqlTemplates.Select(entity, this);
                    p.Register(new SqlOverrideOperation(entity, this));
                }
            }
            return p;
        }
    }
}