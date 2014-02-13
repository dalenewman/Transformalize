using Transformalize.Main.Providers.Internal;

namespace Transformalize.Main.Providers.File {
    public class FileDependencies : AbstractConnectionDependencies {
        public FileDependencies()
            : base(
                new FileProvider(),
                new FalseTableQueryWriter(),
                new FileConnectionChecker(),
                new FileEntityRecordsExist(),
                new FileEntityDropper(),
                new FalseViewWriter(),
                new FalseTflWriter(),
                new FalseScriptRunner(),
                new FalseProviderSupportsModifier()) { }
    }
}