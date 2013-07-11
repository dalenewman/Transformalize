using System;
using System.Linq;
using Transformalize.Data;
using Transformalize.Model;
using Transformalize.Operations;
using Transformalize.Rhino.Etl.Core;
using Transformalize.Rhino.Etl.Core.Operations;

namespace Transformalize.Processes {

    public class EntityProcess : EtlProcess {

        private readonly Process _process;
        private Entity _entity;

        public EntityProcess(ref Process process, Entity entity, IEntityBatch entityBatch = null)
            : base(process.Name) {
            _process = process;
            _entity = entity;
            _entity.TflBatchId = (entityBatch ?? new SqlServerEntityBatch()).GetNext(_entity);
        }

        protected override void Initialize() {

            Register(new EntityInputKeysExtract(_entity));
            Register(new EntityKeysToOperations(_entity));
            Register(new SerialUnionAllOperation());
            Register(new EntityDefaults(_entity));
            Register(new EntityTransform(_entity));

            if (_process.OutputRecordsExist) {
                Register(new EntityJoinAction(_entity).Right(new EntityOutputKeysExtract(_entity)));
                var branch = new BranchingOperation()
                    .Add(new PartialProcessOperation()
                        .Register(new EntityActionFilter(ref _entity, EntityAction.Insert))
                        .RegisterLast(new EntityBulkInsert(_entity)))
                    .Add(new PartialProcessOperation()
                        .Register(new EntityActionFilter(ref _entity, EntityAction.Update))
                        .RegisterLast(new EntityBatchUpdate(_entity)));
                RegisterLast(branch);
            } else {
                Register(new EntityBatchId(_entity));
                RegisterLast(new EntityBulkInsert(_entity));
            }
        }

        protected override void PostProcessing() {

            _entity.Dispose();

            var errors = GetAllErrors().ToArray();
            if (errors.Any()) {
                foreach (var error in errors) {
                    Error(error.InnerException, "Message: {0}\r\nStackTrace:{1}\r\n", error.Message, error.StackTrace);
                }
                throw new InvalidOperationException("Houstan.  We have a problem in the threads!");
            }
            new SqlServerEntityVersionWriter(_entity).WriteEndVersion(_entity.End, _entity.RecordsAffected);
            base.PostProcessing();
        }

    }
}
