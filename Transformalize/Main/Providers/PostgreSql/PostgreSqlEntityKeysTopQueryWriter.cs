namespace Transformalize.Main.Providers.PostgreSql
{
    public class PostgreSqlEntityKeysTopQueryWriter : IEntityQueryWriter {
        private const string SQL_PATTERN = @"
            SELECT {0} FROM ""{1}"" LIMIT {2};
        ";
        private readonly int _top;

        public PostgreSqlEntityKeysTopQueryWriter(int top) {
            _top = top;
        }

        public string Write(Entity entity) {
            var connection = entity.InputConnection;
            return string.Format(SQL_PATTERN, string.Join(", ", entity.SelectKeys(connection.Provider)), entity.Name, _top);
        }
    }
}