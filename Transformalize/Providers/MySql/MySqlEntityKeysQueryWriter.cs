using Transformalize.Core.Entity_;

namespace Transformalize.Providers.MySql
{
    public class MySqlEntityKeysAllQueryWriter : IEntityQueryWriter
    {
        const string SQL_PATTERN = @"
                SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
                SELECT {0} FROM `{1}`;
                COMMIT;
            ";

        public string Write(Entity entity)
        {
            var connection = entity.InputConnection;
            return string.Format(
                SQL_PATTERN,
                string.Join(", ", entity.SelectKeys(connection.Provider)),
                entity.Name
                );
        }
    }


    public class MySqlEntityKeysQueryWriter : IEntityQueryWriter
    {
        const string SQL_PATTERN = @"
                SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
                SELECT {0}
                FROM `{1}`
                WHERE `{2}` <= @End;
                COMMIT;
            ";

        public string Write(Entity entity)
        {
            var connection = entity.InputConnection;
            return string.Format(
                SQL_PATTERN,
                string.Join(", ", entity.SelectKeys(connection.Provider)),
                entity.Name,
                entity.Version.Name
                );
        }
    }
}