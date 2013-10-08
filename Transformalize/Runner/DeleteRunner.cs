using System.Linq;
using Transformalize.Libs.NLog;
using Transformalize.Main;
using Transformalize.Processes;

namespace Transformalize.Runner
{
    public class DeleteRunner : IProcessRunner {
        public void Run(Process process) {
            if (!process.IsReady())
                return;
            foreach (var entityDeleteProcess in process.Entities.Select(entity => new EntityDeleteProcess(process, entity))) {
                entityDeleteProcess.Execute();
            }
            if (process.Options.RenderTemplates)
                new TemplateManager(process).Manage();
        }

        public void Dispose() {
            LogManager.Flush();
        }

    }
}