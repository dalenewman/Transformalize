using System.Collections.Generic;
using Transformalize.Logging;

namespace Transformalize.Main.Providers.Mail {
    public class MailDependencies : AbstractConnectionDependencies {
        public MailDependencies(ILogger logger)
            : base(
                new NullTableQueryWriter(),
                new NullConnectionChecker(),
                new NullEntityRecordsExist(),
                new NullEntityDropper(),
                new NullEntityCreator(),
                new List<IViewWriter> { new NullViewWriter() },
                new NullTflWriter(),
                new NullScriptRunner(),
            new NullDataTypeService(), logger
            ) { }
    }
}