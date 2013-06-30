using System;
using System.Linq;
using Transformalize.NLog;
using Transformalize.Readers;
using Transformalize.Repositories;
using Transformalize.Rhino.Etl.Core;

namespace Transformalize.Run {
    class Program {
        static void Main(string[] args) {

            Guard.Against(args.Length == 0,"You must provide the project name as the first argument.");

            var name = args[0];
            var mode = args.Length > 1 ? args[1] : "delta";
            var logger = LogManager.GetLogger("Transformalize.Run");

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

                // TODO: make a transform process that does this...
                // get all the fields needed to satisfy transform parameters
                // query fields from output table where TflId in (@TflIds), also return TflKey
                // run the transforms on them.
                // run batch update process on transformed output, using TflKey as join
                //var batchIds = process.Entities.Select(e => e.Value.TflId).ToArray();
            }

            watch.Stop();
            logger.Info("{0} | Process completed in {1}.", process.Name, watch.Elapsed);

        }
    }
}
