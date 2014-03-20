using Transformalize.Configuration;

namespace Transformalize.Main.Providers.PostgreSql {
    public class PostgreSqlConnection : AbstractConnection {

        /* Pooling=true;Min Pool Size=0;Max Pool Size=100;Connection Lifetime=0; */
        public override string UserProperty { get { return "User ID"; } }
        public override string PasswordProperty { get { return "Password"; } }
        public override string PortProperty { get { return "Port"; } }
        public override string DatabaseProperty { get { return "Database"; } }
        public override string ServerProperty { get { return "Host"; } }
        public override string TrustedProperty { get { return string.Empty; } }
        public override string PersistSecurityInfoProperty {
            get { return string.Empty; }
        }

        public PostgreSqlConnection(Process process, ConnectionConfigurationElement element, AbstractConnectionDependencies dependencies)
            : base(element, dependencies) {
            TypeAndAssemblyName = process.Providers[element.Provider.ToLower()];
        }

        public override string KeyAllQuery(Entity entity) {
            const string sql = @"SELECT {0} FROM ""{1}"";";
            return string.Format(
                sql,
                string.Join(", ", entity.SelectKeys(Provider)),
                entity.Name
            );
        }

        public override string KeyRangeQuery(Entity entity) {

            const string sql = @"
                SELECT {0}
                FROM ""{1}""
                WHERE ""{2}"" BETWEEN @Begin AND @End;
            ";

            return string.Format(
                sql,
                string.Join(", ", entity.SelectKeys(Provider)),
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
                string.Join(", ", entity.SelectKeys(Provider)),
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
                string.Join(", ", entity.SelectKeys(Provider)),
                entity.Name,
                top
            );
        }
    }
}