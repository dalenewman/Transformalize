using System.Collections.Generic;
using Transformalize.Main.Providers.AnalysisServices;

namespace Transformalize.Main.Providers.Folder {
    public class FolderDependencies : AbstractConnectionDependencies {
        public FolderDependencies()
            : base(
                new FalseTableQueryWriter(),
                new FolderConnectionChecker(),
                new FolderEntityRecordsExist(),
                new FolderEntityDropper(),
                new FolderEntityCreator(),
                new List<IViewWriter> { new FalseViewWriter() },
                new FalseTflWriter(),
                new FalseScriptRunner(), 
                new FalseDataTypeService()) { }
    }
}