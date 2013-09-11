using Transformalize.Core.Entity_;

namespace Transformalize.Providers.MySql
{
    public class MySqlEntityKeysTopQueryWriter : IEntityQueryWriter
    {
        private readonly int _top;

        public MySqlEntityKeysTopQueryWriter(int top)
        {
            _top = top;
        }

        const string SQL_PATTERN = @"
            SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
            SELECT {0} FROM `{1}` LIMIT 0, {2};
            COMMIT;
        ";

        public string Write(Entity entity)
        {
            var connection = entity.InputConnection;
            return string.Format(SQL_PATTERN, string.Join(", ", entity.SelectKeys(connection.Provider)), entity.Name, _top);
        }
    }
}