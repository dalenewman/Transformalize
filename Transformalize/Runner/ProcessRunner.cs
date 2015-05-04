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
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Libs.Rhino.Etl.Pipelines;
using Transformalize.Logging;
using Transformalize.Main;
using Transformalize.Operations;
using Transformalize.Processes;
using Process = Transformalize.Main.Process;

namespace Transformalize.Runner {

    public class ProcessRunner : IProcessRunner {

        public IEnumerable<Row> Run(Process process) {

            var result = Enumerable.Empty<Row>();
            if (!process.IsReady()) {
                return result;
            }

            var timer = new Stopwatch();
            timer.Start();

            process.Setup();
            process.IsFirstRun = process.MasterEntity == null || !process.OutputConnection.RecordsExist(process.MasterEntity);
            process.PerformActions(a => a.Before);

            if (!process.IsFirstRun) {
                ProcessDeletes(process);
            }

            ProcessEntities(ref process);

            if (process.StarEnabled && !process.OutputConnection.Is.Internal()) {
                ProcessMaster(ref process);
            }

            if (process.OutputConnection.Is.Internal()) {
                if (process.Relationships.Any()) {
                    var collector = new CollectorOperation();
                    new MasterJoinProcess(process, ref collector).Execute();
                    process.Results = collector.Rows;
                } else {
                    process.Results = process.MasterEntity == null ? Enumerable.Empty<Row>() : process.MasterEntity.Rows;
                }
            } else {
                process.Results = Enumerable.Empty<Row>();
            }

            if (process.OutputConnection.Is.Internal()) {
                ProcessTransforms(process, new RowsOperation(process.Results));
            } else {
                ProcessTransforms(process, new ParametersExtract(process));
            }

            new TemplateManager(process).Manage();

            process.PerformActions(a => a.After);

            timer.Stop();
            process.Logger.Info( "Process affected {0} records in {1}.", process.Anything, timer.Elapsed);

            process.Complete = true;
            return process.Results;
        }

        private static void ProcessDeletes(Process process) {
            var p = process;
            foreach (var entityDeleteProcess in process.Entities.Where(entity => entity.Delete).Select(entity => new EntityDeleteProcess(p, entity) {
                PipelineExecuter = entity.PipelineThreading == PipelineThreading.SingleThreaded ? (AbstractPipelineExecuter)new SingleThreadedPipelineExecuter() : new ThreadPoolPipelineExecuter()
            })) {
                entityDeleteProcess.Execute();
            }
        }

        private static void ProcessEntities(ref Process process) {
            var p = process;
            if (process.Entities.Count == 1) {
                new EntityProcess(p, process.Entities[0]).Execute();
            } else {
                if (process.Parallel) {
                    process.Entities.AsParallel().ForAll(e => new EntityProcess(p, e) {
                        PipelineExecuter = e.PipelineThreading == PipelineThreading.SingleThreaded ?
                            (IPipelineExecuter)new SingleThreadedPipelineExecuter() :
                            (IPipelineExecuter)new ThreadPoolPipelineExecuter()
                    }.Execute());
                } else {
                    foreach (var entityProcess in process.Entities.Select(entity => new EntityProcess(p, entity) {
                        PipelineExecuter = entity.PipelineThreading == PipelineThreading.SingleThreaded
                            ? (IPipelineExecuter)new SingleThreadedPipelineExecuter()
                            : (IPipelineExecuter)new ThreadPoolPipelineExecuter()
                    })) {
                        entityProcess.Execute();
                    }

                }
            }
        }

        private static void ProcessMaster(ref Process process) {
            var updateMasterProcess = new UpdateMasterProcess(process) {
                PipelineExecuter = process.PipelineThreading == PipelineThreading.SingleThreaded ? (AbstractPipelineExecuter)new SingleThreadedPipelineExecuter() : new ThreadPoolPipelineExecuter()
            };
            updateMasterProcess.Execute();
        }

        private static void ProcessTransforms(Process process, IOperation input) {
            if (process.CalculatedFields.Count <= 0)
                return;
            var transformProcess = new TransformProcess(process, input) {
                PipelineExecuter = process.PipelineThreading == PipelineThreading.SingleThreaded ? (AbstractPipelineExecuter)new SingleThreadedPipelineExecuter() : new ThreadPoolPipelineExecuter()
            };
            transformProcess.Execute();
        }

    }
}