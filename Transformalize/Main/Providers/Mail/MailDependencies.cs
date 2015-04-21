using System.Collections.Generic;

namespace Transformalize.Main.Providers.Mail {
    public class MailDependencies : AbstractConnectionDependencies {
        public MailDependencies()
            : base(
                new FalseTableQueryWriter(),
                new FalseConnectionChecker(),
                new FalseEntityRecordsExist(),
                new FalseEntityDropper(),
                new FalseEntityCreator(),
                new List<IViewWriter> { new FalseViewWriter() },
                new FalseTflWriter(),
                new FalseScriptRunner(),
            new FalseDataTypeService()
            ) { }
    }
}