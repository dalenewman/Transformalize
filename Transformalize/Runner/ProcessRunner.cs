using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Transformalize.Core;
using Transformalize.Core.Entity_;
using Transformalize.Core.Field_;
using Transformalize.Core.Process_;
using Transformalize.Core.Template_;
using Transformalize.Libs.NLog;
using Transformalize.Libs.Rhino.Etl.Core.Pipelines;
using Transformalize.Processes;
using Transformalize.Providers.SqlServer;
using Encoding = System.Text.Encoding;

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
                default:
                    new EntityRecordsExist(ref _process).Check();

                    ProcessEntities();
                    ProcessMaster();
                    ProcessTransforms();
                    RenderTemplates();

                    break;
            }
        }

        private void RenderTemplates()
        {
            if (_process.Options.RenderTemplates)
                new TemplateManager(_process).Manage();
        }

        private void ProcessTransforms()
        {
            if (_process.CalculatedFields.Count <= 0) return;

            var transformProcess = new TransformProcess(_process);

            if (_process.Options.Mode == Modes.Test)
                transformProcess.PipelineExecuter = new SingleThreadedNonCachedPipelineExecuter();

            transformProcess.Execute();
        }

        private void ProcessMaster()
        {
            var updateMasterProcess = new UpdateMasterProcess(ref _process);
            if (_process.Options.Mode == Modes.Test)
                updateMasterProcess.PipelineExecuter = new SingleThreadedNonCachedPipelineExecuter();

            updateMasterProcess.Execute();
        }

        private void ProcessEntities()
        {
            foreach (var entityKeysProcess in _process.Entities.Select(entity => new EntityKeysProcess(_process, entity)))
            {
                if (_process.Options.Mode == Modes.Test)
                    entityKeysProcess.PipelineExecuter = new SingleThreadedNonCachedPipelineExecuter();

                entityKeysProcess.Execute();
            }

            foreach (var entityProcess in _process.Entities.Select(entity => new EntityProcess(_process, entity)))
            {
                if (_process.Options.Mode == Modes.Test)
                    entityProcess.PipelineExecuter = new SingleThreadedNonCachedPipelineExecuter();

                entityProcess.Execute();
            }
        }
    }
}