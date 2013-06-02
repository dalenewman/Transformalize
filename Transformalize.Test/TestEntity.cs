using System.Configuration;
using System.Linq;
using NUnit.Framework;
using Transformalize.Configuration;
using Transformalize.Model;
using Transformalize.Readers;

namespace Transformalize.Test {
    [TestFixture]
    public class TestEntity {
        [Test]
        public void TestLastVersion() {
            var process = new ProcessReader("Test").GetProcess();

            var entity = process.Entities["OrderDetail"];
            var orderDetailKeys = entity.GetKeys();

            Assert.AreEqual(4, orderDetailKeys.Count());

        }
    }
}
