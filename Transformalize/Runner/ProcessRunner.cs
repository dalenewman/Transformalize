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

using System.IO;
using System.Linq;
using System.Text;
using Transformalize.Libs.Rhino.Etl.Pipelines;
using Transformalize.Main;
using Transformalize.Main.Providers.SqlServer;
using Transformalize.Processes;

namespace Transformalize.Runner
{
    public class ProcessRunner
    {
        private Process _process;

        public ProcessRunner(Process process)
        {
            _process = process;
        }

        public void Run()
        {
            if (!_process.IsReady()) return;

            switch (_process.Options.Mode)
            {
                case Modes.Initialize:
                    new InitializationProcess(_process).Execute();
                    break;
                case Modes.Metadata:
                    var fileName = new FileInfo(Path.Combine(Common.GetTemporaryFolder(_process.Name), "MetaData.xml")).FullName;
                    var writer = new MetaDataWriter(_process, new SqlServerEntityAutoFieldReader());
                    File.WriteAllText(fileName, writer.Write(), Encoding.UTF8);
                    System.Diagnostics.Process.Start(fileName);
                    break;
                case Modes.Delete:
                    ProcessEntityDeletes();
                    break;
                default:
                    new EntityRecordsExist(ref _process).Check();

                    ProcessEntities();
                    ProcessMaster();
                    ProcessTransforms();
                    RenderTemplates();

                    break;
            }
        }

        private void ProcessEntityDeletes()
        {
            foreach (var entityDeleteProcess in _process.Entities.Select(entity => new EntityDeleteProcess(_process, entity))) {
                entityDeleteProcess.Execute();
            }
        }

        private void ProcessEntities() {

            foreach (var entityKeysProcess in _process.Entities.Select(entity => new EntityKeysProcess(_process, entity))) {
                if (_process.Options.Mode == Modes.Test)
                    entityKeysProcess.PipelineExecuter = new SingleThreadedNonCachedPipelineExecuter();

                entityKeysProcess.Execute();
            }

            foreach (var entityProcess in _process.Entities.Select(entity => new EntityProcess(_process, entity))) {
                if (_process.Options.Mode == Modes.Test)
                    entityProcess.PipelineExecuter = new SingleThreadedNonCachedPipelineExecuter();

                entityProcess.Execute();
            }
        }

        private void ProcessMaster() {
            var updateMasterProcess = new UpdateMasterProcess(ref _process);
            if (_process.Options.Mode == Modes.Test)
                updateMasterProcess.PipelineExecuter = new SingleThreadedNonCachedPipelineExecuter();

            updateMasterProcess.Execute();
        }

        private void ProcessTransforms() {
            if (_process.CalculatedFields.Count <= 0) return;

            var transformProcess = new TransformProcess(_process);

            if (_process.Options.Mode == Modes.Test)
                transformProcess.PipelineExecuter = new SingleThreadedNonCachedPipelineExecuter();

            transformProcess.Execute();
        }

        private void RenderTemplates()
        {
            if (_process.Options.RenderTemplates)
                new TemplateManager(_process).Manage();
        }

    }
}