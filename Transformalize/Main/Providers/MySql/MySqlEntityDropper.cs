namespace Transformalize.Main.Providers.MySql
{
    public class MySqlEntityDropper : DatabaseEntityDropper
    {
        public MySqlEntityDropper(IEntityExists entityExists) : base(entityExists) {}
    }
}