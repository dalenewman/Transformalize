using Transformalize.Main.Providers.File;

namespace Transformalize.Main.Providers.Log
{
    public class HtmlDependencies : AbstractConnectionDependencies {
        public HtmlDependencies()
            : base(
                new HtmlProvider(),
                new FalseTableQueryWriter(),
                new FileConnectionChecker(),
                new FileEntityRecordsExist(),
                new FileEntityDropper(),
                new FileEntityCreator(),
                new FalseViewWriter(),
                new FalseTflWriter(),
                new FalseScriptRunner(),
                new FalseProviderSupportsModifier()) { }
    }
}