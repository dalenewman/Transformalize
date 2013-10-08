using Transformalize.Libs.NLog;
using Transformalize.Main;
using Transformalize.Processes;

namespace Transformalize.Runner
{
    public class InitializeRunner : IProcessRunner {

        public void Run(Process process) {
            if (!process.IsReady())
                return;
            new InitializationProcess(process).Execute();
            if (process.Options.RenderTemplates)
                new TemplateManager(process).Manage();
        }

        public void Dispose() {
            LogManager.Flush();
        }
    }
}