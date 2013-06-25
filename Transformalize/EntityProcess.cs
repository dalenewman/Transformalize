using System;
using System.Linq;
using Transformalize.Model;
using Transformalize.Operations;
using Transformalize.Rhino.Etl.Core;
using Transformalize.Rhino.Etl.Core.Operations;
using Transformalize.Writers;

namespace Transformalize {

    public class EntityProcess : EtlProcess {

        private readonly Process _process;
        private readonly Entity _entity;

        public EntityProcess(Process process)
            : base(process.Name) {
            _process = process;
            _entity = _process.Entities.First(kv => !kv.Value.Processed).Value;
        }

        protected override void Initialize() {
            
            var firstKey = _entity.FirstKey();
            if (_entity.PrimaryKey.Count == 1 && _process.HasRegisteredKey(firstKey)) {
                Register(
                    new ParallelUnionAllOperation()
                        .Add(new RegisteredKeyExtract(_process, firstKey))
                        .Add(new EntityKeysExtract(_entity)
                    )
                );
                Register(new DistinctOperation(_entity.PrimaryKey.Keys));
            }
            else {
                Register(new EntityKeysExtract(_entity));
            }

            Register(new EntityKeysToOperations(_entity));
            Register(new SerialUnionAllOperation());
            Register(new TransformOperation(_entity));
            RegisterLast(new BranchingOperation()
                .Add(new EntityDatabaseLoad(_entity))
                .Add(new EntityKeyRegisterLoad(_process, _entity))
            );
        }

        protected override void PostProcessing() {
            var errors = GetAllErrors().ToArray();
            if (errors.Any()) {
                foreach (var error in errors) {
                    Error(error.InnerException, "Message: {0}\r\nStackTrace:{1}\r\n", error.Message, error.StackTrace);
                }
                throw new InvalidOperationException("Houstan.  We have a problem.");
            }

            _entity.Processed = true;
            new VersionWriter(_entity).WriteEndVersion(_entity.End, _entity.RecordsAffected);
            foreach (var key in _process.KeyRegister.Keys) {
                var set = _process.KeyRegister[key];
                Info("{0} | {1} {2}(s) saved.", _entity.ProcessName, set.Count, key);
            }
            base.PostProcessing();
        }

    }

}
