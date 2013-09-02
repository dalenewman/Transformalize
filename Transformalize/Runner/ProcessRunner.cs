using System.Linq;
using Transformalize.Core;
using Transformalize.Core.Entity_;
using Transformalize.Core.Process_;
using Transformalize.Core.Template_;
using Transformalize.Libs.Rhino.Etl.Core.Pipelines;
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

            switch (Process.Options.Mode)
            {
                case Modes.Initialize:
                    new InitializationProcess(_process).Execute();
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
            if (Process.Options.RenderTemplates)
                new TemplateManager(_process).Manage();
        }

        private void ProcessTransforms()
        {
            if (Process.CalculatedFields.Count > 0)
            {
                var transformProcess = new TransformProcess(_process);

                if (Process.Options.Mode == Modes.Test)
                    transformProcess.PipelineExecuter = new SingleThreadedNonCachedPipelineExecuter();

                transformProcess.Execute();
            }
        }

        private void ProcessMaster()
        {
            var updateMasterProcess = new UpdateMasterProcess(ref _process);
            if (Process.Options.Mode == Modes.Test)
                updateMasterProcess.PipelineExecuter = new SingleThreadedNonCachedPipelineExecuter();

            updateMasterProcess.Execute();
        }

        private static void ProcessEntities()
        {
            foreach (var entityProcess in Process.Entities.Select(entity => new EntityProcess(entity)))
            {
                if (Process.Options.Mode == Modes.Test)
                    entityProcess.PipelineExecuter = new SingleThreadedNonCachedPipelineExecuter();

                entityProcess.Execute();
            }
        }
    }
}