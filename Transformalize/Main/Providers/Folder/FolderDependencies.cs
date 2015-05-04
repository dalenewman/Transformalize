using System.Collections.Generic;
using Transformalize.Logging;

namespace Transformalize.Main.Providers.Folder {
    public class FolderDependencies : AbstractConnectionDependencies {
        public FolderDependencies(ILogger logger)
            : base(
                new NullTableQueryWriter(),
                new FolderConnectionChecker(logger),
                new FolderEntityRecordsExist(),
                new FolderEntityDropper(),
                new FolderEntityCreator(),
                new List<IViewWriter> { new NullViewWriter() },
                new NullTflWriter(),
                new NullScriptRunner(), 
                new NullDataTypeService(), logger) { }
    }
}