#region License

// /*
// Transformalize - Replicate, Transform, and Denormalize Your Data...
// Copyright (C) 2013 Dale Newman
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
// */

#endregion

using System.Linq;
using System.Text;
using Transformalize.Extensions;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Logging;
using Transformalize.Main;
using Transformalize.Operations;

namespace Transformalize.Processes {

    public class EntityDeleteProcess : EtlProcess {
        private readonly Process _process;
        private readonly Entity _entity;

        public EntityDeleteProcess(Process process, Entity entity)
            : base(process) {
            _process = process;
            _entity = entity;
        }

        protected override void Initialize() {

            var firstConnection = _entity.Input.First().Connection;
            var singleInput = _entity.Input.Count == 1;

            if (singleInput) {
                Register(
                    firstConnection.Is.Internal() ?
                    _entity.InputOperation :
                    firstConnection.ExtractAllKeysFromInput(_process, _entity)
                );
            } else {
                var multiInput = new ParallelUnionAllOperation();
                foreach (var namedConnection in _entity.Input) {
                    multiInput.Add(namedConnection.Connection.ExtractAllKeysFromInput(_process, _entity));
                }
                Register(multiInput);
            }

            //primary key and/or version may be calculated, so defaults and transformations should be run on them
            if (!_entity.PrimaryKey.All(f => f.Input) || (_entity.Version != null && !_entity.Version.Input)) {
                TflLogger.Warn(_entity.ProcessName, _entity.Alias, "Using a calculated primary key or version to perform deletes requires setting default values and all transformations to run.  The preferred method is to use an input field.");
                Register(new ApplyDefaults(true, new Fields(_entity.Fields, _entity.CalculatedFields)) { EntityName = _entity.Name });
                foreach (var transform in _entity.OperationsBeforeAggregation) {
                    Register(transform);
                }
                foreach (var transform in _entity.OperationsAfterAggregation) {
                    Register(transform);
                }
            }

            Register(new EntityDetectDeletes(_process, _entity).Right(_process.OutputConnection.ExtractAllKeysFromOutput(_entity)));
            Register(new EntityActionFilter(_process, _entity, EntityAction.Delete));
            Register(_process.OutputConnection.Delete(_entity));
        }

        protected override void PostProcessing() {

            _entity.InputKeys = new Row[0];

            var errors = GetAllErrors().ToArray();
            if (errors.Any()) {
                var messageBuilder = new StringBuilder();
                foreach (var error in errors) {
                    foreach (var e in error.FlattenHierarchy()) {
                        TflLogger.Error(_entity.ProcessName, _entity.Name, e.Message);
                        messageBuilder.AppendLine(e.Message);
                        TflLogger.Debug(_entity.ProcessName, _entity.Name, e.StackTrace);
                        messageBuilder.AppendLine(e.StackTrace);
                    }
                }
                throw new TransformalizeException(this.Process.Name, string.Empty, "Entity Delete Process for {0} failed. {1}", _entity.Alias, messageBuilder.ToString());
            }

            base.PostProcessing();
        }
    }
}