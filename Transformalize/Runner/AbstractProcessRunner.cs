using Transformalize.Core;
using Transformalize.Core.Entity_;
using Transformalize.Core.Process_;
using Transformalize.Core.Template_;
using Transformalize.Libs.NLog;
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
                    if (process.Options.Mode != Modes.Test)
                        new EntityRecordsExist(ref process).Check();

                    foreach (var entity in Process.Entities)
                        new EntityProcess(ref process, entity).Execute();

                    if (process.Options.Mode != Modes.Test)
                        new UpdateMasterProcess(ref process).Execute();

                    if (Process.CalculatedFields.Count > 0)
                        new TransformProcess(process).Execute();

                    if (process.Options.RenderTemplates)
                        new TemplateManager(process).Manage();

                    break;
            }
        }
    }
}