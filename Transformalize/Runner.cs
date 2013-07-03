using System.Linq;
using Transformalize.Model;
using Transformalize.Readers;
using Transformalize.Repositories;

namespace Transformalize {
    public class Runner {
        private readonly string _mode;
        private readonly Process _process;

        public Runner(string process, string mode, string entity = "") {
            _mode = mode.ToLower();
            _process = mode.Equals("entity") ?
                new ProcessReader(process).GetSingleEntityProcess(entity) :
                new ProcessReader(process).GetProcess();
        }

        public void Run() {
            if (!new ConnectionChecker(_process.Connections.Select(kv => kv.Value.ConnectionString), _process.Name).Check()) return;

            switch (_mode) {
                case "init":
                    new TflTrackerRepository(_process).Init();
                    new OutputRepository(_process).Init();
                    break;
                case "entity":
                    new TflTrackerRepository(_process).Init();
                    new OutputRepository(_process).Init();
                    ProcessEntities();
                    break;
                default:
                    ProcessEntities();
                    ProcessTransforms();
                    break;
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
