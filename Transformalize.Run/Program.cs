using System;
using System.Linq;
using NLog;
using Transformalize.Readers;
using Transformalize.Repositories;

namespace Transformalize.Run {
    class Program {
        static void Main(string[] args) {

            var logger = LogManager.GetLogger("Transformalize.Run");
            var name = args[0];
            var mode = args[1] ?? "delta";

            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();

            var process = new ProcessReader(name).GetProcess();

            if (mode.Equals("init", StringComparison.OrdinalIgnoreCase)) {
                new EntityTrackerRepository(process).InitializeEntityTracker();
                new OutputRepository(process).InitializeOutput();
            }

            if (new ConnectionChecker(process.Connections.Select(kv => kv.Value.ConnectionString), process.Name).Check()) {
                while (process.Entities.Any(kv => !kv.Value.Processed)) {
                    using (var etlProcess = new EntityProcess(process)) {
                        etlProcess.Execute();
                    }
                }
            }

            watch.Stop();
            logger.Info("{0} | Process completed in {1}.", process.Name, watch.Elapsed);

        }
    }
}
