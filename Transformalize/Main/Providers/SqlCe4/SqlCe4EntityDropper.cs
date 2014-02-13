namespace Transformalize.Main.Providers.SqlCe4 {
    public class SqlCe4EntityDropper : DatabaseEntityDropper {
        public SqlCe4EntityDropper(IEntityExists entityExists) : base(entityExists) { }
    }
}