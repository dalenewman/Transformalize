namespace Transformalize.Main.Providers
{
    public class InternalEntityDropper : IEntityDropper {
        public IEntityExists EntityExists { get; set; }
        public void Drop(AbstractConnection connection, string schema, string name) {
            connection.InputOperation = null;
        }
    }
}