using System.Configuration;
using Rhino.Etl.Core;
using Transformalize.Configuration;

namespace Transformalize {
    public class TransformalizeProcess : EtlProcess {

        private readonly string _processName;

        public TransformalizeProcess(string processName) {
            _processName = processName;
        }

        protected override void Initialize() {
            var config = (TransformalizeConfiguration)ConfigurationManager.GetSection("transformalize");
            var process = new ProcessConfiguration(config.Processes.Get(_processName));
            var outputRepository = new OutputRepository(process);

            outputRepository.InitializeEntityTracker();
            outputRepository.InitializeOutput();
        }
    }
}
