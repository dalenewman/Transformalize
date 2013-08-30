using Transformalize.Core;
using Transformalize.Core.Entity_;
using Transformalize.Core.Process_;
using Transformalize.Core.Template_;
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

                    foreach (var entity in Process.Entities)
                        new EntityProcess(ref _process, entity).Execute();

                    new UpdateMasterProcess(ref _process).Execute();

                    if (Process.CalculatedFields.Count > 0)
                        new TransformProcess(_process).Execute();

                    if (Process.Options.RenderTemplates)
                        new TemplateManager(_process).Manage();

                    break;
            }
        }
    }
}