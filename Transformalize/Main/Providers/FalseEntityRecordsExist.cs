namespace Transformalize.Main.Providers {
    public class FalseEntityRecordsExist : IEntityRecordsExist {
        public bool RecordsExist(AbstractConnection connection, Entity entity) {
            return false;
        }
    }
}