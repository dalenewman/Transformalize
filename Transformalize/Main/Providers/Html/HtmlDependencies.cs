using System.Collections.Generic;
using Transformalize.Logging;
using Transformalize.Main.Providers.File;

namespace Transformalize.Main.Providers.Html
{
    public class HtmlDependencies : AbstractConnectionDependencies {
        public HtmlDependencies(ILogger logger)
            : base(
                new NullTableQueryWriter(),
                new FileConnectionChecker(logger),
                new FileEntityRecordsExist(),
                new FileEntityDropper(),
                new FileEntityCreator(),
                new List<IViewWriter> { new NullViewWriter() },
                new NullTflWriter(),
                new NullScriptRunner(),
            new NullDataTypeService(), logger) { }
    }
}