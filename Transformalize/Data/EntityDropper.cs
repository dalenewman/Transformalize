using Transformalize.Model;

namespace Transformalize.Data {
    public class EntityDropper {
        private readonly Process _process;
        private readonly IEntityDropper _dropper;

        public EntityDropper(ref Process process, IEntityDropper dropper = null) {
            _process = process;
            _dropper = dropper ?? new SqlServerEntityDropper();
        }

        public void Drop() {
            foreach (var entity in _process.Entities) {
                _dropper.DropOutput(entity.Value);
            }
        }
    }
}