using System.Data;
using Transformalize.Configuration;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Main.Providers.SqlServer;
using Transformalize.Operations.Transform;

namespace Transformalize.Main.Providers.PostgreSql {
    public class PostgreSqlConnection : AbstractConnection {

        /* Pooling=true;Min Pool Size=0;Max Pool Size=100;Connection Lifetime=0; */
        public override string UserProperty { get { return "User ID"; } }
        public override string PasswordProperty { get { return "Password"; } }
        public override string PortProperty { get { return "Port"; } }
        public override string DatabaseProperty { get { return "Database"; } }
        public override string ServerProperty { get { return "Host"; } }
        public override string TrustedProperty { get { return string.Empty; } }
        public override string PersistSecurityInfoProperty { get { return string.Empty; } }

        public PostgreSqlConnection(ConnectionConfigurationElement element, AbstractConnectionDependencies dependencies)
            : base(element, dependencies) {
            Type = ProviderType.PostgreSql;
            L = "\"";
            R = "\"";
            IsDatabase = true;
            InsertMultipleRows = true;
            Views = true;
            Schemas = true;
            DefaultSchema = "public";
        }

        public override string KeyAllQuery(Entity entity) {
            const string sql = @"SELECT {0} FROM ""{1}"";";
            return string.Format(
                sql,
                string.Join(", ", entity.SelectKeys(this)),
                entity.Name
            );
        }

        public override void WriteEndVersion(AbstractConnection input, Entity entity, bool force = false) {
            //nope  
        }

        public override IOperation EntityOutputKeysExtract(Entity entity) {
            return new EmptyOperation();
        }

        public override IOperation EntityOutputKeysExtractAll(Entity entity) {
            return new EmptyOperation();
        }

        public override IOperation EntityBulkLoad(Entity entity) {
            return new EmptyOperation();
        }

        public override IOperation EntityBatchUpdate(Entity entity) {
            return new EmptyOperation();
        }

        public override int NextBatchId(string processName) {
            var tflEntity = new Entity { TflBatchId = 1, Name = "TflBatch", Alias = "TflBatch", Schema = "dbo", PrimaryKey = new Fields() { new Field(FieldType.PrimaryKey) { Name = "TflBatchId" } } };
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
                FROM ""{1}""
                WHERE ""{2}"" BETWEEN @Begin AND @End;
            ";

            return string.Format(
                sql,
                string.Join(", ", entity.SelectKeys(this)),
                entity.Name,
                entity.Version.Name
            );

        }

        public override string KeyQuery(Entity entity) {

            const string sql = @"
                SELECT {0}
                FROM ""{1}""
                WHERE ""{2}"" <= @End;
            ";

            return string.Format(
                sql,
                string.Join(", ", entity.SelectKeys(this)),
                entity.Name,
                entity.Version.Name
            );

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

        public override EntitySchema GetEntitySchema(Process process, string name, string schema = "", bool isMaster = false) {
            return new DatabaseEntitySchemaReader(this).Read(name, schema);
        }
    }
}