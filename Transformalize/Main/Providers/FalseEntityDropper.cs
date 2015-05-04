namespace Transformalize.Main.Providers {
    public class NullEntityDropper : IEntityDropper {
        public IEntityExists EntityExists { get; set; }
        public void Drop(AbstractConnection connection, Entity entity) {
            //never dropping anything
        }
    }
}