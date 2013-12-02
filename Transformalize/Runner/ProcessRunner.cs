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

using System.Linq;
using Transformalize.Libs.NLog;
using Transformalize.Libs.Rhino.Etl.Pipelines;
using Transformalize.Main;
using Transformalize.Processes;

namespace Transformalize.Runner {
    public class ProcessRunner : IProcessRunner {

        private AbstractPipelineExecuter _pipelineExecuter = new ThreadPoolPipelineExecuter();

        public void Run(Process process) {
            if (!process.IsReady())
                return;

            if (process.Options.Mode == "test")
                _pipelineExecuter = new SingleThreadedNonCachedPipelineExecuter();

            ProcessEntities(process);
            ProcessMaster(process);
            ProcessTransforms(process);

            if (process.Options.RenderTemplates)
                new TemplateManager(process).Manage();

        }

        private void ProcessEntities(Process process) {

            foreach (var entityKeysProcess in process.Entities.Where(e => e.InputConnection.Provider.IsDatabase).Select(entity => new EntityKeysProcess(process, entity))) {
                entityKeysProcess.Execute();
            }

            foreach (var entityProcess in process.Entities.Select(entity => new EntityProcess(process, entity))) {
                entityProcess.PipelineExecuter = _pipelineExecuter;
                entityProcess.Execute();
            }
        }

        private void ProcessMaster(Process process) {
            var updateMasterProcess = new UpdateMasterProcess(ref process) { PipelineExecuter = _pipelineExecuter };
            updateMasterProcess.Execute();
        }

        private void ProcessTransforms(Process process) {
            if (process.CalculatedFields.Count <= 0)
                return;
            var transformProcess = new TransformProcess(process) { PipelineExecuter = _pipelineExecuter };
            transformProcess.Execute();
        }

        public void Dispose() {
            LogManager.Flush();
        }

    }
}