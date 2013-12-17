using System.Collections.Generic;
using Transformalize.Libs.NLog;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Main;
using Transformalize.Processes;

namespace Transformalize.Runner
{
    public class InitializeRunner : IProcessRunner {

        public IEnumerable<IEnumerable<Row>> Run(Process process) {
            var result = new List<IEnumerable<Row>>();

            if (!process.IsReady())
                return result;
            new InitializationProcess(process).Execute();
            if (process.Options.RenderTemplates)
                new TemplateManager(process).Manage();

            return result;
        }

        public void Dispose() {
            LogManager.Flush();
        }
    }
}