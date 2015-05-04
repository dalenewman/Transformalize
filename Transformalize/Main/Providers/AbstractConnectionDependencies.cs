using System.Collections.Generic;
using Transformalize.Logging;

namespace Transformalize.Main.Providers {

    public abstract class AbstractConnectionDependencies {

        public ILogger Logger { get; private set; }
        public ITableQueryWriter TableQueryWriter { get; private set; }
        public IConnectionChecker ConnectionChecker { get; private set; }
        public IEntityRecordsExist EntityRecordsExist { get; private set; }
        public IEntityDropper EntityDropper { get; private set; }
        public List<IViewWriter> ViewWriters { get; private set; }
        public ITflWriter TflWriter { get; private set; }
        public IScriptRunner ScriptRunner { get; private set; }
        public IEntityCreator EntityCreator { get; private set; }
        public IDataTypeService DataTypeService { get; private set; }

        protected AbstractConnectionDependencies(
            ITableQueryWriter tableQueryWriter,
            IConnectionChecker connectionChecker,
            IEntityRecordsExist entityRecordsExist,
            IEntityDropper entityDropper,
            IEntityCreator entityCreator,
            List<IViewWriter> viewWriters,
            ITflWriter tflWriter,
            IScriptRunner scriptRunner,
            IDataTypeService dataTypeService,
            ILogger logger
            ) {
            TableQueryWriter = tableQueryWriter;
            ConnectionChecker = connectionChecker;
            EntityRecordsExist = entityRecordsExist;
            EntityDropper = entityDropper;
            EntityCreator = entityCreator;
            ViewWriters = viewWriters;
            TflWriter = tflWriter;
            ScriptRunner = scriptRunner;
            DataTypeService = dataTypeService;
            Logger = logger;
        }
    }
}