namespace Transformalize.Main.Providers
{
    public class SqlServerEntityDropper : DatabaseEntityDropper
    {
        public SqlServerEntityDropper(IEntityExists entityExists) : base(entityExists) {}
    }
}