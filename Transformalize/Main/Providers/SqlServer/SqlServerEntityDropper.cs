namespace Transformalize.Main.Providers.SqlServer {
    public class SqlServerEntityDropper : DatabaseEntityDropper {
        public SqlServerEntityDropper(IEntityExists entityExists) : base(entityExists) { }
    }
}