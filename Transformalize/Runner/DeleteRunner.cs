using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Transformalize.Libs.NLog;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Main;
using Transformalize.Processes;
using Process = Transformalize.Main.Process;

namespace Transformalize.Runner
{
    public class DeleteRunner : IProcessRunner
    {

        private readonly Logger _log = LogManager.GetLogger("tfl");

        public IDictionary<string, IEnumerable<Row>> Run(Process process) {

            GlobalDiagnosticsContext.Set("process", process.Name);
            GlobalDiagnosticsContext.Set("entity", Common.LogLength("All"));

            var result = new Dictionary<string,IEnumerable<Row>>();

            var timer = new Stopwatch();
            timer.Start();

            if (!process.IsReady())
                return result;

            foreach (var entityDeleteProcess in process.Entities.Select(entity => new EntityDeleteProcess(process, entity))) {
                entityDeleteProcess.Execute();
            }
            new TemplateManager(process).Manage();

            timer.Stop();
            _log.Info("Deleted {0} records in {1}.", process.Anything, timer.Elapsed);

            return result;
        }

        public void Dispose() {
            LogManager.Flush();
        }

    }
}