using System.Linq;
using Transformalize.Libs.Dapper;

namespace Transformalize.Main.Providers.MySql
{
    public class MySqlEntityExists : IEntityExists {

        private const string SQL = @"
            SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
            SELECT TABLE_NAME
            FROM information_schema.tables
            WHERE TABLE_NAME = @name 
            LIMIT 1;
            COMMIT;
        ";

        public bool Exists(AbstractConnection connection, Entity entity) {
            using (var cn = connection.GetConnection()) {
                cn.Open();
                var table = cn.Query<string>(SQL, new { name = entity.OutputName() }).DefaultIfEmpty(string.Empty).FirstOrDefault();
                return !string.IsNullOrEmpty(table);
            }
        }

    }
}