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
using System.Collections.Concurrent;
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
                    Register(new EntityKeysToOperations(_entity));
                    Register(new SerialUnionAllOperation());
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

            var standardOutput = new Output { Connection = _process.OutputConnection, Name = STANDARD_OUTPUT };

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

        private IOperation PrepareFileOperation(string file) {
            if (_entity.InputConnection.IsExcel(file)) {
                return new FileExcelExtract(_entity, file, _process.Options.Top);
            }
            if (_entity.InputConnection.IsDelimited()) {
                return new FileDelimitedExtract(_entity, file, _process.Options.Top);
            }
            return new FileFixedExtract(_entity, file, _process.Options.Top);
        }

        private PartialProcessOperation PrepareOutputOperation(Output output) {

            var process = new PartialProcessOperation();
            process.Register(new FilterOutputOperation(output.ShouldRun));

            switch (output.Connection.Provider.Type) {
                case ProviderType.Internal:
                    process.RegisterLast(_collectors[output.Name]);
                    break;
                case ProviderType.Console:
                    process.RegisterLast(new ConsoleOperation(_entity));
                    break;
                case ProviderType.Log:
                    process.RegisterLast(new LogOperation(_entity));
                    break;
                case ProviderType.Mail:
                    process.RegisterLast(new MailOperation(_entity));
                    break;
                case ProviderType.File:
                    process.RegisterLast(new FileLoadOperation(output.Connection, _entity));
                    break;
                case ProviderType.Html:
                    process.Register(new HtmlRowOperation(_entity, "HtmlRow"));
                    process.RegisterLast(new HtmlLoadOperation(output.Connection, _entity, "HtmlRow"));
                    break;
                default:
                    if (_process.IsFirstRun) {
                        if (output.Connection.Provider.IsDatabase && _entity.IndexOptimizations) {
                            output.Connection.DropUniqueClusteredIndex(_entity);
                            output.Connection.DropPrimaryKey(_entity);
                        }
                        process.Register(new EntityAddTflFields(_entity));
                        process.RegisterLast(new EntityBulkInsert(output.Connection, _entity));
                    } else {
                        process.Register(new EntityJoinAction(_entity).Right(new EntityOutputKeysExtract(output.Connection, _entity)));
                        var branch = new BranchingOperation()
                            .Add(new PartialProcessOperation()
                                .Register(new EntityActionFilter(ref _entity, EntityAction.Insert))
                                .RegisterLast(new EntityBulkInsert(output.Connection, _entity)))
                            .Add(new PartialProcessOperation()
                                .Register(new EntityActionFilter(ref _entity, EntityAction.Update))
                                .RegisterLast(new EntityBatchUpdate(output.Connection, _entity)));
                        process.RegisterLast(branch);
                    }
                    break;
            }
            return process;
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
                LogManager.Flush();
                Environment.Exit(1);
            }

            if (_process.OutputConnection.Provider.Type == ProviderType.Internal) {
                _entity.Rows = _collectors[STANDARD_OUTPUT].Rows;
                foreach (var output in _entity.Output) {
                    if (output.Connection.Provider.Type == ProviderType.Internal) {
                        _entity.InternalOutput[output.Name] = _collectors[output.Name].Rows;
                    }
                }
            } else {
                new DatabaseEntityVersionWriter(_process, _entity).WriteEndVersion();
            }

            base.PostProcessing();
        }
    }
}