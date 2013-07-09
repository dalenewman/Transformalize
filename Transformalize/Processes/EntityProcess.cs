using System;
using System.Linq;
using Transformalize.Data;
using Transformalize.Model;
using Transformalize.Operations;
using Transformalize.Rhino.Etl.Core;
using Transformalize.Rhino.Etl.Core.Operations;
using Transformalize.Writers;

namespace Transformalize.Processes {

    public class EntityProcess : EtlProcess {

        private readonly Process _process;
        private readonly Entity _entity;
        private readonly IEntityBatch _entityBatch;

        public EntityProcess(ref Process process, Entity entity, IEntityBatch entityBatch = null) : base(process.Name) { 
            _process = process;
            _entity = entity;
            _entityBatch = entityBatch ?? new SqlServerEntityBatch();
        }

        protected override void Initialize() {

            Register(new EntityCreate(_entity, _process));
            Register(new EntityKeysExtract(_entity));
            Register(new EntityKeysToOperations(_entity));
            Register(new SerialUnionAllOperation());
            Register(new EntityDefaults(_entity));
            Register(new EntityTransform(_entity));
            Register(new EntityBatchId(_entity, _entityBatch));
            RegisterLast(new EntityBulkInsert(_entity));
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
            new VersionWriter(_entity).WriteEndVersion(_entity.End, _entity.RecordsAffected);
            base.PostProcessing();
        }

    }
}
