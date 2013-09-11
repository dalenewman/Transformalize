using Transformalize.Core.Entity_;

namespace Transformalize.Providers.SqlServer
{
    public class SqlServerEntityKeysTopQueryWriter : IEntityQueryWriter
    {
        private readonly int _top;

        public SqlServerEntityKeysTopQueryWriter(int top)
        {
            _top = top;
        }

        const string SQL_PATTERN = @"
            SELECT TOP {0} {1} FROM [{2}] WITH (NOLOCK);
        ";

        public string Write(Entity entity)
        {
            var connection = entity.InputConnection;
            return string.Format(SQL_PATTERN, _top, string.Join(", ", entity.SelectKeys(connection.Provider)), entity.Name );
        }
    }
}