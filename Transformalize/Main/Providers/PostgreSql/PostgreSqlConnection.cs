using System;
using System.Data;
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Main.Providers.Sql;
using Transformalize.Operations;
using Transformalize.Processes;

namespace Transformalize.Main.Providers.PostgreSql {
    public class PostgreSqlConnection : AbstractConnection {

        /* Pooling=true;Min Pool Size=0;Max Pool Size=100;Connection Lifetime=0; */

        public PostgreSqlConnection(TflConnection element, AbstractConnectionDependencies dependencies)
            : base(element, dependencies) {
            Type = ProviderType.PostgreSql;
            L = "\"";
            R = "\"";
            TextQualifier = "'";
            IsDatabase = true;
            InsertMultipleRows = true;
            Views = true;
            Schemas = true;
            DefaultSchema = "public";
            ConnectionStringProperties.UserProperty = "User ID";
            ConnectionStringProperties.PasswordProperty = "Password";
            ConnectionStringProperties.PortProperty = "Port";
            ConnectionStringProperties.DatabaseProperty = "Database";
            ConnectionStringProperties.ServerProperty = "Host";
            NoLock = false;
        }

        public override string KeyAllQuery(Entity entity) {
            var sql = string.Format(
                @"SELECT {0} FROM {1} ",
                string.Join(", ", entity.SelectKeys(this)),
                Enclose(entity.Name)
            );

            if (entity.Filters.Any()) {
                sql += " WHERE " + entity.Filters.ResolveExpression(TextQualifier);
            }

            sql += ";";
            return sql;

        }

        public override void WriteEndVersion(Process process, AbstractConnection input, Entity entity, bool force = false) {
            throw new NotImplementedException();
        }

        public override IOperation ExtractCorrespondingKeysFromOutput(Entity entity) {
            return new SqlEntityKeysExtractAllFromOutput(this, entity);
        }

        public override IOperation ExtractAllKeysFromOutput(Entity entity) {
            return new SqlEntityKeysExtractAllFromOutput(this, entity);
        }

        public override IOperation ExtractAllKeysFromInput(Process process, Entity entity) {
            return new SqlEntityKeysExtractAllFromInput(this, entity);
        }

        public override IOperation Insert(Process process, Entity entity) {
            throw new NotImplementedException();
        }

        public override IOperation Update(Entity entity) {
            throw new NotImplementedException();
        }

        public override int NextBatchId(string processName) {
            var tflEntity = TflBatchEntity(processName);
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
            var sql = string.Format(
                @"SELECT {0} FROM {1} WHERE {2} BETWEEN @Begin AND @End ",
                string.Join(", ", entity.SelectKeys(this)),
                Enclose(entity.Name),
                Enclose(entity.Version.Name)
            );

            if (entity.Filters.Any()) {
                sql += " AND " + entity.Filters.ResolveExpression(TextQualifier);
            }

            sql += ";";
            return sql;
        }

        public override string KeyQuery(Entity entity) {
            var sql = string.Format(
                @"SELECT {0} FROM {1} WHERE {2} <= @End ",
                string.Join(", ", entity.SelectKeys(this)),
                Enclose(entity.Name),
                Enclose(entity.Version.Name)
            );

            if (entity.Filters.Any()) {
                sql += " AND " + entity.Filters.ResolveExpression(TextQualifier);
            }
            sql += ";";
            return sql;
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

        public override Fields GetEntitySchema(Process process, Entity entity, bool isMaster = false) {
            return new SqlEntitySchemaReader(this).Read(entity.Name, entity.Schema);
        }

        public override IOperation Delete(Entity entity) {
            return new SqlEntityDelete(this, entity);
        }

        public override IOperation Extract(Process process, Entity entity, bool firstRun) {
            if (Schemas && entity.Schema.Equals(string.Empty)) {
                entity.Schema = DefaultSchema;
            }

            var p = new PartialProcessOperation(process);
            if (entity.HasSqlOverride()) {
                p.Register(new SqlOverrideOperation(entity, this));
            } else {
                if (entity.PrimaryKey.WithInput().Any()) {
                    p.Register(new EntityKeysSaveOperation(entity));
                    p.Register(new EntityKeysToOperations(ref entity, this, firstRun));
                    p.Register(new SerialUnionAllOperation(entity));
                } else {
                    entity.SqlOverride = SqlTemplates.Select(entity, this);
                    p.Register(new SqlOverrideOperation(entity, this));
                }
            }
            return p;

        }
    }
}