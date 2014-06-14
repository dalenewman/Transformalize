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
using Transformalize.Libs.EnterpriseLibrary.Common.Configuration;
using Transformalize.Libs.NLog;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Libs.Rhino.Etl.Pipelines;
using Transformalize.Main;
using Transformalize.Main.Providers;
using Transformalize.Processes;
using Process = Transformalize.Main.Process;

namespace Transformalize.Runner {

    public class ProcessRunner : IProcessRunner {

        private readonly Logger _log = LogManager.GetLogger("tfl");

        public IDictionary<string, IEnumerable<Row>> Run(Process process) {

            ResetLog(process);

            var results = new Dictionary<string, IEnumerable<Row>>();

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

            return process.Entities.ToDictionary(e => e.Alias, e => e.Rows);
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

        private void ProcessTransforms(Process process) {
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
}