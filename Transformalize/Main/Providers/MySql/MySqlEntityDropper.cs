namespace Transformalize.Main.Providers.MySql {

    public class MySqlEntityDropper : DatabaseEntityDropper {
        public MySqlEntityDropper() : base(new MySqlEntityExists()) { }
    }
}