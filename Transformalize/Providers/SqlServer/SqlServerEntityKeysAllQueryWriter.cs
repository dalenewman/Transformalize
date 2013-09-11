using Transformalize.Core.Entity_;

namespace Transformalize.Providers.SqlServer
{
    public class SqlServerEntityKeysAllQueryWriter : IEntityQueryWriter
    {
        const string SQL_PATTERN = @"
                SELECT {0} FROM [{1}].[{2}] WITH (NOLOCK);
            ";

        public string Write(Entity entity)
        {
            var connection = entity.InputConnection;
            return string.Format(
                SQL_PATTERN,
                string.Join(", ", entity.SelectKeys(connection.Provider)),
                entity.Schema,
                entity.Name
                );
        }
    }
}