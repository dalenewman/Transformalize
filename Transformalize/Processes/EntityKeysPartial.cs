using System.Linq;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Main;
using Transformalize.Main.Providers;
using Transformalize.Main.Providers.Sql;
using Transformalize.Operations;
using Transformalize.Operations.Transform;

namespace Transformalize.Processes {

    public class EntityKeysPartial : PartialProcessOperation {
        private readonly Entity _entity;

        public EntityKeysPartial(ref Process process, Entity entity) : base(ref process) {
            _entity = entity;

            if (entity.Input.Count == 1) {
                var connection = entity.Input.First().Connection;
                if (connection.IsDatabase && !entity.HasSqlOverride()) {
                    Register(ComposeInputOperation(ref process, connection));
                }
            } else {
                var union = new ParallelUnionAllOperation();
                foreach (var input in entity.Input) {
                    if (input.Connection.IsDatabase && !_entity.HasSqlOverride()) {
                        union.Add(ComposeInputOperation(ref process, input.Connection));
                    }
                }
                Register(union);
            }
            Register(new EntityKeysDistinct(_entity));
        }

        private IOperation ComposeInputOperation(ref Process process, AbstractConnection connection) {

            if (connection.Schemas && _entity.Schema.Equals(string.Empty)) {
                _entity.Schema = connection.DefaultSchema;
            }

            if (_entity.HasSqlKeysOverride()) {
                return new SqlKeysOverrideOperation(_entity, connection);
            }

            if (!_entity.PrimaryKey.WithInput().Any()) {
                return new EmptyOperation();
            }

            if (process.IsFirstRun || !_entity.CanDetectChanges(connection.IsDatabase)) {
                return connection.ExtractAllKeysFromInput(_entity);
            }

            var operation = new EntityInputKeysExtractDelta(ref process, _entity, connection);
            if (operation.NeedsToRun()) {
                return operation;
            }

            return new EmptyOperation();
        }

    }
}