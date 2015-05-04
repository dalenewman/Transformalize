using System.Collections.Generic;
using Transformalize.Logging;

namespace Transformalize.Main.Providers.Lucene {
    public class LuceneDependencies : AbstractConnectionDependencies {
        public LuceneDependencies(string processName, ILogger logger)
            : base(
                new NullTableQueryWriter(),
                new LuceneConnectionChecker(processName, logger),
                new LuceneEntityRecordsExist(),
                new LuceneEntityDropper(),
                new LuceneEntityCreator(),
                new List<IViewWriter> { new NullViewWriter() },
                new LuceneTflWriter(),
                new NullScriptRunner(),
                new NullDataTypeService(), logger) { }
    }
}