namespace Transformalize.Main.Providers.PostgreSql
{
    public class PostgreSqlEntityKeysAllQueryWriter : IEntityQueryWriter {
        private const string SQL_PATTERN = @"SELECT {0} FROM ""{1}"";";

        public string Write(Entity entity) {
            var connection = entity.InputConnection;
            return string.Format(
                SQL_PATTERN,
                string.Join(", ", entity.SelectKeys(connection.Provider)),
                entity.Name
                );
        }
    }
}