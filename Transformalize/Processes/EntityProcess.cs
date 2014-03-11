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
using System.IO;
using System.Linq;
using Transformalize.Libs.NLog;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.ConventionOperations;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Main;
using Transformalize.Main.Providers;
using Transformalize.Operations;
using Transformalize.Operations.Extract;
using Transformalize.Operations.Load;

namespace Transformalize.Processes {

    public class EntityProcess : EtlProcess {
        private readonly Process _process;
        private Entity _entity;
        private readonly CollectorOperation _collector = new CollectorOperation();

        public EntityProcess(Process process, Entity entity) {
            GlobalDiagnosticsContext.Set("entity", Common.LogLength(entity.Alias));
            _process = process;
            _entity = entity;
        }

        protected override void Initialize() {

            if (!_entity.InputConnection.Provider.IsDatabase) {
                if (_entity.InputConnection.IsFile()) {
                    Register(PrepareFileOperation(_entity.InputConnection.File));
                } else {
                    if (_entity.InputConnection.IsFolder()) {
                        var union = new SerialUnionAllOperation();
                        foreach (var file in new DirectoryInfo(_entity.InputConnection.Folder).GetFiles(_entity.InputConnection.SearchPattern, _entity.InputConnection.SearchOption)) {
                            union.Add(PrepareFileOperation(file.FullName));
                        }
                        Register(union);
                    } else {
                        Register(_entity.InputOperation);
                        Register(new AliasOperation(_entity));
                    }
                }
            } else {
                if (!string.IsNullOrEmpty(_entity.SqlOverride)) {
                    Register(new ConventionInputCommandOperation(_entity.InputConnection) { Command = _entity.SqlOverride });
                } else {
                    if (_process.IsFirstRun && _entity.UseBcp && _entity.InputConnection.Provider.Type == ProviderType.SqlServer) {
                        Register(new BcpExtract(_process, _entity));
                    } else {
                        Register(new EntityKeysToOperations(_entity));
                        Register(new SerialUnionAllOperation());
                    }
                }

            }

            if (_entity.Sample > 0m && _entity.Sample < 100m) {
                Register(new SampleOperation(_entity.Sample));
            }

            Register(new ApplyDefaults(_entity.Fields, _entity.CalculatedFields));
            foreach (var transform in _entity.Operations) {
                Register(transform);
            }

            if (_entity.Group)
                Register(new EntityAggregation(_entity));

            Register(new TruncateOperation(_entity.Fields, _entity.CalculatedFields));

            if (_process.OutputConnection.Provider.Type == ProviderType.Internal) {
                RegisterLast(_collector);
            } else {
                if (_process.OutputConnection.Provider.Type == ProviderType.File) {
                    RegisterLast(new FileLoadOperation(_process, _entity));
                } else {
                    if (_process.IsFirstRun) {
                        if (_process.OutputConnection.Provider.IsDatabase && _entity.IndexOptimizations) {
                            _process.OutputConnection.DropUniqueClusteredIndex(_entity);
                            _process.OutputConnection.DropPrimaryKey(_entity);
                        }
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
            }
        }

        private IOperation PrepareFileOperation(string file) {
            if (_entity.InputConnection.IsExcel(file)) {
                return new FileExcelExtract(_entity, file, _process.Options.Top);
            }
            if (_entity.InputConnection.IsDelimited()) {
                return new FileDelimitedExtract(_entity, file, _process.Options.Top);
            }
            return new FileFixedExtract(_entity, file, _process.Options.Top);
        }

        protected override void PostProcessing() {

            _entity.InputKeys.Clear();
            if (_process.IsFirstRun && _process.OutputConnection.Provider.IsDatabase && _entity.IndexOptimizations) {
                _process.OutputConnection.AddUniqueClusteredIndex(_entity);
                _process.OutputConnection.AddPrimaryKey(_entity);
            }

            var errors = GetAllErrors().ToArray();
            if (errors.Any()) {
                foreach (var error in errors) {
                    Error(error.InnerException, "Message: {0}\r\nStackTrace:{1}\r\n", error.Message, error.StackTrace);
                }
                Environment.Exit(1);
            }

            if (_process.OutputConnection.Provider.Type == ProviderType.Internal) {
                _entity.Rows = _collector.Rows;
            } else {
                new DatabaseEntityVersionWriter(_process, _entity).WriteEndVersion();
            }

            base.PostProcessing();
        }
    }
}