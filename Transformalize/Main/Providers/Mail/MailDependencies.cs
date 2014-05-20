using Transformalize.Main.Providers.AnalysisServices;

namespace Transformalize.Main.Providers.Mail {
    public class MailDependencies : AbstractConnectionDependencies {
        public MailDependencies()
            : base(
                new FalseTableQueryWriter(),
                new FalseConnectionChecker(),
                new FalseEntityRecordsExist(),
                new FalseEntityDropper(),
                new FalseEntityCreator(),
                new FalseViewWriter(),
                new FalseTflWriter(),
                new FalseScriptRunner(),
            new FalseDataTypeService()
            ) { }
    }
}