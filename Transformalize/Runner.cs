using Transformalize.Data;
using Transformalize.Model;
using Transformalize.Processes;
using Transformalize.Readers;

namespace Transformalize {
    public class Runner {
        private readonly string _mode;
        private Process _process;

        public Runner(string process, string mode) {
            _mode = mode.ToLower();
            _process = new ProcessReader(process).Read();
        }

        public void Run() {
            if (!_process.IsReady()) return;

            switch (_mode) {
                case "init":
                    new TflBatchRepository(ref _process).Init();
                    new EntityDropper(ref _process).Drop();
                    break;
                default:
                    new EntityCounter(ref _process).Count();
                    ProcessEntities();
                    ProcessMaster();
                    //ProcessTransforms();
                    break;
            }
        }

        private void ProcessEntities() {
            foreach (var entity in _process.Entities) {
                using (var process = new EntityProcess(ref _process, entity.Value)) {
                    process.Execute();
                }
            }
        }

        private void ProcessMaster() {
            using (var process = new UpdateMasterProcess(ref _process)) {
                process.Execute();
            }
        }

        private void ProcessTransforms() {
            using (var process = new TransformProcess(_process)) {
                process.Execute();
            }
        }

    }
}
