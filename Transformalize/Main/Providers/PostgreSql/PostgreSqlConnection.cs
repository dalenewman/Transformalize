using Transformalize.Configuration;
using Transformalize.Libs.Rhino.Etl.Operations;
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

        public PostgreSqlConnection(Process process, ConnectionConfigurationElement element, AbstractConnectionDependencies dependencies)
            : base(element, dependencies) {

            TypeAndAssemblyName = process.Providers[element.Provider.ToLower()];
            Type = ProviderType.PostgreSql;
            L = "\"";
            R = "\"";
            IsDatabase = true;
            InsertMultipleRows = true;
            Views = true;
            Schemas = true;
        }

        public override string KeyAllQuery(Entity entity) {
            const string sql = @"SELECT {0} FROM ""{1}"";";
            return string.Format(
                sql,
                string.Join(", ", entity.SelectKeys(this)),
                entity.Name
            );
        }

        public override void WriteEndVersion(AbstractConnection input, Entity entity) {
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
            var tflEntity = new Entity(1) { Name = "TflBatch", Alias = "TflBatch", Schema = "dbo", PrimaryKey = new Fields() { new Field(FieldType.PrimaryKey) { Name = "TflBatchId" } } };
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

        public override string KeyTopQuery(Entity entity, int top) {
            const string sql = @"
                SELECT {0} FROM ""{1}"" LIMIT {2};
            ";

            return string.Format(
                sql,
                string.Join(", ", entity.SelectKeys(this)),
                entity.Name,
                top
            );
        }
    }
}