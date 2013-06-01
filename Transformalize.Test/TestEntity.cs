using System.Configuration;
using System.Linq;
using NUnit.Framework;
using Transformalize.Configuration;
using Transformalize.Model;

namespace Transformalize.Test {
    [TestFixture]
    public class TestEntity {
        [Test]
        public void TestLastVersion()
        {
            var config = (TransformalizeConfiguration)ConfigurationManager.GetSection("transformalize");
            var process = new Process(config.Processes.Get("Test"));

            var entity = process.Entities["OrderDetail"];
            var orderDetailKeys = entity.GetKeys();

            Assert.AreEqual(4, orderDetailKeys.Count());
            
        }
    }
}
