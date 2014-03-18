using Transformalize.Libs.Dapper;
using System.Linq;

namespace Transformalize.Main.Providers.MySql {
    public class MySqlEntityRecordsExist : IEntityRecordsExist {
        private readonly IEntityExists _entityExists;

        public MySqlEntityRecordsExist()
        {
            _entityExists = new MySqlEntityExists();
        }

        public bool RecordsExist(AbstractConnection connection, Entity entity) {
            
            if (!_entityExists.Exists(connection, entity))
                return false;

            using (var cn = connection.GetConnection()) {
                cn.Open();
                var records = cn.Query<string>(string.Format(@"
                    SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
                    SELECT `{0}`
                    FROM `{1}`
                    LIMIT 1;
                    COMMIT;
                ", entity.PrimaryKey.First().Key, entity.OutputName()));
                return records != null && records.Any();
            }
        }
    }
}