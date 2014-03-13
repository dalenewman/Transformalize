using System.Collections.Generic;
using System.Linq;
using Transformalize.Libs.NLog;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Main;
using Transformalize.Processes;

namespace Transformalize.Runner
{
    public class DeleteRunner : IProcessRunner {
        public IDictionary<string, IEnumerable<Row>> Run(Process process) {
            var result = new Dictionary<string,IEnumerable<Row>>();

            if (!process.IsReady())
                return result;

            foreach (var entityDeleteProcess in process.Entities.Select(entity => new EntityDeleteProcess(process, entity))) {
                entityDeleteProcess.Execute();
            }
            if (process.Options.RenderTemplates)
                new TemplateManager(process).Manage();

            return result;
        }

        public void Dispose() {
            LogManager.Flush();
        }

    }
}