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
using System.Collections;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using Transformalize.Extensions;
using Transformalize.Libs.EnterpriseLibrary.Validation;
using Transformalize.Libs.NLog;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Main;
using Transformalize.Main.Providers;
using Transformalize.Main.Providers.SqlServer;
using Transformalize.Operations;
using Transformalize.Operations.Extract;
using Transformalize.Operations.Load;
using Transformalize.Operations.Transform;

namespace Transformalize.Processes {

    public class EntityProcess : EtlProcess {
        private const string STANDARD_OUTPUT = "output";
        private readonly Process _process;
        private Entity _entity;
        private readonly ConcurrentDictionary<string, CollectorOperation> _collectors = new ConcurrentDictionary<string, CollectorOperation>();

        public EntityProcess(Process process, Entity entity) {
            GlobalDiagnosticsContext.Set("entity", Common.LogLength(entity.Alias));
            _process = process;
            _entity = entity;
            _collectors[STANDARD_OUTPUT] = new CollectorOperation();
        }

        protected override void Initialize() {

            if (_entity.Input.Count == 1) {
                Register(ComposeInputOperation(_entity.Input.First()));
            } else {
                var union = new ParallelUnionAllOperation();
                foreach (var input in _entity.Input) {
                    union.Add(ComposeInputOperation(input));
                }
                Register(union);
            }

            if (_entity.Sample > 0m && _entity.Sample < 100m) {
                Register(new SampleOperation(_entity.Sample));
            }

            Register(new ApplyDefaults(true, _entity.Fields, _entity.CalculatedFields));
            foreach (var transform in _entity.Operations) {
                Register(transform);
            }

            if (_entity.Group)
                Register(new EntityAggregation(_entity));

            if (_entity.SortingEnabled()) {
                Register(new SortOperation(_entity));
            }

            Register(new TruncateOperation(_entity.Fields, _entity.CalculatedFields));

            var standardOutput = new NamedConnection { Connection = _process.OutputConnection, Name = STANDARD_OUTPUT };

            if (_entity.Output.Count > 0) {
                var branch = new BranchingOperation()
                    .Add(PrepareOutputOperation(standardOutput));
                foreach (var output in _entity.Output) {
                    _collectors[output.Name] = new CollectorOperation();
                    branch.Add(PrepareOutputOperation(output));
                }
                Register(branch);
            } else {
                Register(PrepareOutputOperation(standardOutput));
            }

        }

        private IOperation ComposeInputOperation(NamedConnection input) {

            var p = new PartialProcessOperation();

            var isDatabase = input.Connection.IsDatabase;

            if (isDatabase) {
                if (_entity.HasSqlOverride()) {
                    p.Register(new SqlOverrideOperation(_entity, input.Connection));
                } else {
                    p.Register(new EntityKeysToOperations(_entity, input.Connection));
                    p.Register(new SerialUnionAllOperation());
                }
            } else {
                if (input.Connection.IsFile()) {
                    p.Register(PrepareFileOperation(input.Connection));
                } else {
                    if (input.Connection.IsFolder()) {
                        var union = new SerialUnionAllOperation();
                        foreach (var file in new DirectoryInfo(input.Connection.Folder).GetFiles(input.Connection.SearchPattern, input.Connection.SearchOption)) {
                            input.Connection.File = file.FullName;
                            union.Add(PrepareFileOperation(input.Connection));
                        }
                        p.Register(union);
                    } else {
                        p.Register(_entity.InputOperation);
                        p.Register(new AliasOperation(_entity));
                    }
                }
            }

            return p;
        }

        private IOperation PrepareFileOperation(AbstractConnection connection) {
            if (connection.IsExcel()) {
                return new FileExcelExtract(_entity, connection, _entity.Top + _process.Options.Top);
            }
            if (connection.IsDelimited()) {
                return new FileDelimitedExtract(_entity, connection, _entity.Top + _process.Options.Top);
            }
            return new FileFixedExtract(_entity, connection, _entity.Top + _process.Options.Top);
        }

        private PartialProcessOperation PrepareOutputOperation(NamedConnection nc) {

            var process = new PartialProcessOperation();
            process.Register(new FilterOutputOperation(nc.ShouldRun));

            if (nc.Connection.Type == ProviderType.Internal) {
                process.RegisterLast(_collectors[nc.Name]);
            } else {
                if (_process.IsFirstRun || !_entity.DetectChanges) {
                    process.Register(new EntityAddTflFields(_entity));
                    process.RegisterLast(nc.Connection.EntityBulkLoad(_entity));
                } else {
                    process.Register(new EntityJoinAction(_entity).Right(nc.Connection.EntityOutputKeysExtract(_entity)));
                    var branch = new BranchingOperation()
                        .Add(new PartialProcessOperation()
                            .Register(new EntityActionFilter(ref _entity, EntityAction.Insert))
                            .RegisterLast(nc.Connection.EntityBulkLoad(_entity)))
                        .Add(new PartialProcessOperation()
                            .Register(new EntityActionFilter(ref _entity, EntityAction.Update))
                            .RegisterLast(nc.Connection.EntityBatchUpdate(_entity)));

                    process.RegisterLast(branch);
                }
            }
            return process;
        }

        protected override void PostProcessing() {

            _entity.InputKeys.Clear();

            var errors = GetAllErrors().ToArray();
            if (errors.Any()) {
                foreach (var error in errors) {
                    foreach (var e in error.FlattenHierarchy()) {
                        Error(e.Message);
                        Debug(e.StackTrace);
                    }
                }
                throw new TransformalizeException("Entity Process failed for {0}. See error log.", _entity.Alias);
            }

            if (_process.OutputConnection.Type == ProviderType.Internal) {
                _entity.Rows = _collectors[STANDARD_OUTPUT].Rows;
                foreach (var output in _entity.Output) {
                    if (output.Connection.Type == ProviderType.Internal) {
                        _entity.InternalOutput[output.Name] = _collectors[output.Name].Rows;
                    }
                }
            } else {
                // not handling things by input yet, so just use first
                _process.OutputConnection.WriteEndVersion(_entity.Input.First().Connection, _entity);
            }

            base.PostProcessing();
        }
    }
}