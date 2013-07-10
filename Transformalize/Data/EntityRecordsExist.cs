using Transformalize.Model;
using Transformalize.Rhino.Etl.Core;

namespace Transformalize.Data {
    public class EntityRecordsExist : WithLoggingMixin {
        private readonly Process _process;
        private readonly IEntityRecordsExist _entityRecordsExist;

        public EntityRecordsExist(ref Process process, IEntityRecordsExist entityRecordsExist = null) {
            _process = process;
            _entityRecordsExist = entityRecordsExist ?? new SqlServerEntityRecordsExist();
        }

        public void Check() {
            var entity = _process.MasterEntity;
            _process.OutputRecordsExist = _entityRecordsExist.OutputRecordsExist(entity);
            Info(
                _process.OutputRecordsExist
                    ? "{0} | {1}.{2} has records; delta run."
                    : "{0} | {1}.{2} is empty; initial run.",
                _process.Name,
                entity.Schema,
                entity.OutputName()
            );
        }
    }
}
