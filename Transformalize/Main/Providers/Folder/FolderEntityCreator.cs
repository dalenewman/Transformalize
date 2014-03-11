namespace Transformalize.Main.Providers.Folder
{
    public class FolderEntityCreator : IEntityCreator {
        public IEntityExists EntityExists { get; set; }
        public void Create(AbstractConnection connection, Process process, Entity entity) {
            throw new System.NotImplementedException();
        }
    }
}