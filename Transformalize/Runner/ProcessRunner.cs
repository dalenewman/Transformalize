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

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using Transformalize.Libs.NLog;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Libs.Rhino.Etl.Pipelines;
using Transformalize.Main;
using Transformalize.Operations;
using Transformalize.Operations.Load;
using Transformalize.Processes;
using Process = Transformalize.Main.Process;

namespace Transformalize.Runner {

    public class ProcessRunner : IProcessRunner {

        private readonly Logger _log = LogManager.GetLogger("tfl");

        public IEnumerable<Row> Run(Process process) {

            ResetLog(process);

            var results = Enumerable.Empty<Row>();

            var timer = new Stopwatch();
            timer.Start();

            if (!process.IsReady())
                return results;

            process.PerformActions(a => a.Before);

            ProcessDeletes(process);
            ProcessEntities(process);

            if (process.StarEnabled && !process.OutputConnection.IsInternal()) {
                ProcessMaster(process);
                ProcessTransforms(process);
            }

            new TemplateManager(process).Manage();

            process.PerformActions(a => a.After);

            timer.Stop();
            _log.Info("Process affected {0} records in {1}.", process.Anything, timer.Elapsed);

            if (!process.Entities.Any())
                return Enumerable.Empty<Row>();

            if (!process.Relationships.Any())
                return process.MasterEntity.Rows;

            var collector = new CollectorOperation();
            new MasterJoinProcess(process, ref collector).Execute();
            return collector.Rows;
        }

        private static void ResetLog(Process process) {
            GlobalDiagnosticsContext.Set("process", process.Name);
            GlobalDiagnosticsContext.Set("entity", Common.LogLength("All"));
        }

        private static void ProcessDeletes(Process process) {
            foreach (var entityDeleteProcess in process.Entities.Where(e => e.Delete).Select(entity => new EntityDeleteProcess(process, entity) {
                PipelineExecuter = entity.PipelineThreading == PipelineThreading.SingleThreaded ? (AbstractPipelineExecuter)new SingleThreadedPipelineExecuter() : new ThreadPoolPipelineExecuter()
            })) {
                entityDeleteProcess.Execute();
            }

            ResetLog(process);
        }

        private static void ProcessEntities(Process process) {

            process.IsFirstRun = process.MasterEntity == null || !process.OutputConnection.RecordsExist(process.MasterEntity);

            foreach (var entityProcess in process.Entities.Select(entity => new EntityProcess(process, entity) {
                PipelineExecuter = entity.PipelineThreading == PipelineThreading.SingleThreaded ? (AbstractPipelineExecuter)new SingleThreadedPipelineExecuter() : new ThreadPoolPipelineExecuter()
            })) {
                entityProcess.Execute();
            }

            ResetLog(process);
        }

        private static void ProcessMaster(Process process) {
            var updateMasterProcess = new UpdateMasterProcess(ref process) {
                PipelineExecuter = process.PipelineThreading == PipelineThreading.SingleThreaded ? (AbstractPipelineExecuter)new SingleThreadedPipelineExecuter() : new ThreadPoolPipelineExecuter()
            };
            updateMasterProcess.Execute();

            ResetLog(process);
        }

        private static void ProcessTransforms(Process process) {
            if (process.CalculatedFields.Count <= 0)
                return;
            var transformProcess = new TransformProcess(process) {
                PipelineExecuter = process.PipelineThreading == PipelineThreading.SingleThreaded ? (AbstractPipelineExecuter)new SingleThreadedPipelineExecuter() : new ThreadPoolPipelineExecuter()
            };
            transformProcess.Execute();

            ResetLog(process);
        }

        public void Dispose() {
            LogManager.Flush();
        }

    }

    public class MasterJoinProcess : EtlProcess {
        private readonly Process _process;
        private readonly CollectorOperation _collector;

        public MasterJoinProcess(Process process, ref CollectorOperation collector) {
            _process = process;
            _collector = collector;
        }


        protected override void Initialize() {
            Register(new RowsOperation(_process.Relationships.First().LeftEntity.Rows));
            foreach (var rel in _process.Relationships) {
                Register(new EntityJoinOperation(rel).Right(new RowsOperation(rel.RightEntity.Rows)));
                //Register(new GatherOperation());
            }
            Register(_collector);
        }
    }

    public class EntityJoinOperation : JoinOperation {
        private readonly Relationship _rel;
        private readonly string[] _fields;

        public EntityJoinOperation(Relationship rel) {
            _rel = rel;
            var rightFields = new HashSet<string>(rel.RightEntity.OutputFields().Aliases());
            rightFields.ExceptWith(rel.LeftEntity.OutputFields().Aliases());
            _fields = rightFields.ToArray();
        }

        protected override Row MergeRows(Row leftRow, Row rightRow) {
            var row = leftRow.Clone();
            foreach (var field in _fields) {
                row[field] = rightRow[field];
            }
            return row;
        }

        protected override void SetupJoinConditions() {
            LeftJoin
                .Left(_rel.Join.Select(j => j.LeftField).Select(f => f.Alias).ToArray())
                .Right(_rel.Join.Select(j => j.RightField).Select(f => f.Alias).ToArray());
        }
    }

    public class RowsOperation : AbstractOperation {
        private readonly IEnumerable<Row> _rows;

        public RowsOperation(IEnumerable<Row> rows) {
            _rows = rows;
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            return _rows;
        }
    }
}