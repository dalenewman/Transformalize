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
using Transformalize.Libs.NLog;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Main;
using Transformalize.Main.Providers;
using Transformalize.Operations;

namespace Transformalize.Processes {

    public class EntityProcess : EtlProcess {
        private readonly Fields _fieldsWithTransforms;
        private readonly Process _process;
        private Entity _entity;

        public EntityProcess(Process process, Entity entity) {
            GlobalDiagnosticsContext.Set("entity", Common.LogLength(entity.Alias, 20));
            _process = process;
            _entity = entity;
            _fieldsWithTransforms = new FieldSqlWriter(entity.All).ExpandXml().HasTransform().Context();
        }

        protected override void Initialize() {
            Register(new EntityKeysToOperations(_entity));
            Register(new SerialUnionAllOperation());
            Register(new ApplyDefaults(_entity.All, _entity.CalculatedFields));
            Register(new TransformFields(_fieldsWithTransforms));
            Register(new TransformFields(_entity.CalculatedFields));

            if (_entity.Group)
                Register(new EntityAggregation(_entity));

            if (_process.IsFirstRun) {
                Register(new EntityAddTflFields(_entity));
                RegisterLast(new EntityBulkInsert(_process, _entity));
            } else {
                Register(new EntityJoinAction(_entity).Right(new EntityOutputKeysExtract(_process, _entity)));
                var branch = new BranchingOperation()
                    .Add(new PartialProcessOperation()
                        .Register(new EntityActionFilter(ref _entity, EntityAction.Insert))
                        .RegisterLast(new EntityBulkInsert(_process, _entity)))
                    .Add(new PartialProcessOperation()
                        .Register(new EntityActionFilter(ref _entity, EntityAction.Update))
                        .RegisterLast(new EntityBatchUpdate(_process, _entity)));
                RegisterLast(branch);
            }
        }

        protected override void PostProcessing() {

            _entity.InputKeys.Clear();

            var errors = GetAllErrors().ToArray();
            if (errors.Any()) {
                foreach (var error in errors) {
                    Error(error.InnerException, "Message: {0}\r\nStackTrace:{1}\r\n", error.Message, error.StackTrace);
                }
                Environment.Exit(1);
            }

            new DatabaseEntityVersionWriter(_process, _entity).WriteEndVersion();

            base.PostProcessing();
        }
    }
}