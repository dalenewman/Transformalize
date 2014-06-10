
namespace Transformalize.Main.Providers.SqlCe {
    public class SqlCeEntityDropper : DatabaseEntityDropper {
        public SqlCeEntityDropper() : base(new SqlCeEntityExists()) { }
    }
}