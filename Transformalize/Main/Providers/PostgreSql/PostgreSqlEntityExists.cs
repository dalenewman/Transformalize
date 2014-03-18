using System.Linq;
using Transformalize.Libs.Dapper;

namespace Transformalize.Main.Providers.PostgreSql
{
    public class PostgreSqlEntityExists : IEntityExists {

        private const string SQL = @"
            SELECT EXISTS(
                SELECT * 
                FROM information_schema.tables 
                WHERE 
                  table_schema = 'public' AND 
                  table_name = @name
            );
        ";

        public bool Exists(AbstractConnection connection, Entity entity) {
            using (var cn = connection.GetConnection()) {
                cn.Open();
                return cn.Query<bool>(SQL, new { name = entity.OutputName() }).DefaultIfEmpty(false).FirstOrDefault();
            }
        }

    }
}