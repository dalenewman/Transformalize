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
using System.Linq;
using Transformalize.Libs.NLog;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Pipelines;
using Transformalize.Main;
using Transformalize.Main.Providers;
using Transformalize.Processes;

namespace Transformalize.Runner {

    public class ProcessRunner : IProcessRunner {

        public IDictionary<string,IEnumerable<Row>> Run(Process process) {

            var results = new Dictionary<string, IEnumerable<Row>>();

            if (!process.IsReady())
                return results;

            ProcessDeletes(process);
            ProcessEntities(process);
            ProcessMaster(process);
            ProcessTransforms(process);

            if (process.Options.RenderTemplates)
                new TemplateManager(process).Manage();

            return process.Entities.ToDictionary(e => e.Alias, e => e.Rows);
        }

        private static void ProcessDeletes(Process process) {
            foreach (var entityDeleteProcess in process.Entities.Where(e => e.Delete).Select(entity => new EntityDeleteProcess(process, entity) {
                PipelineExecuter = entity.PipelineThreading == PipelineThreading.SingleThreaded ? (AbstractPipelineExecuter)new SingleThreadedPipelineExecuter() : new ThreadPoolPipelineExecuter()
            })) {
                entityDeleteProcess.Execute();
            }
        }

        private static void ProcessEntities(Process process) {

            process.IsFirstRun = !process.OutputConnection.RecordsExist(process.MasterEntity);

            foreach (var entityKeysProcess in process.Entities.Select(entity => new EntityKeysProcess(process, entity) {
                PipelineExecuter = entity.PipelineThreading == PipelineThreading.SingleThreaded ? (AbstractPipelineExecuter)new SingleThreadedPipelineExecuter() : new ThreadPoolPipelineExecuter()
            })) {
                entityKeysProcess.Execute();
            }

            foreach (var entityProcess in process.Entities.Select(entity => new EntityProcess(process, entity) {
                PipelineExecuter = entity.PipelineThreading == PipelineThreading.SingleThreaded ? (AbstractPipelineExecuter)new SingleThreadedPipelineExecuter() : new ThreadPoolPipelineExecuter()
            })) {
                entityProcess.Execute();
            }
        }

        private static void ProcessMaster(Process process) {
            if (process.OutputConnection.Type == ProviderType.Internal)
                return;
            var updateMasterProcess = new UpdateMasterProcess(ref process) {
                PipelineExecuter = process.Options.Mode.Equals("test") ? (AbstractPipelineExecuter)new SingleThreadedPipelineExecuter() : new ThreadPoolPipelineExecuter()
            };
            updateMasterProcess.Execute();
        }

        private static void ProcessTransforms(Process process) {
            if (process.CalculatedFields.Count <= 0)
                return;
            if (process.OutputConnection.Type == ProviderType.Internal)
                return;
            var transformProcess = new TransformProcess(process) {
                PipelineExecuter = process.Options.Mode.Equals("test") ? (AbstractPipelineExecuter)new SingleThreadedPipelineExecuter() : new ThreadPoolPipelineExecuter()
            };
            transformProcess.Execute();
        }

        public void Dispose() {
            LogManager.Flush();
        }

    }
}