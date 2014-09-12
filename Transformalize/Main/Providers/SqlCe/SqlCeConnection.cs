using System;
using System.Data;
using Transformalize.Configuration;
using Transformalize.Libs.NLog;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Main.Providers.Sql;
using Transformalize.Operations;
using Transformalize.Operations.Transform;
using Transformalize.Processes;

namespace Transformalize.Main.Providers.SqlCe {

    public class SqlCeConnection : AbstractConnection {

        private readonly Logger _log = LogManager.GetLogger("tfl");

        public SqlCeConnection(ConnectionConfigurationElement element, AbstractConnectionDependencies dependencies)
            : base(element, dependencies) {
            Type = ProviderType.SqlCe4;
            L = "[";
            R = "]";
            IsDatabase = true;
            ConnectionStringProperties.ServerProperty = "Data Source";
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
            const string sql = @"
                SELECT {0} FROM [{1}].[{2}] WITH (NOLOCK);
            ";

            return string.Format(
                sql,
                string.Join(", ", entity.SelectKeys(this)),
                entity.Schema,
                entity.Name
                );
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
            return new EmptyOperation();
        }

        public override IOperation ExtractAllKeysFromInput(Entity entity) {
            return new EmptyOperation();
        }

        public override IOperation ExtractAllKeysFromOutput(Entity entity) {
            return new EmptyOperation();
        }

        public override IOperation Insert(ref Process process, Entity entity) {
            return new EmptyOperation();
        }

        public override IOperation Update(Entity entity) {
            return new EmptyOperation();
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
            return new SqlEntitySchemaReader(this).Read(name, schema);
        }

        public override IOperation Delete(Entity entity) {
            throw new NotImplementedException();
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