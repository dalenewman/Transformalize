using Transformalize.Data;
using Transformalize.Libs.NLog;
using Transformalize.Model;
using Transformalize.Processes;

namespace Transformalize.Runner
{
    public abstract class AbstractProcessRunner
    {
        protected Logger Log = LogManager.GetCurrentClassLogger();
        protected Process Process { get; set; }

        public void Run()
        {
            var process = Process;

            if (!process.IsReady()) return;

            switch (process.Options.Mode)
            {
                case Modes.Initialize:
                    using (var initializationProcess = new InitializationProcess(Process))
                    {
                        initializationProcess.Execute();
                    }
                    break;
                default:
                    new EntityRecordsExist(ref process).Check();

                    foreach (var entity in process.Entities)
                    {
                        using (var entityProcess = new EntityProcess(ref process, entity))
                        {
                            entityProcess.Execute();
                        }
                    }

                    using (var masterProcess = new UpdateMasterProcess(ref process))
                    {
                        masterProcess.Execute();
                    }

                    if (process.Transforms.Length > 0)
                    {
                        using (var transformProcess = new TransformProcess(process))
                        {
                            transformProcess.Execute();
                        }
                    }

                    if (process.Options.RenderTemplates)
                    {
                        new TemplateManager(process).Manage();
                    }

                    break;
            }
        }
    }
}