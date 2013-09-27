using Transformalize.Libs.Dapper;
using System.Linq;

namespace Transformalize.Main.Providers.MySql {

    public class MySqlEntityRecordsExist : IEntityRecordsExist
    {
        private const string SQL = @"
            SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
            SELECT *
            FROM `{0}`
            LIMIT 1;
            COMMIT;
        ";

        public bool RecordsExist(AbstractConnection connection, string schema, string name)
        {
            using (var cn = connection.GetConnection()) {
                cn.Open();
                var records = cn.Query<string>(string.Format(SQL, name));
                return records != null && records.Any();
            }
        }
    }
}