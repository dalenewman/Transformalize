using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Transformalize.Libs.NLog;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Main;
using Transformalize.Processes;
using Process = Transformalize.Main.Process;

namespace Transformalize.Runner {
    public class InitializeRunner : AbstractProcessRunner, IDisposable {

        private readonly Logger _log = LogManager.GetLogger("tfl");

        public override IEnumerable<Row> Run(Process process) {

            SetLog(process);    

            var result = Enumerable.Empty<Row>();

            var timer = new Stopwatch();
            timer.Start();

            if (!process.IsReady())
                return result;

            process.PerformActions(a => a.Before);

            new InitializationProcess(process).Execute();
            new TemplateManager(process).Manage();

            process.PerformActions(a => a.After);

            timer.Stop();
            _log.Info("Initialized output in {0}.", timer.Elapsed);

            return result;
        }

        public void Dispose() {
            LogManager.Flush();
        }
    }
}