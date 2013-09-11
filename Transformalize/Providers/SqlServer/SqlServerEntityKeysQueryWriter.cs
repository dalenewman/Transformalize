using Transformalize.Core.Entity_;

namespace Transformalize.Providers.SqlServer
{
    public class SqlServerEntityKeysQueryWriter : IEntityQueryWriter
    {
        const string SQL_PATTERN = @"
                SELECT {0}
                FROM [{1}].[{2}] WITH (NOLOCK)
                WHERE [{3}] <= @End
            ";

        public string Write(Entity entity)

        {
            var connection = entity.InputConnection;
            return string.Format(
                SQL_PATTERN,
                string.Join(", ", entity.SelectKeys(connection.Provider)),
                entity.Schema,
                entity.Name,
                entity.Version.Name
                );
        }
    }
}