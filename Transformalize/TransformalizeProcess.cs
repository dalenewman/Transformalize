using System.Configuration;
using Transformalize.Configuration;
using Transformalize.Model;
using Transformalize.Repositories;
using Transformalize.Rhino.Etl.Core;

namespace Transformalize {
    public class TransformalizeProcess : EtlProcess {

        private readonly string _processName;

        public TransformalizeProcess(string processName) {
            _processName = processName;
        }

        protected override void Initialize() {
            var config = (TransformalizeConfiguration)ConfigurationManager.GetSection("transformalize");
            var process = new Process(config.Processes.Get(_processName));

            var entityTrackerRepository = new EntityTrackerRepository(process);
            var outputRepository = new OutputRepository(process);

            entityTrackerRepository.InitializeEntityTracker();
            outputRepository.InitializeOutput();
        }
    }
}
