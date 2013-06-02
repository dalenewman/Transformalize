using System.Configuration;
using Transformalize.Configuration;
using Transformalize.Model;
using Transformalize.Readers;
using Transformalize.Repositories;
using Transformalize.Rhino.Etl.Core;

namespace Transformalize {
    public class TransformalizeProcess : EtlProcess {

        private readonly string _name;

        public TransformalizeProcess(string name) {
            _name = name;
        }

        protected override void Initialize()
        {
            var process = new ProcessReader(_name).GetProcess();

            var entityTrackerRepository = new EntityTrackerRepository(process);
            var outputRepository = new OutputRepository(process);

            entityTrackerRepository.InitializeEntityTracker();
            outputRepository.InitializeOutput();
        }
    }
}
