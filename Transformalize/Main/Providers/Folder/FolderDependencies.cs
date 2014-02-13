using Transformalize.Main.Providers.Internal;

namespace Transformalize.Main.Providers.Folder {
    public class FolderDependencies : AbstractConnectionDependencies {
        public FolderDependencies()
            : base(
                new FolderProvider(),
                new FalseTableQueryWriter(),
                new FolderConnectionChecker(),
                new FolderEntityRecordsExist(),
                new FolderEntityDropper(),
                new FalseViewWriter(),
                new FalseTflWriter(),
                new FalseScriptRunner(),
                new FalseProviderSupportsModifier()) { }
    }
}