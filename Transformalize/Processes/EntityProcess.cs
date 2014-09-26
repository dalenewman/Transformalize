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

using System.Collections.Concurrent;
using System.Linq;
using Transformalize.Extensions;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Libs.Rhino.Etl.Pipelines;
using Transformalize.Main;
using Transformalize.Main.Providers;
using Transformalize.Operations;
using Transformalize.Operations.Transform;

namespace Transformalize.Processes {

    public class EntityProcess : EtlProcess {

        private const string STANDARD_OUTPUT = "output";
        private readonly Process _process;
        private readonly Entity _entity;
        private readonly ConcurrentDictionary<string, CollectorOperation> _collectors = new ConcurrentDictionary<string, CollectorOperation>();

        public EntityProcess(Process process, Entity entity)
            : base(process) {
            _process = process;
            _entity = entity;
            _collectors[STANDARD_OUTPUT] = new CollectorOperation();
            PipelineExecuter = entity.PipelineThreading == PipelineThreading.SingleThreaded ? (AbstractPipelineExecuter)new SingleThreadedPipelineExecuter() : new ThreadPoolPipelineExecuter();
        }

        protected override void Initialize() {

            Register(new EntityKeysPartial(_process, _entity));

            if (_entity.Input.Count == 1) {
                Register(_entity.Input.First().Connection.Extract(_process, _entity, _process.IsFirstRun));
            } else {
                var union = new ParallelUnionAllOperation();
                foreach (var input in _entity.Input) {
                    union.Add(input.Connection.Extract(_process, _entity, Process.IsFirstRun));
                }
                Register(union);
            }

            if (!_entity.Sampled && _entity.Sample > 0m && _entity.Sample < 100m) {
                Register(new SampleOperation(_entity.Sample) { EntityName = _entity.Name });
            }

            Register(new ApplyDefaults(true, new Fields(_entity.Fields, _entity.CalculatedFields)) { EntityName = _entity.Name });

            foreach (var transform in _entity.OperationsBeforeAggregation) {
                Register(transform);
            }

            if (_entity.Group) {
                Register(new EntityAggregation(_entity));
            }

            foreach (var transform in _entity.OperationsAfterAggregation) {
                Register(transform);
            }

            if (_entity.HasSort()) {
                Register(new SortOperation(_entity) { EntityName = _entity.Name });
            }

            Register(new TruncateOperation(_entity.Fields, _entity.CalculatedFields));

            var standardOutput = new NamedConnection { Connection = _process.OutputConnection, Name = STANDARD_OUTPUT };

            if (_entity.Output.Count > 0) {
                var branch = new BranchingOperation()
                    .Add(PrepareOutputOperation(_process, standardOutput));
                foreach (var output in _entity.Output) {
                    _collectors[output.Name] = new CollectorOperation();
                    branch.Add(PrepareOutputOperation(_process, output));
                }
                Register(branch);
            } else {
                Register(PrepareOutputOperation(_process, standardOutput));
            }

        }

        private PartialProcessOperation PrepareOutputOperation(Process process, NamedConnection nc) {

            var partial = new PartialProcessOperation(process);
            partial.Register(new FilterOutputOperation(nc.ShouldRun) { EntityName = _entity.Name });

            if (nc.Connection.Type == ProviderType.Internal) {
                partial.RegisterLast(_collectors[nc.Name]);
            } else {
                if (Process.IsFirstRun || !_entity.DetectChanges) {
                    partial.Register(new EntityAddTflFields(process, _entity));
                    partial.RegisterLast(nc.Connection.Insert(process, _entity));
                } else {
                    partial.Register(new EntityJoinAction(process, _entity).Right(nc.Connection.ExtractCorrespondingKeysFromOutput(_entity)));
                    var branch = new BranchingOperation()
                        .Add(new PartialProcessOperation(process)
                            .Register(new EntityActionFilter(process, _entity, EntityAction.Insert))
                            .RegisterLast(nc.Connection.Insert(process, _entity)))
                        .Add(new PartialProcessOperation(process)
                            .Register(new EntityActionFilter(process, _entity, EntityAction.Update))
                            .RegisterLast(nc.Connection.Update(_entity)));

                    partial.RegisterLast(branch);
                }
            }
            return partial;
        }

        protected override void PostProcessing() {

            if (_entity.Delete) {
                TflLogger.Info(_entity.ProcessName, _entity.Name, "Processed {0} insert{1}, {2} update{3}, and {4} delete{5} in {6}.", _entity.Inserts, _entity.Inserts.Plural(), _entity.Updates, _entity.Updates.Plural(), _entity.Deletes, _entity.Deletes.Plural(), _entity.Alias);
            } else {
                TflLogger.Info(_entity.ProcessName, _entity.Name, "Processed {0} insert{1}, and {2} update{3} in {4}.", _entity.Inserts, _entity.Inserts.Plural(), _entity.Updates, _entity.Updates.Plural(), _entity.Alias);
            }

            _entity.InputKeys = new Row[0];

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

            if (Process.OutputConnection.Is.Internal()) {
                _entity.Rows = _collectors[STANDARD_OUTPUT].Rows;
            } else {
                // not handling things by input yet, so just use first
                Process.OutputConnection.WriteEndVersion(_process, _entity.Input.First().Connection, _entity);
            }

            base.PostProcessing();
        }
    }
}