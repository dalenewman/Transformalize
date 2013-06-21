using System;
using System.Linq;
using Transformalize.Readers;
using Transformalize.Repositories;

namespace Transformalize.Run {
    class Program {
        static void Main(string[] args){

            var name = args[0];
            var mode = args[1] ?? "delta";

            var process = new ProcessReader(name).GetProcess();

            if (mode.Equals("init", StringComparison.OrdinalIgnoreCase)) {
                new EntityTrackerRepository(process).InitializeEntityTracker();
                new OutputRepository(process).InitializeOutput();
            }

            while (process.Entities.Any(kv => !kv.Value.Processed)) {
                using (var etlProcess = new EntityProcess(process)) {
                    etlProcess.Execute();
                }
            }
        }
    }
}
