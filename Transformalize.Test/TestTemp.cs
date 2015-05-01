using System.Diagnostics.Tracing;
using NUnit.Framework;
using Transformalize.Libs.SemanticLogging;
using Transformalize.Logging;
using Transformalize.Main;

namespace Transformalize.Test {

    [TestFixture]
    public class TestTemp : EtlProcessHelper {

        [Test]
        [Ignore("test")]
        public void OneOff() {

            var console = new ObservableEventListener();
            console.EnableEvents(TflEventSource.Log, EventLevel.Informational);
            var sink = console.LogToConsole(new LegacyLogFormatter());

            const string file = "http://localhost/Orchard181/Transformalize/Api/Configuration/26";
            ProcessFactory.CreateSingle(file, new Options { Mode = "first" }).ExecuteScaler();

            Assert.AreEqual(2, 2);

            sink.Dispose();
            console.Dispose();
        }
    }
}