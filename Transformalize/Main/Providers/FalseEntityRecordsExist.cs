namespace Transformalize.Main.Providers {
    public class NullEntityRecordsExist : IEntityRecordsExist {
        public IEntityExists EntityExists { get; set; }
        public bool RecordsExist(AbstractConnection connection, Entity entity) {
            return false;
        }
    }
}