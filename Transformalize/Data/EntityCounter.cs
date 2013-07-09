using Transformalize.Model;
using Transformalize.Rhino.Etl.Core;

namespace Transformalize.Data {
    public class EntityCounter : WithLoggingMixin {
        private readonly Process _process;
        private readonly IEntityCounter _entityCounter;

        public EntityCounter(ref Process process, IEntityCounter entityCounter = null) {
            _process = process;
            _entityCounter = entityCounter ?? new SqlServerEntityCounter(new SqlServerConnectionChecker(process.Name));
        }

        public void Count() {
            foreach (var entity in _process.Entities) {
                entity.Value.InputCount = _entityCounter.CountInput(entity.Value);
                Info("{0} | Entity {1} input has {2} records.", _process.Name, entity.Value.Name, entity.Value.InputCount);
                entity.Value.OutputCount = _entityCounter.CountOutput(entity.Value);
                Info("{0} | Entity {1} output has {2} records.", _process.Name, entity.Value.Name, entity.Value.OutputCount);
            }
        }
    }
}
