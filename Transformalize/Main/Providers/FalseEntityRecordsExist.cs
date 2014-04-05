namespace Transformalize.Main.Providers {
    public class FalseEntityRecordsExist : IEntityRecordsExist {
        public IEntityExists EntityExists { get; set; }
        public bool RecordsExist(AbstractConnection connection, Entity entity) {
            return false;
        }
    }
}