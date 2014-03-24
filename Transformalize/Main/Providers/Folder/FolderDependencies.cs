namespace Transformalize.Main.Providers.Folder {
    public class FolderDependencies : AbstractConnectionDependencies {
        public FolderDependencies()
            : base(
                new FalseTableQueryWriter(),
                new FolderConnectionChecker(),
                new FolderEntityRecordsExist(),
                new FolderEntityDropper(),
                new FolderEntityCreator(),
                new FalseViewWriter(),
                new FalseTflWriter(),
                new FalseScriptRunner()) { }
    }
}