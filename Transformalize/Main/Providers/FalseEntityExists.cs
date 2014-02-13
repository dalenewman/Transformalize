namespace Transformalize.Main.Providers {
    public class FalseEntityExists : IEntityExists {
        public bool Exists(AbstractConnection connection, Entity entity) {
            return false;
        }
    }
}