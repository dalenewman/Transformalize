using NUnit.Framework;

namespace Transformalize.Test.Integration {
    [TestFixture]
    public class TestRunner {

        [Test]
        public void Init() {
            new Runner("Test", "init").Run();
        }

        [Test]
        public void Delta() {
            new Runner("Test", "delta").Run();
        }
        
    }
}
