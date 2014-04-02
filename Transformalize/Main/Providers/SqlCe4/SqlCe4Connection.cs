using Transformalize.Configuration;

namespace Transformalize.Main.Providers.SqlCe4 {
    public class SqlCe4Connection : AbstractConnection {

        public override string UserProperty { get { return string.Empty; } }
        public override string PasswordProperty { get { return string.Empty; } }
        public override string PortProperty { get { return string.Empty; } }
        public override string DatabaseProperty { get { return string.Empty; } }
        public override string ServerProperty { get { return "Data Source"; } }
        public override string TrustedProperty { get { return string.Empty; } }
        public override string PersistSecurityInfoProperty { get { return "Persist Security Info"; } }

        public SqlCe4Connection(Process process, ConnectionConfigurationElement element, AbstractConnectionDependencies dependencies)
            : base(element, dependencies) {

            TypeAndAssemblyName = process.Providers[element.Provider.ToLower()];
            Type = ProviderType.SqlCe4;
            L = "[";
            R = "]";
            IsDatabase = true;
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

        public override string KeyTopQuery(Entity entity, int top) {
            const string sql = @"
                SELECT TOP {0} {1} FROM [{2}] WITH (NOLOCK);
            ";
            return string.Format(sql, top, string.Join(", ", entity.SelectKeys(this)), entity.Name);
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

    }
}