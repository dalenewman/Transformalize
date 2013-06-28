using System.Linq;
using NUnit.Framework;
using Transformalize.Readers;
using Transformalize.Repositories;

namespace Transformalize.Test.Integration {
    [TestFixture]
    public class TestProcess {
        
        [Test]
        public void RunInitAndProcess() {

            var process = new ProcessReader("Test").GetProcess();

            new EntityTrackerRepository(process).InitializeEntityTracker();
            new OutputRepository(process).InitializeOutput();

            while (process.Entities.Any(kv => !kv.Value.Processed)) {
                using (var etlProcess = new EntityProcess(process)) {
                    etlProcess.Execute();
                }
            }
        }

        [Test]
        public void RunProcess() {

            var process = new ProcessReader("Test").GetProcess();

            while (process.Entities.Any(kv => !kv.Value.Processed)) {
                using (var etlProcess = new EntityProcess(process)) {
                    etlProcess.Execute();
                }
            }
        }


    }
}
