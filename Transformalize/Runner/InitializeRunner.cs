using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Logging;
using Transformalize.Main;
using Transformalize.Processes;
using Process = Transformalize.Main.Process;

namespace Transformalize.Runner {
    public class InitializeRunner : IProcessRunner {

        public IEnumerable<Row> Run(ref Process process) {

            process.CheckIfReady();
            process.Setup();

            var result = Enumerable.Empty<Row>();
            var timer = new Stopwatch();
            timer.Start();

            process.PerformActions(a => a.Before);

            new InitializationProcess(process).Execute();
            new TemplateManager(process).Manage();

            process.PerformActions(a => a.After);

            timer.Stop();
            TflLogger.Info(process.Name, string.Empty, "Initialized output in {0}.", timer.Elapsed);

            process.Complete = true;
            return result;
        }

        public void Dispose() {
            //LogManager.Flush();
        }
    }
}