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
using Transformalize.Extensions;
using Transformalize.Libs.NLog;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;
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

            GlobalDiagnosticsContext.Set("entity", Common.LogLength(_entity.Alias));

            if (_entity.Input.Count == 1) {
                var connection = _entity.Input.First().Connection;
                Register(
                    connection.Is.Internal() ?
                    _entity.InputOperation :
                    connection.ExtractAllKeysFromInput(_process, _entity)
                );
            } else {
                var multiInput = new ParallelUnionAllOperation();
                foreach (var namedConnection in _entity.Input) {
                    multiInput.Add(namedConnection.Connection.ExtractAllKeysFromInput(_process, _entity));
                }
                Register(multiInput);
            }

            Register(new EntityDetectDeletes(_process, _entity).Right(_process.OutputConnection.ExtractAllKeysFromOutput(_entity)));
            Register(new EntityActionFilter(_process, _entity, EntityAction.Delete));
            Register(_process.OutputConnection.Delete(_entity));
        }

        protected override void PostProcessing() {

            _entity.InputKeys = new Row[0];

            var errors = GetAllErrors().ToArray();
            if (errors.Any()) {
                foreach (var e in errors.SelectMany(error => error.FlattenHierarchy())) {
                    Error(e.Message);
                    Debug(e.StackTrace);
                }
                throw new TransformalizeException("Entity Delete Process for {0} failed. See error log", _entity.Alias);
            }

            base.PostProcessing();
        }
    }
}