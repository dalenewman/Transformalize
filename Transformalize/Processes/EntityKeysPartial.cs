using System.Linq;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Main;
using Transformalize.Main.Providers;
using Transformalize.Main.Providers.Sql;
using Transformalize.Operations;
using Transformalize.Operations.Transform;

namespace Transformalize.Processes {

    public class EntityKeysPartial : PartialProcessOperation {
        private readonly Process _process;
        private readonly Entity _entity;

        public EntityKeysPartial(Process process, Entity entity) : base(process) {
            _process = process;
            _entity = entity;

            if (entity.Input.Count == 1) {
                var connection = entity.Input.First().Connection;
                if (connection.IsDatabase && !entity.HasSqlOverride()) {
                    Register(ComposeInputOperation(process, connection));
                }
            } else {
                var union = new ParallelUnionAllOperation();
                foreach (var input in entity.Input) {
                    if (input.Connection.IsDatabase && !_entity.HasSqlOverride()) {
                        union.Add(ComposeInputOperation(process, input.Connection));
                    }
                }
                Register(union);
            }
            Register(new EntityKeysDistinct(_entity));
        }

        private IOperation ComposeInputOperation(Process process, AbstractConnection connection) {

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
                return connection.ExtractAllKeysFromInput(_process, _entity);
            }

            var operation = new EntityInputKeysExtractDelta(process, _entity, connection);
            if (operation.NeedsToRun()) {
                return operation;
            }

            return new EmptyOperation();
        }

    }
}