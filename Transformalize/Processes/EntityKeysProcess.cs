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

using System;
using System.Linq;
using Transformalize.Extensions;
using Transformalize.Libs.NLog;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Main;
using Transformalize.Main.Providers;
using Transformalize.Operations;
using Transformalize.Operations.Transform;

namespace Transformalize.Processes {

    public class EntityKeysProcess : EtlProcess {
        private readonly Entity _entity;
        private readonly Process _process;

        public EntityKeysProcess(Process process, Entity entity) {
            _process = process;
            _entity = entity;
        }

        protected override void Initialize() {

            GlobalDiagnosticsContext.Set("process", _process.Name);
            GlobalDiagnosticsContext.Set("entity", Common.LogLength(_entity.Alias, 20));

            if (_entity.Input.Count == 1) {
                var connection = _entity.Input.First().Connection;
                if (connection.IsDatabase && !_entity.HasSqlOverride()) {
                    Register(ComposeInputOperation(connection));
                }
            } else {
                var union = new ParallelUnionAllOperation();
                foreach (var input in _entity.Input) {
                    if (input.Connection.IsDatabase && !_entity.HasSqlOverride()) {
                        union.Add(ComposeInputOperation(input.Connection));
                    }
                }
                Register(union);
            }
            RegisterLast(new EntityInputKeysStore(_entity));
        }

        private IOperation ComposeInputOperation(AbstractConnection connection) {

            if (connection.Schemas && _entity.Schema.Equals(string.Empty)) {
                _entity.Schema = connection.DefaultSchema;
            }

            if (_entity.HasSqlKeysOverride()) {
                return new SqlKeysOverrideOperation(_entity, connection);
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

        protected override void PostProcessing() {
            var errors = GetAllErrors().ToArray();
            if (errors.Any()) {
                foreach (var error in errors) {
                    foreach (var e in error.FlattenHierarchy()) {
                        Error(e.Message);
                        Debug(e.StackTrace);
                    }
                }
                throw new TransformalizeException("Entity Keys Process failed for {0}. See error log.", _entity.Alias);
            }

            base.PostProcessing();
        }
    }
}