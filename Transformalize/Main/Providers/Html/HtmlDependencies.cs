using System.Collections.Generic;
using Transformalize.Main.Providers.AnalysisServices;
using Transformalize.Main.Providers.File;

namespace Transformalize.Main.Providers.Html
{
    public class HtmlDependencies : AbstractConnectionDependencies {
        public HtmlDependencies()
            : base(
                new FalseTableQueryWriter(),
                new FileConnectionChecker(),
                new FileEntityRecordsExist(),
                new FileEntityDropper(),
                new FileEntityCreator(),
                new List<IViewWriter> { new FalseViewWriter() },
                new FalseTflWriter(),
                new FalseScriptRunner(),
            new FalseDataTypeService()) { }
    }
}