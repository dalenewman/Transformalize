namespace Transformalize.Main.Providers.PostgreSql
{
    public class PostgreSqlEntityDropper : DatabaseEntityDropper
    {
        public PostgreSqlEntityDropper() : base(new PostgreSqlEntityExists()) {}
    }
}