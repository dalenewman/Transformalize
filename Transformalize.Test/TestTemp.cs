using NUnit.Framework;
using Transformalize.Main;

namespace Transformalize.Test {

    [TestFixture]
    public class TestTemp {

        [Test]
        [Ignore("test")]
        public void OneOff() {

            const string file = @"https://fleet.scope-services.com/Transformalize/Api/Configuration/55";
            ProcessFactory.CreateSingle(file, new TestLogger(), new Options() { Mode="init"}).ExecuteScaler();
            Assert.AreEqual(2, 2);

        }
    }
}