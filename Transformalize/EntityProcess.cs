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
        private readonly bool _isMaster;

        public EntityProcess(Process process)
            : base(process.Name) {
            _process = process;
            _isMaster = !_process.Entities.Any(kv => kv.Value.Processed);
            _entity = _process.Entities.First(kv => !kv.Value.Processed).Value;
        }

        protected override void Initialize() {

            // this is suspicious stuff here -- refactor later 
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
            Register(new EntityDefaults(_entity));
            Register(new EntityTransform(_entity));
            if (_entity.DoBulkInsert()) {
                Info("{0} | Performing Bulk Insert for {1}.", _process.Name, _entity.Name);
                Register(new EntityBatchId(_entity));
                Register(new EntityKeyRegisterLoad(_process, _entity));
                RegisterLast(new EntityBulkInsert(_entity));
            } else {
                Info("{0} | Performing Custom Update for {1}.", _process.Name, _entity.Name);
                Register(new EntityKeyRegisterLoad(_process, _entity));
                RegisterLast(new EntityDatabaseLoad(_entity));
            }

        }

        protected override void PostProcessing() {

            _entity.Dispose();

            var errors = GetAllErrors().ToArray();
            if (errors.Any()) {
                foreach (var error in errors) {
                    Error(error.InnerException, "Message: {0}\r\nStackTrace:{1}\r\n", error.Message, error.StackTrace);
                }
                throw new InvalidOperationException("Houstan.  We have a problem.");
            }

            _entity.Processed = true;
            new VersionWriter(_entity).WriteEndVersion(_entity.End, _entity.RecordsAffected);
            foreach (var pair in _process.KeyRegister) {
                Debug("{0} | {1} {2}(s) saved.", _entity.ProcessName, pair.Value.Count, pair.Key);
            }

            base.PostProcessing();
        }

    }
}
