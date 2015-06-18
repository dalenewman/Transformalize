using NUnit.Framework;
using Transformalize.Main;

namespace Transformalize.Test {

    [TestFixture]
    public class TestTemp {

        [Test]
        [Ignore("test")]
        public void OneOff() {

            const string file = @"path to config here";
            ProcessFactory.CreateSingle(file, new TestLogger(), new Options() { Mode="init"}).ExecuteScaler();
            Assert.AreEqual(2, 2);

        }
    }
}