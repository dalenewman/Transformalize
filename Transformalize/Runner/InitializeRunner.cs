using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Main;
using Transformalize.Processes;
using Process = Transformalize.Main.Process;

namespace Transformalize.Runner {
    public class InitializeRunner : IProcessRunner {

        public IEnumerable<Row> Run(Process process) {

            var result = Enumerable.Empty<Row>();
            if (!process.IsReady()) {
                return result;
            }

            var timer = new Stopwatch();
            timer.Start();

            process.Setup();
            process.PerformActions(a => a.Before);

            new InitializationProcess(process).Execute();
            new TemplateManager(process).Manage();

            process.PerformActions(a => a.After);

            timer.Stop();
            process.Logger.Info("Initialized output in {0}.", timer.Elapsed);

            process.Complete = true;
            return result;
        }

    }
}