using Transformalize.Libs.Rhino.Etl.Operations;

namespace Transformalize.Main.Providers {

    public abstract class AbstractConnectionDependencies {

        public ITableQueryWriter TableQueryWriter { get; private set; }
        public IConnectionChecker ConnectionChecker { get; private set; }
        public IEntityRecordsExist EntityRecordsExist { get; private set; }
        public IEntityDropper EntityDropper { get; private set; }
        public IViewWriter ViewWriter { get; private set; }
        public ITflWriter TflWriter { get; private set; }
        public IScriptRunner ScriptRunner { get; private set; }
        public IEntityCreator EntityCreator { get; private set; }

        protected AbstractConnectionDependencies(
            ITableQueryWriter tableQueryWriter,
            IConnectionChecker connectionChecker,
            IEntityRecordsExist entityRecordsExist,
            IEntityDropper entityDropper,
            IEntityCreator entityCreator,
            IViewWriter viewWriter,
            ITflWriter tflWriter,
            IScriptRunner scriptRunner
            ) {
            TableQueryWriter = tableQueryWriter;
            ConnectionChecker = connectionChecker;
            EntityRecordsExist = entityRecordsExist;
            EntityDropper = entityDropper;
            EntityCreator = entityCreator;
            ViewWriter = viewWriter;
            TflWriter = tflWriter;
            ScriptRunner = scriptRunner;
            }
    }
}