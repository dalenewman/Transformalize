using System.Linq;
using Transformalize.Libs.Dapper;

namespace Transformalize.Main.Providers.PostgreSql
{
    public class PostgreSqlEntityRecordsExist : IEntityRecordsExist {
        public IEntityExists EntityExists { get; set; }

        public PostgreSqlEntityRecordsExist() {
            EntityExists = new PostgreSqlEntityExists();
        }

        public bool RecordsExist(AbstractConnection connection, Entity entity) {

            if (!EntityExists.Exists(connection, entity))
                return false;

            using (var cn = connection.GetConnection()) {
                cn.Open();
                var exists = cn.Query<bool>(string.Format(@"
                    SELECT EXISTS(
                        SELECT ""{0}"" 
                        FROM ""{1}""
                        LIMIT 1
                    );
                ", entity.PrimaryKey.First().Alias, entity.OutputName())).DefaultIfEmpty(false).First();
                return exists;
            }
        }
    }
}