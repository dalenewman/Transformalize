using System.Linq;
using Transformalize.Model;
using Transformalize.Processes;
using Transformalize.Readers;
using Transformalize.Repositories;

namespace Transformalize {
    public class Runner {
        private readonly string _mode;
        private readonly Process _process;

        public Runner(string process, string mode) {
            _mode = mode.ToLower();
            _process = new ProcessReader(process).Read();
        }

        public void Run() {
            if (!_process.IsReady()) return;

            switch (_mode) {
                case "init":
                    new TflTrackerRepository(_process).Init();
                    //todo:clear entity tables too
                    break;
                default:
                    ProcessEntities();
                    ProcessEntityUpdates();
                    //ProcessTransforms();
                    break;
            }
        }

        private void ProcessEntityUpdates()
        {
            using (var entityUpdateProcess = new UpdateMasterProcess(_process)) {
                entityUpdateProcess.Execute();
            }
        }

        private void ProcessEntities() {
            while (_process.Entities.Any(kv => !kv.Value.Processed)) {
                using (var entityProcess = new EntityProcess(_process)) {
                    entityProcess.Execute();
                }
            }
        }

        private void ProcessTransforms() {
            using (var transform = new TransformProcess(_process)) {
                transform.Execute();
            }
        }

    }
}
