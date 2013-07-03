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

        [Test]
        public void OrderDetail() {
            new Runner("Test", "entity", "OrderDetail").Run();
        }

        [Test]
        public void Product() {
            new Runner("Test", "entity", "Product").Run();
        }

        [Test]
        public void Customer() {
            new Runner("Test", "entity", "Customer").Run();
        }

        [Test]
        public void Order() {
            new Runner("Test", "entity", "Order").Run();
        }
    }
}
