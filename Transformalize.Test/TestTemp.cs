using System.Diagnostics.Tracing;
using NUnit.Framework;
using Transformalize.Logging;
using Transformalize.Main;

namespace Transformalize.Test {

    [TestFixture]
    public class TestTemp : EtlProcessHelper {

        [Test]
        [Ignore("test")]
        public void OneOff() {

            const string file = "http://localhost/Orchard181/Transformalize/Api/Configuration/26";
            ProcessFactory.CreateSingle(file, new TestLogger(), new Options { Mode = "first" }).ExecuteScaler();

            Assert.AreEqual(2, 2);

        }
    }
}