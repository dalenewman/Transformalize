using NUnit.Framework;
using Transformalize.Main;

namespace Transformalize.Test {

    [TestFixture]
    public class TestTemp {

        [Test]
        [Ignore("test")]
        public void OneOff() {

            const string file = "http://localhost/Orchard181/Transformalize/Api/Configuration/1725";
            ProcessFactory.CreateSingle(file, new TestLogger()).ExecuteScaler();
            Assert.AreEqual(2, 2);

        }
    }
}