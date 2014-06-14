using System.Linq;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Main;
using Transformalize.Main.Providers;
using Transformalize.Operations;
using Transformalize.Operations.Transform;

namespace Transformalize.Processes {

    public class EntityKeysPartial : PartialProcessOperation {
        private readonly Entity _entity;
        private readonly Process _process;

        public EntityKeysPartial(Process process, Entity entity) {
            _process = process;
            _entity = entity;

            if (entity.Input.Count == 1) {
                var connection = entity.Input.First().Connection;
                if (connection.IsDatabase && !entity.HasSqlOverride()) {
                    Register(ComposeInputOperation(connection));
                }
            } else {
                var union = new ParallelUnionAllOperation();
                foreach (var input in entity.Input) {
                    if (input.Connection.IsDatabase && !_entity.HasSqlOverride()) {
                        union.Add(ComposeInputOperation(input.Connection));
                    }
                }
                Register(union);
            }
            Register(new EntityKeysDistinct(_entity));
        }

        private IOperation ComposeInputOperation(AbstractConnection connection) {

            if (connection.Schemas && _entity.Schema.Equals(string.Empty)) {
                _entity.Schema = connection.DefaultSchema;
            }

            if (_entity.HasSqlKeysOverride()) {
                return new SqlKeysOverrideOperation(_entity, connection);
            }

            if (!_entity.PrimaryKey.WithInput().Any()) {
                return new EmptyOperation();
            }

            if (_process.IsFirstRun || !_entity.CanDetectChanges(connection.IsDatabase)) {
                return new EntityInputKeysExtractAll(_entity, connection);
            }

            var operation = new EntityInputKeysExtractDelta(_process, _entity, connection);
            if (operation.NeedsToRun()) {
                return operation;
            }

            return new EmptyOperation();
        }

    }
}